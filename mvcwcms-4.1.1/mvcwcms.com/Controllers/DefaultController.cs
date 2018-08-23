using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using MVCwCMS.Helpers;

namespace MVCwCMS.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index(string languageCode, string segments)
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            if (!globalConfiguration.IsOffline || Request["OfflineCode"] == globalConfiguration.OfflineCode || Session["OfflineCode"].ConvertTo<string>(null, true) == globalConfiguration.OfflineCode)
            {
                if (Session["OfflineCode"].IsNull())
                    Session["OfflineCode"] = Request["OfflineCode"];

                FrontEndCmsPage page = new FrontEndCmsPage()
                {
                    PageId = null,
                    LanguageCode = globalConfiguration.DefaultLanguageCode,
                    LanguageFolder = "",
                    Parameter = string.Empty,
                    MetaTagTitle = globalConfiguration.MetaTitle,
                    MetaTagKeywords = globalConfiguration.MetaKeywords,
                    MetaTagDescription = globalConfiguration.MetaDescription,
                    Robots = globalConfiguration.Robots,
                    PageTemplateId = null,
                    StatusCode = null
                };

                CmsPages cmsPages = new CmsPages();
                CmsPage cmsPage = cmsPages.GetHomePage();
                bool isHomePageDefined = cmsPage.IsNotNull();

                if (segments.IsNotEmptyOrWhiteSpace())
                {
                    segments = segments.TrimEnd('/');

                    cmsPage = cmsPages.GetPageBySegments(segments);
                    if (cmsPage.IsNull() && segments.Contains('/'))
                    {
                        cmsPage = cmsPages.GetPageBySegments(segments.Remove(segments.LastIndexOf('/')));
                        page.Parameter = segments.Substring(segments.LastIndexOf('/') + 1); //the parameter can be a dash separated list of parameters. E.g. 2-today_the_weather_is_nice
                    }
                }

                Language language = new Languages().GetLanguageByCode(languageCode);

                if (
                    cmsPage.IsNotNull() && //The page exists
                    (cmsPage.IsActive || (Request["preview"] == "true" && BackEndSessions.CurrentUser.IsNotNull())) && //The page is active or the page is in preview mode with a user logged in the admin area
                    language.IsNotNull() && //The language exists
                    language.IsActive //The language is active
                    )
                {
                    if (cmsPage.IsAccessRestricted && FrontEndSessions.CurrentSubscription.IsNull())
                    {
                        SubscriptionConfiguration subscriptionConfiguration = new SubscriptionConfigurations().GetSubscriptionConfiguration();
                        CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.SignInPageId, language.LanguageCode);
                        if (cmsPageActionlink.IsNotNull())
                            return Redirect(cmsPageActionlink.Url + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsoluteUri));
                    }

                    page.PageId = cmsPage.PageId;
                    page.LanguageCode = language.LanguageCode;
                    page.LanguageFolder = language.LanguageCode == globalConfiguration.DefaultLanguageCode ? "" : language.LanguageCode;
                    page.PageTemplateId = cmsPage.PageTemplateId;

                    PageLanguage pageLanguage = new PagesLanguages().GetPageLanguage(cmsPage.PageId, language.LanguageCode);
                    if (pageLanguage.IsNotNull())
                    {
                        if (pageLanguage.MetaTagTitle.IsNotEmptyOrWhiteSpace())
                            page.MetaTagTitle = pageLanguage.MetaTagTitle.ReplaceGlobalTokens();

                        if (pageLanguage.MetaTagKeywords.IsNotEmptyOrWhiteSpace())
                            page.MetaTagKeywords = pageLanguage.MetaTagKeywords.ReplaceGlobalTokens();

                        if (pageLanguage.MetaTagDescription.IsNotEmptyOrWhiteSpace())
                            page.MetaTagDescription = pageLanguage.MetaTagDescription.ReplaceGlobalTokens();

                        if (pageLanguage.Robots.IsNotEmptyOrWhiteSpace())
                            page.Robots = pageLanguage.Robots;
                    }
                }
                else
                {
                    if (language.IsNotNull())
                    {
                        page.LanguageCode = language.LanguageCode;
                        page.LanguageFolder = language.LanguageCode == globalConfiguration.DefaultLanguageCode ? "" : language.LanguageCode;
                    }

                    page.PageTemplateId = globalConfiguration.DefaultErrorPageTemplateId;
                    if (isHomePageDefined)
                        page.StatusCode = 404;
                    else
                        page.StatusCode = 501; //Home page not defined in the database table tb_cms_pages
                }

                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture((language.IsNotNull() && language.IsActive) ? language.LanguageCode : page.LanguageCode);
                
                return View(page);
            }
            else
            {
                return Redirect("~/_app_offline.htm");
            }
        }
    }
}