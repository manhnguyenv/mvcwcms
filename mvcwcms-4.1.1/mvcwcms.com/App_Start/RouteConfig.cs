using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Canonicalize;

namespace MVCwCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            if (globalConfiguration.IsCanonicalizeActive)
            {
                if (globalConfiguration.HostNameLabel == "www.")
                    routes.Canonicalize().Www();
                else
                    routes.Canonicalize().NoWww();
            }

            //Introduced to support the FrontEndCmsPage.Parameter property in DefaultController
            routes.Canonicalize().TrailingSlash();

            routes.MapRoute(
                name: "Sitemap",
                url: "sitemap.xml",
                defaults: new { controller = "Sitemap", action = "Index" }
            );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{*AnyAppleTouchIconInTheRoot}", new { AnyAppleTouchIconInTheRoot = @"apple-touch-icon(.*)\.png" });
            routes.IgnoreRoute("{*AllPngFiles}", new { AllPngFiles = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*AllJpgFiles}", new { AllJpgFiles = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*AllGifFiles}", new { AllGifFiles = @".*\.gif(/.*)?" });
            routes.IgnoreRoute("{*AllPhpFiles}", new { AllPhpFiles = @".*\.php(/.*)?" });
            routes.IgnoreRoute("{*AllXmlFiles}", new { AllXmlFiles = @".*\.xml(/.*)?" });
            routes.IgnoreRoute("{*AllHtmFiles}", new { AllHtmFiles = @".*\.htm(/.*)?" });
            routes.IgnoreRoute("{*AllHtmlFiles}", new { AllHtmlFiles = @".*\.html(/.*)?" });
            routes.IgnoreRoute("{*AllCssFiles}", new { AllCssFiles = @".*\.css(/.*)?" });
            routes.IgnoreRoute("{*AllBsFiles}", new { AllBsFiles = @".*\.bs(/.*)?" });
            routes.IgnoreRoute("{*AllJsFiles}", new { AllBsFiles = @".*\.js(/.*)?" });
            routes.IgnoreRoute("{*AllTxtFiles}", new { AllTxtFiles = @".*\.txt(/.*)?" });

            routes.MapRoute(
                name: "Admin",
                url: "Admin/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );

            //Used to route FrontEnd child action modules
            routes.MapRoute(
                "FrontEnd",
                "{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                ControllerHelper.GetRoutesConstraints("FrontEnd")
            );

            //Used to route custom content type (E.g. xml, json, etc..). A clear example is the OutputNewsController.
            routes.MapRoute(
                "Output",
                "{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                ControllerHelper.GetRoutesConstraints("Output")
            );

            routes.MapRoute(
                "Default",
                "{languageCode}/{*segments}",
                new { controller = "Default", action = "Index", languageCode = globalConfiguration.DefaultLanguageCode, segments = UrlParameter.Optional }
            );
        }
    }
}
