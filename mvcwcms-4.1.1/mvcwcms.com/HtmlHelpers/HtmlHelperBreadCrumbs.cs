using MVCwCMS;
using MVCwCMS.Models;
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
    public static class HtmlHelperBreadCrumbs
    {
        public static IHtmlString GetBreadCrumbs(this HtmlHelper htmlHelper, FrontEndCmsPage model, string className = "breadcrumb", string label = "", string separator = "/")
        {
            string breadCrumbs = string.Empty;
            CmsPages pages = new CmsPages();
            PagesLanguages pagesLanguages = new PagesLanguages();
            BuildBreadCrumbs(pages, pagesLanguages, ref breadCrumbs, model.PageId, model.LanguageCode, separator, false);
            if (breadCrumbs.IsNotEmptyOrWhiteSpace())
            {
                if (label.IsNotEmptyOrWhiteSpace())
                {
                    label = "<span>" + label + "</span>";
                }
                breadCrumbs = "<ul class=\"" + className + "\">" + breadCrumbs.Insert(breadCrumbs.IndexOf('>') + 1, label) + "</ul>";
            }
            return htmlHelper.Raw(breadCrumbs);
        }

        private static void BuildBreadCrumbs(CmsPages pages, PagesLanguages pageLanguages, ref string breadCrumbs, int? pageId, string langCode, string separator, bool isPreviousHomePage)
        {
            CmsPage page = pages.GetPageByPageId(pageId);
            if (page.IsNotNull())
            {
                PageLanguage pageLanguage = pageLanguages.GetPageLanguage(pageId, langCode);
                string title = page.PageName;
                if (pageLanguage.IsNotNull() && pageLanguage.MenuName.IsNotEmptyOrWhiteSpace())
                    title = pageLanguage.MenuName;

                if (breadCrumbs.IsEmptyOrWhiteSpace())
                {
                    breadCrumbs = "<li class=\"active\">" + title + "</li>";
                }
                else
                {
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
                    string url = "/";
                    if (!page.IsHomePage || langCode != globalConfiguration.DefaultLanguageCode)
                    {
                        if (page.IsHomePage)
                            url += langCode + "/";
                        else
                            url += langCode + "/" + page.FullSegment + "/";
                    }
                    breadCrumbs = "<li><a href=\"" + url + "\">" + title + "</a></li>" + breadCrumbs;
                }
                BuildBreadCrumbs(pages, pageLanguages, ref breadCrumbs, page.PageParentId, langCode, separator, page.IsHomePage);
            }
            else if (page.IsNull() && !isPreviousHomePage)
            {
                page = pages.GetHomePage();
                if (page.IsNotNull())
                    BuildBreadCrumbs(pages, pageLanguages, ref breadCrumbs, page.PageId, langCode, separator, page.IsHomePage);
            }
        }
    }
}
