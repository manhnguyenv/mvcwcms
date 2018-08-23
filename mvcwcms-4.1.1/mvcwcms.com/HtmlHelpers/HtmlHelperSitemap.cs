using MVCwCMS;
using MVCwCMS.Models;
using MVCwCMS.Helpers;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCwCMS.HtmlHelpers
{
    public static class HtmlHelperSitemap
    {
        public static IHtmlString GetSitemap(this HtmlHelper htmlHelper, FrontEndCmsPage model, string className = "")
        {
            string sitemap = string.Empty;
            PagesLanguages backEndCmsPagesLanguages = new PagesLanguages();
            BuildSitemap(new CmsPages().GetAllPages(), backEndCmsPagesLanguages, 1, null, ref sitemap, className, model.LanguageCode);
            return htmlHelper.Raw(sitemap);
        }
        private static void BuildSitemap(List<CmsPage> backEndCmsPageList, PagesLanguages pagesLanguages, int currentLevel, int? pageParentId, ref string sitemap, string className, string langCode)
        {
            if (backEndCmsPageList != null)
            {
                List<CmsPage> pagesObj = (from page in backEndCmsPageList
                                                 where page.PageParentId == pageParentId
                                                 && page.ShowInSitemap
                                                 && page.IsActive
                                                 select page).ToList();

                if (pagesObj != null && pagesObj.Count() > 0)
                {
                    sitemap += Environment.NewLine + "<ul";
                    if (currentLevel == 1 && className.IsNotEmptyOrWhiteSpace())
                        sitemap += " class=\"" + className + "\"";
                    sitemap += ">" + Environment.NewLine;

                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                    PageLanguage backEndCmsPageLanguage;
                    string pageTitle;
                    string url;
                    foreach (CmsPage pageObj in pagesObj)
                    {
                        backEndCmsPageLanguage = pagesLanguages.GetPageLanguage(pageObj.PageId, langCode);

                        pageTitle = pageObj.PageName;
                        if (backEndCmsPageLanguage.IsNotNull() && backEndCmsPageLanguage.MenuName.IsNotEmptyOrWhiteSpace())
                            pageTitle = backEndCmsPageLanguage.MenuName;

                        if (pageObj.PageTemplateId.IsNull())
                        {
                            url = pageObj.Url;
                        }
                        else
                        {
                            if (!pageObj.IsHomePage || langCode != globalConfiguration.DefaultLanguageCode)
                            {
                                if (pageObj.IsHomePage)
                                    url = "/" + langCode + "/";
                                else
                                    url = "/" + langCode + "/" + pageObj.FullSegment + "/";
                            }
                            else
                            {
                                url = "/";
                            }
                        }

                        sitemap += "<li><a href=\"" + url + "\" title=\"" + pageTitle + "\" target=\"" + pageObj.Target + "\">" + pageTitle + "</a>";

                        currentLevel++;

                        BuildSitemap(backEndCmsPageList, pagesLanguages, currentLevel, pageObj.PageId, ref sitemap, className, langCode);

                        currentLevel--;

                        sitemap += "</li>" + Environment.NewLine;
                    }
                    sitemap += "</ul>" + Environment.NewLine;
                }
            }
        }
    }
}
