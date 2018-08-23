using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Web;
using MVCwCMS.Helpers;
using System.Web.Script.Serialization;
using MVCwCMS.Filters;
using System.Linq;
using System.Web.Routing;
using System.Configuration;
using System.Web.Configuration;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/
        [IsRestricted]
        public ActionResult Index()
        {
            return View();
        }

        // /Admin/ClearCache/
        [IsRestricted]
        public ActionResult ClearCache()
        {
            HttpRuntime.UnloadAppDomain();

            return RedirectToAction("Index");
        }

        // /Admin/Uninstall/
        [IsRestricted]
        public ActionResult UninstallCms()
        {
            Configuration webConfigConfiguration = WebConfigurationManager.OpenWebConfiguration("~");
            webConfigConfiguration.ConnectionStrings.ConnectionStrings.Remove("MainConnectionString");
            webConfigConfiguration.Save();

            return Redirect("~/");
        }

        //  /Admin/Login/
        [HttpGet]
        [IsRestricted]
        public ActionResult Login(string ReturnUrl)
        {
            AdminPage backEndPage = new AdminPage();
            backEndPage.PageName = "Login";
            ViewBag.AdminPage = backEndPage;

            BackEndLogin backEndLogin = new BackEndLogin()
            {
                ReturnUrl = ReturnUrl
            };

            return View(backEndLogin);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult Login(BackEndLogin backEndLogin, string ReturnUrl)
        {
            AdminPage backEndPage = new AdminPage();
            backEndPage.PageName = "Login";
            ViewBag.AdminPage = backEndPage;

            if (ModelState.IsValidOrRefresh())
            {
                Users users = new Users();
                User user = users.GetUserByUserNameAndPassword(backEndLogin.Username, backEndLogin.Password);
                if (user.IsNotNull())
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.SuccessfullyLoggedIn);

                    BackEndSessions.CurrentUser = user;

                    AdminPages backEndPages = new AdminPages();
                    BackEndSessions.CurrentMenu = backEndPages.GetMenuByGroupId(user.GroupId);

                    if (ReturnUrl.IsNotEmptyOrWhiteSpace())
                    {
                        return Redirect(HttpUtility.UrlDecode(ReturnUrl));
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UsernameOrPasswordNotValid);
                }
            }

            return View(backEndLogin);
        }

        //  /Admin/Logout/
        [IsRestricted]
        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Index");
        }

        //  /Admin/ChangePassword/
        [HttpGet]
        [IsRestricted]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(BackEndChangePassword backEndChangePassword)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Users users = new Users();
                int? result = users.ChangePassword(BackEndSessions.CurrentUser.UserName, BackEndSessions.CurrentUser.Salt, backEndChangePassword.CurrentPassword, backEndChangePassword.NewPassword);
                switch (result)
                {
                    case 0:
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.PasswordSuccessfullyChanged);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.CurrentPasswordNotValid);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }

            }

            return View(backEndChangePassword);
        }

        //  /Admin/GetCmsModules/
        public ActionResult GetCmsModules()
        {
            return Json(ExtensionsHelper.GetCmsModules(false), JsonRequestBehavior.AllowGet);
        }

        //  /Admin/GetContentTemplates/
        public ActionResult GetContentTemplates()
        {
            ContentTemplates contentTemplates = new ContentTemplates();
            return Json(contentTemplates.GetAllContentTemplates(isActive: true), JsonRequestBehavior.AllowGet);
        }

        //  /Admin/IsSessionActive/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IsSessionActive()
        {
            return Content(BackEndSessions.CurrentUser.IsNotNull().ToString(), "text/plain");
        }

        //  /Admin/IsPageBrowseAuthorized/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IsPageBrowseAuthorized(string id)
        {
            AdminPages backEndPages = new AdminPages();
            AdminPage backEndPage = backEndPages.GetPageByAction(id);
            return Content(backEndPages.IsPermissionGranted(backEndPage.PageId, PermissionCode.Browse).ToString(), "text/plain");
        }

        //  /Admin/GetUniqueKey/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUniqueKey()
        {
            return Content(DateTime.Now.Ticks.ToBase36(), "text/plain");
        }

        //  /Admin/GlobalConfiguration/
        [HttpGet]
        [IsRestricted]
        public ActionResult GlobalConfiguration()
        {
            BackEndGlobalConfigurationEdit backEndGlobalConfigurationEdit = new BackEndGlobalConfigurationEdit();

            GlobalConfigurations backEndGlobalConfigurations = new GlobalConfigurations();
            GlobalConfiguration globalConfiguration = backEndGlobalConfigurations.GetGlobalConfiguration();
            if (globalConfiguration.IsNotNull())
            {
                backEndGlobalConfigurationEdit.SiteName = globalConfiguration.SiteName;
                backEndGlobalConfigurationEdit.MetaTitle = globalConfiguration.MetaTitle;
                backEndGlobalConfigurationEdit.MetaKeywords = globalConfiguration.MetaKeywords;
                backEndGlobalConfigurationEdit.MetaDescription = globalConfiguration.MetaDescription;
                backEndGlobalConfigurationEdit.Robots = globalConfiguration.Robots;
                backEndGlobalConfigurationEdit.NotificationEmail = globalConfiguration.NotificationEmail;
                backEndGlobalConfigurationEdit.IsCanonicalizeActive = globalConfiguration.IsCanonicalizeActive;
                backEndGlobalConfigurationEdit.HostNameLabel = globalConfiguration.HostNameLabel;
                backEndGlobalConfigurationEdit.DomainName = globalConfiguration.DomainName;
                backEndGlobalConfigurationEdit.BingVerificationCode = globalConfiguration.BingVerificationCode;
                backEndGlobalConfigurationEdit.GoogleVerificationCode = globalConfiguration.GoogleVerificationCode;
                backEndGlobalConfigurationEdit.GoogleAnalyticsTrackingCode = globalConfiguration.GoogleAnalyticsTrackingCode;
                backEndGlobalConfigurationEdit.GoogleSearchCode = globalConfiguration.GoogleSearchCode;
                backEndGlobalConfigurationEdit.IsOffline = globalConfiguration.IsOffline;
                backEndGlobalConfigurationEdit.OfflineCode = globalConfiguration.OfflineCode;
                backEndGlobalConfigurationEdit.ServerTimeZone = globalConfiguration.ServerTimeZone;
                backEndGlobalConfigurationEdit.DateFormat = globalConfiguration.DateFormat;
                backEndGlobalConfigurationEdit.TimeFormat = globalConfiguration.TimeFormat;
                backEndGlobalConfigurationEdit.DefaultLanguageCode = globalConfiguration.DefaultLanguageCode;
                backEndGlobalConfigurationEdit.DefaultErrorPageTemplateId = globalConfiguration.DefaultErrorPageTemplateId;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndGlobalConfigurationEdit);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult GlobalConfiguration(BackEndGlobalConfigurationEdit backEndGlobalConfigurationEdit)
        {
            GlobalConfigurations backEndGlobalConfigurations = new GlobalConfigurations();
            GlobalConfiguration globalConfiguration = backEndGlobalConfigurations.GetGlobalConfiguration();
            int? result = backEndGlobalConfigurations.Edit(
                backEndGlobalConfigurationEdit.SiteName,
                backEndGlobalConfigurationEdit.MetaTitle,
                backEndGlobalConfigurationEdit.MetaKeywords,
                backEndGlobalConfigurationEdit.MetaDescription,
                backEndGlobalConfigurationEdit.Robots,
                backEndGlobalConfigurationEdit.NotificationEmail,
                backEndGlobalConfigurationEdit.IsCanonicalizeActive,
                backEndGlobalConfigurationEdit.HostNameLabel,
                backEndGlobalConfigurationEdit.DomainName,
                backEndGlobalConfigurationEdit.BingVerificationCode,
                backEndGlobalConfigurationEdit.GoogleVerificationCode,
                backEndGlobalConfigurationEdit.GoogleAnalyticsTrackingCode,
                backEndGlobalConfigurationEdit.GoogleSearchCode,
                backEndGlobalConfigurationEdit.IsOffline,
                backEndGlobalConfigurationEdit.OfflineCode,
                backEndGlobalConfigurationEdit.ServerTimeZone,
                backEndGlobalConfigurationEdit.DateFormat,
                backEndGlobalConfigurationEdit.TimeFormat,
                backEndGlobalConfigurationEdit.DefaultLanguageCode,
                backEndGlobalConfigurationEdit.DefaultErrorPageTemplateId);
            switch (result)
            {
                case 0:
                    if (globalConfiguration.HostNameLabel != backEndGlobalConfigurationEdit.HostNameLabel)
                    {
                        //Updates RouteTable
                        using (RouteTable.Routes.GetWriteLock())
                        {
                            RouteTable.Routes.Clear();
                            RouteConfig.RegisterRoutes(RouteTable.Routes);
                        }
                    }

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    break;
                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    ViewData.IsFormVisible(false);
                    break;
                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return View(backEndGlobalConfigurationEdit);
        }

        //  /Admin/ErrorPage/
        public ActionResult ErrorPage(string errorPage, string errorMessage)
        {
            AdminPage backEndPage = new AdminPage();
            backEndPage.PageName = Resources.Strings.ErrorOccurred;
            ViewBag.AdminPage = backEndPage;

            ViewBag.ErrorPage = errorPage;
            ViewBag.ErrorMessage = errorMessage;

            return View();
        }
    }
}