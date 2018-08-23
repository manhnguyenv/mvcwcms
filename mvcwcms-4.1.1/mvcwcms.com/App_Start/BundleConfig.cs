using System.Configuration;
using System.Web;
using System.Web.Optimization;

namespace MVCwCMS
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Backend

            bundles.Add(new ScriptBundle("~/bundles/backend-js").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap-select.js",
                        "~/Scripts/bootstrap-tokenfield.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/url.js",
                        "~/Content/shared/js/jquery.*", //This entry will catch all new jquery plugins named jquery.xxxxx.js, where xxxxx stands for the name of the new plugin. e.g. jquery.slide.js
                        "~/Content/backend/js/mvcwcms-*", //This entry will catch all new modules named mvcwcms-xxxxx.js, where xxxxx stands for the name of the new module. e.g. mvcwcms-news.js
                        "~/Content/shared/js/ays-beforeunload-shim.js"
                        ));

            bundles.Add(new StyleBundle("~/bundles/backend-css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/shared/css/font-awesome.css",
                        "~/Content/bootstrap-select.css",
                        "~/Content/bootstrap-tokenfield.css",
                        "~/Content/bootstrap-datetimepicker.css",
                        "~/Content/shared/css/jquery.*", //This entry will catch all new jquery plugins named jquery.xxxxx.css, where xxxxx stands for the name of the new plugin. e.g. jquery.slide.css
                        "~/Content/backend/css/mvcwcms-*" //This entry will catch all new modules named mvcwcms-xxxxx.css, where xxxxx stands for the name of the new module. e.g. mvcwcms-news.css
                        ));



            //Frontend

            bundles.Add(new ScriptBundle("~/bundles/frontend-js").Include(
                        "~/Scripts/jquery.unobtrusive*",
                         "~/Scripts/jquery.validate*",
                         "~/Scripts/bootstrap.js",
                         "~/Scripts/moment-with-locales.js",
                         "~/Scripts/bootstrap-select.js",
                         "~/Scripts/bootstrap-tokenfield.js",
                         "~/Scripts/bootstrap-datetimepicker.js",
                         "~/Scripts/url.js",
                         "~/Content/shared/js/jquery.*", //This entry will catch all new jquery plugins named jquery.xxxxx.js, where xxxxx stands for the name of the new plugin. e.g. jquery.slide.js
                        "~/Content/frontend/js/mvcwcms-*", //This entry will catch all new modules named mvcwcms-xxxxx.js, where xxxxx stands for the name of the new module. e.g. mvcwcms-news.js
                         "~/Content/shared/js/ays-beforeunload-shim.js"
                         ));

            bundles.Add(new StyleBundle("~/bundles/frontend-css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/shared/css/font-awesome.css",
                        "~/Content/bootstrap-select.css",
                        "~/Content/bootstrap-tokenfield.css",
                        "~/Content/bootstrap-datetimepicker.css",
                        "~/Content/frontend/css/jquery.*", //This entry will catch all new jquery plugins named jquery.xxxxx.css, where xxxxx stands for the name of the new plugin. e.g. jquery.slide.css
                        "~/Content/frontend/css/mvcwcms-*" //This entry will catch all new modules named mvcwcms-xxxxx.css, where xxxxx stands for the name of the new module. e.g. mvcwcms-news.css
                        ));

            
            //Enables optimization only in Release mode
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}