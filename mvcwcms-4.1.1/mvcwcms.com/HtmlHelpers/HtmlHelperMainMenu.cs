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
    public static class HtmlHelperMainMenu
    {
        public static IHtmlString GetMainMenu(this HtmlHelper htmlHelper, FrontEndCmsPage model, string className = "nav")
        {
            string mainMenu = string.Empty;
            BuildMainMenu(new CmsPages().GetAllPages(), 1, null, ref mainMenu, string.Empty, className, model.LanguageCode);
            return htmlHelper.Raw(mainMenu);
        }
        private static void BuildMainMenu(List<CmsPage> backEndCmsPageList, int currentLevel, int? pageParentId, ref string mainMenu, string savedNode, string className, string langCode)
        {
            if (backEndCmsPageList != null && currentLevel <= 2)
            {
                List<CmsPage> pagesObj = (from page in backEndCmsPageList
                                                 where page.PageParentId == pageParentId
                                                 && page.ShowInMainMenu
                                                 && page.IsActive
                                                 select page).ToList();

                if (pagesObj != null && pagesObj.Count() > 0)
                {
                    mainMenu += Environment.NewLine + "<ul";
                    if (currentLevel == 1)
                        mainMenu += " class=\"" + className + "\"";
                    else
                        mainMenu += " class=\"dropdown-menu\"";
                    mainMenu += ">" + Environment.NewLine;

                    if (savedNode.IsNotEmptyOrWhiteSpace())
                    {
                        mainMenu += savedNode + "<li class=\"divider\"></li>";
                    }

                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
                    PagesLanguages backEndCmsPagesLanguages = new PagesLanguages();
                    PageLanguage backEndCmsPageLanguage;
                    string pageTitle;
                    foreach (CmsPage pageObj in pagesObj)
                    {
                        backEndCmsPageLanguage = backEndCmsPagesLanguages.GetPageLanguage(pageObj.PageId, langCode);

                        if (backEndCmsPageLanguage.IsNotNull() && backEndCmsPageLanguage.MenuName.IsNotEmptyOrWhiteSpace())
                            pageTitle = backEndCmsPageLanguage.MenuName;
                        else
                            pageTitle = pageObj.PageName;

                        bool hasChildren = (from page in backEndCmsPageList
                                            where page.PageParentId == pageObj.PageId
                                            && page.ShowInMainMenu
                                            && page.IsActive
                                            select page).Count() > 0;

                        mainMenu += "<li";
                        if (hasChildren && currentLevel < 2)
                        {
                            savedNode = "<li><a href=\"/";
                            if (!pageObj.IsHomePage || langCode != globalConfiguration.DefaultLanguageCode)
                            {
                                if (pageObj.IsHomePage)
                                    savedNode += langCode + "/";
                                else
                                    savedNode += langCode + "/" + pageObj.FullSegment + "/";
                            }
                            savedNode += "\" title=\"" + pageTitle + "\" target=\"" + pageObj.Target + "\">" + pageTitle + "</a></li>";

                            mainMenu += " class=\"dropdown\"";
                        }
                        mainMenu += ">";

                        if (hasChildren && currentLevel < 2)
                        {
                            mainMenu += "<a href=\"#\" title=\"" + pageTitle + "\" target=\"" + pageObj.Target + "\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + pageTitle + " <b class=\"caret\"></b></a>";
                        }
                        else
                        {
                            mainMenu += "<a href=\"{$url}\" title=\"" + pageTitle + "\" target=\"" + pageObj.Target + "\">" + pageTitle + "</a>";
                        }

                        if (pageObj.PageTemplateId.IsNull())
                        {
                            mainMenu = mainMenu.Replace("{$url}", pageObj.Url);
                        }
                        else
                        {
                            if (!pageObj.IsHomePage || langCode != globalConfiguration.DefaultLanguageCode)
                            {
                                if (pageObj.IsHomePage)
                                    mainMenu = mainMenu.Replace("{$url}", "/" + langCode + "/");
                                else
                                    mainMenu = mainMenu.Replace("{$url}", "/" + langCode + "/" + pageObj.FullSegment + "/");
                            }
                            else
                            {
                                mainMenu = mainMenu.Replace("{$url}", "/");
                            }
                        }

                        currentLevel++;

                        BuildMainMenu(backEndCmsPageList, currentLevel, pageObj.PageId, ref mainMenu, savedNode, className, langCode);

                        currentLevel--;

                        mainMenu += "</li>" + Environment.NewLine;
                    }
                    mainMenu += "</ul>" + Environment.NewLine;
                }
            }
        }
    }
}
