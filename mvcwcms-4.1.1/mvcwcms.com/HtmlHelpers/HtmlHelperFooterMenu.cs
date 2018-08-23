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
    public static class HtmlHelperFooterMenu
    {
        public static IHtmlString GetFooterMenu(this HtmlHelper htmlHelper, FrontEndCmsPage model, string className = "nav nav-pills")
        {
            string footerMenu = string.Empty;
            List<CmsPage> pagesObj = new CmsPages().GetAllPages();
            if (pagesObj != null)
            {
                pagesObj = (from page in pagesObj
                            where page.ShowInBottomMenu && page.IsActive
                            select page).ToList();
                if (pagesObj != null && pagesObj.Count > 0)
                {
                    footerMenu += "<ul class=\"" + className + "\">";
                    string url;
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
                    PagesLanguages backEndCmsPagesContent = new PagesLanguages();
                    PageLanguage backEndCmsPageLanguage;
                    string pageTitle = string.Empty;
                    foreach (CmsPage pageObj in pagesObj)
                    {
                        backEndCmsPageLanguage = backEndCmsPagesContent.GetPageLanguage(pageObj.PageId, model.LanguageCode);

                        if (backEndCmsPageLanguage.IsNotNull() && backEndCmsPageLanguage.MenuName.IsNotEmptyOrWhiteSpace())
                            pageTitle = backEndCmsPageLanguage.MenuName;
                        else
                            pageTitle = pageObj.PageName;

                        if (pageObj.PageTemplateId.IsNull())
                            url = pageObj.Url;
                        else
                        {
                            url = "/";
                            if (!pageObj.IsHomePage || model.LanguageCode != globalConfiguration.DefaultLanguageCode)
                            {
                                if (pageObj.IsHomePage)
                                    url += model.LanguageCode + "/";
                                else
                                    url += model.LanguageCode + "/" + pageObj.FullSegment + "/";
                            }
                        }
                        footerMenu += "<li><a href=\"" + url + "\" target=\"" + pageObj.Target + "\" title=\"" + pageTitle + "\">" + pageTitle + "</a></li>";
                    }
                    footerMenu += "</ul>";
                }
            }
            return htmlHelper.Raw(footerMenu);
        }
    }
}
