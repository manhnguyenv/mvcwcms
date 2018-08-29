using MVCwCMS.Helpers;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCwCMS.Controllers
{
    public class InstallationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (RouteTable.Routes["Installation"].IsNull())
            {
                return Redirect("~/");
            }

            BackEndInstallation backEndInstallation = new BackEndInstallation()
            {
                SqlServerName = "",
                DatabaseName = "",
                CreateDbIfDoesNotExist = true,
                IgnoreDbExistsWarning = false,
                ResetDbIfDoesExist = false,
                SqlUsername = "",
                SqlPassword = "",
                CurrentWindowsUser = WindowsIdentity.GetCurrent().Name,
                AdminLanguageCode = ConfigurationManager.AppSettings["AdminLanguageCode"],
                IsChangeAdminLanguageCode = false
            };

            ViewData.IsFormVisible(true);

            return View(backEndInstallation);
        }

        [HttpPost]
        public ActionResult Index(string id, BackEndInstallation backEndInstallation)
        {
            if (backEndInstallation.IsChangeAdminLanguageCode)
            {
                Configuration appSettingsConfiguration = WebConfigurationManager.OpenWebConfiguration("~");
                appSettingsConfiguration.AppSettings.Settings["AdminLanguageCode"].Value = backEndInstallation.AdminLanguageCode;
                appSettingsConfiguration.Save();

                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(backEndInstallation.AdminLanguageCode);

                ModelState.Clear();

                backEndInstallation.IsChangeAdminLanguageCode = false;
            }
            else
            {
                if (ModelState.IsValidOrRefresh())
                {
                    try
                    {
                        string mainConnectionString;
                        string installationConnectionString;
                        List<string> installationScriptList = new List<string>();
                        string installationScript = string.Empty;
                        string databaseFilePath = string.Empty;
                        string sqlUsername;

                        //Initialize Sql Authentication Type
                        if (backEndInstallation.SqlAuthenticationType == FormHelper.SqlAuthenticationType.IntegratedWindowsAuthentication.ToString())
                        {
                            //IntegratedWindowsAuthentication

                            mainConnectionString = "Data Source=" + backEndInstallation.SqlServerName + "; Initial Catalog=" + backEndInstallation.DatabaseName + "; Integrated Security=SSPI;";
                            installationConnectionString = "Data Source=" + backEndInstallation.SqlServerName + "; Initial Catalog=master; Integrated Security=SSPI;";
                            sqlUsername = backEndInstallation.CurrentWindowsUser;
                        }
                        else
                        {
                            //SqlServerAccount

                            mainConnectionString = "Data Source=" + backEndInstallation.SqlServerName + "; Initial Catalog=" + backEndInstallation.DatabaseName + "; User Id=" + backEndInstallation.SqlUsername + "; Password=" + backEndInstallation.SqlPassword + "; Persist Security Info=False; MultipleActiveResultSets=True;";
                            //installationConnectionString = "Data Source=" + backEndInstallation.SqlServerName + "; Initial Catalog=" + backEndInstallation.DatabaseName + "; User Id=" + backEndInstallation.SqlUsername + "; Password=" + backEndInstallation.SqlPassword + "; Persist Security Info=False;";
                            installationConnectionString = "Data Source=" + backEndInstallation.SqlServerName + "; Initial Catalog=master; User Id=" + backEndInstallation.SqlUsername + "; Password=" + backEndInstallation.SqlPassword + "; Persist Security Info=False;";
                            sqlUsername = backEndInstallation.SqlUsername;
                        }

                        if (AdoHelper.IsDatabaseValid(mainConnectionString))
                        {
                            //Database already exists
                            if (backEndInstallation.IgnoreDbExistsWarning)
                            {
                                if (backEndInstallation.ResetDbIfDoesExist)
                                {
                                    //Resets database
                                    installationScriptList.Add("script-1-database-reset");
                                    installationScriptList.Add("script-2-tables");
                                    installationScriptList.Add("script-3-user-defined-functions");
                                    installationScriptList.Add("script-4-stored-procedures");
                                    if (backEndInstallation.SqlAuthenticationType == FormHelper.SqlAuthenticationType.SqlServerAccount.ToString())
                                    {
                                        installationScriptList.Add("script-5-permissions");
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(Resources.Strings.WarningDatabaseAlreadyExists);
                            }
                        }
                        else
                        {
                            //Database does not exist
                            if (backEndInstallation.CreateDbIfDoesNotExist)
                            {
                                //Creates database
                                installationScriptList.Add("script-1-database-create");
                                installationScriptList.Add("script-2-tables");
                                installationScriptList.Add("script-3-user-defined-functions");
                                installationScriptList.Add("script-4-stored-procedures");
                                if (backEndInstallation.SqlAuthenticationType == FormHelper.SqlAuthenticationType.SqlServerAccount.ToString())
                                {
                                    installationScriptList.Add("script-5-permissions");
                                }
                            }
                            else
                            {
                                throw new Exception(Resources.Strings.WarningDatabaseDoesNotExist);
                            }
                        }

                        if (installationScriptList.Count > 0)
                        {
                            //Reads SQL script files
                            foreach (string item in installationScriptList)
                            {
                                installationScript += System.IO.File.ReadAllText(Server.MapPath("~/App_Data/db/" + item + ".txt")) + Environment.NewLine;
                            }

                            //Retrieves databaseFilePath
                            using (AdoHelper db = new AdoHelper(installationConnectionString))
                            {
                                using (DataSet ds = db.ExecDataSet("SELECT physical_name FROM sys.database_files WHERE type = 0"))
                                {
                                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                    {
                                        databaseFilePath = ds.Tables[0].Rows[0]["physical_name"].ConvertTo<string>(string.Empty, true);
                                        databaseFilePath = databaseFilePath.Remove(databaseFilePath.LastIndexOf('\\') + 1);
                                    }
                                }
                            }

                            //Replaces tokens
                            installationScript = installationScript.Replace("{#DatabaseName#}", backEndInstallation.DatabaseName);
                            installationScript = installationScript.Replace("{#DatabaseFilePath#}", databaseFilePath);
                            installationScript = installationScript.Replace("{#SqlUsername#}", sqlUsername);

                            //Executes SQL scripts
                            using (AdoHelper db = new AdoHelper(installationConnectionString))
                            {
                                db.ConnectionContextExecuteNonQuery(installationScript);
                            }
                        }

                        //Updates MainConnectionString in Web.config
                        ConnectionStringSettings settings = new ConnectionStringSettings("MainConnectionString", mainConnectionString, "System.Data.SqlClient");
                        Configuration webConfigConfiguration = WebConfigurationManager.OpenWebConfiguration("~");
                        webConfigConfiguration.ConnectionStrings.ConnectionStrings.Add(settings);
                        webConfigConfiguration.Save();

                        ViewData.IsFormVisible(false);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Error, ex.Message + (ex.InnerException.IsNotNull() ? "<br/><br/>" + ex.InnerException.Message : ""));
                        ViewData.IsFormVisible(true);
                    }
                }
            }

            return View(backEndInstallation);
        }
    }
}