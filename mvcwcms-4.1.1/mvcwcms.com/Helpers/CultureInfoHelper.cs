using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace MVCwCMS.Helpers
{
    public class CultureInfoHelper
    {
        public static void ForceBackEndLanguage()
        {
            //Save the CultureInfo so it can be restored once the page request is completed
            HttpContext.Current.Session["previousCultureInfo"] = Thread.CurrentThread.CurrentUICulture;

            //Set the preferred CultureInfo as defined in the AppSettings
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ConfigurationManager.AppSettings["AdminLanguageCode"]);
        }

        public static void RestoreFrontEndLanguage()
        {
            if (HttpContext.Current.Session["previousCultureInfo"] != null)
            {
                //Restore the previous CultureInfo
                Thread.CurrentThread.CurrentUICulture = HttpContext.Current.Session["previousCultureInfo"] as CultureInfo;
            }
        }
    }
}