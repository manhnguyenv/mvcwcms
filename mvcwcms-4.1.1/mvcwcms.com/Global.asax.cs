using MVCwCMS.App_Start;
using MVCwCMS.Data;
using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVCwCMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private CultureInfo previousCultureInfo;

        private static bool IsApplicationStart;

        protected void Application_Error()
        {
            var ex = Server.GetLastError();

            Debug.WriteLine(ex.ToString());
        }

        protected void Application_Start()
        {
            //Init database
            System.Data.Entity.Database.SetInitializer(new StoreSeedData());

            //Prevents information leakage from the X-AspNetMvc-Version header of the HTTP response
            MvcHandler.DisableMvcResponseHeader = true;

            //Adds Installation route
            RouteTable.Routes.MapRoute(
                name: "Installation",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Installation", action = "Index", id = UrlParameter.Optional }
            );

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //I did not apply the following fix as I've decided to use a String data type rather then a DateTime
            //Fixes issue where MVC DateTime is bound with incorrect date format http://stackoverflow.com/a/17651519/496676
            //ModelBinders.Binders.Add(typeof(DateTime), new CustomDateBinder());
            //ModelBinders.Binders.Add(typeof(DateTime?), new NullableCustomDateBinder());

            //Registers the custom attributes with the relative validation providers http://forums.asp.net/t/1528277.aspx
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsDomainName), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsRange), typeof(RangeAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsNotEqualTo), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyDecimal), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyIntegers), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyLetters), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyLettersNumbers), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyNonAccentedLettersNumbersDashes), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyNonAccentedLettersNumbersDashesSpaces), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlyNumbers), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsOnlySafeCharacters), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsRequired), typeof(RequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsStringLengthMax), typeof(StringLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsStringLengthMin), typeof(MinLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsStringLengthRange), typeof(StringLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DataAnnotationsURL), typeof(RegularExpressionAttributeAdapter));

            //Autofac and Automapper configurations
            Bootstrapper.Run();

            IsApplicationStart = true;
        }

        protected void Session_Start()
        {
            if (IsApplicationStart)
            {
                if (ConfigurationManager.ConnectionStrings["MainConnectionString"].IsNotNull())
                {
                    AdoHelper.ConnectionString = ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString;

                    GlobalConfigurations.RegisterEmailHelper();

                    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

                    //Removes Installation route
                    if (RouteTable.Routes["Installation"].IsNotNull())
                    {
                        RouteTable.Routes.Remove(RouteTable.Routes["Installation"]);
                    }

                    RouteConfig.RegisterRoutes(RouteTable.Routes);
                }

                IsApplicationStart = false;

                //After RouteConfig.RegisterRoutes was moved to Session_Start was introduced the following bug:
                //  - If the first page hit on Application start was different from the home page a 404 error was raised.
                //To avoid the above issue it is necessary to restart the session using the following code.
                Session.Abandon();
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void Application_BeginRequest()
        {
            //Save the CultureInfo so it can be restored once the page request is completed
            previousCultureInfo = Thread.CurrentThread.CurrentUICulture; //Thread.CurrentThread.CurrentCulture;

            //Set the preferred CultureInfo as defined in the AppSettings
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ConfigurationManager.AppSettings["AdminLanguageCode"]);
            //Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
        }

        protected void Application_EndRequest()
        {
            //Restore the previous CultureInfo
            Thread.CurrentThread.CurrentUICulture = previousCultureInfo;
        }
    }
}