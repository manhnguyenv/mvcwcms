using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace MVCwCMS.Models
{
    [Serializable]
    public class CmsPage
    {
        public int? PageId { get; set; }
        public int? PageParentId { get; set; }
        public string Segment { get; set; }
        public string FullSegment { get; set; }
        public string PageName { get; set; }
        public string Target { get; set; }
        public int? PageTemplateId { get; set; }
        public bool IsTemplateActive { get; set; }
        public string Url { get; set; }
        public int? Ordering { get; set; }
        public bool ShowInMainMenu { get; set; }
        public bool ShowInBottomMenu { get; set; }
        public bool ShowInSitemap { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccessRestricted { get; set; }
        public bool IsHomePage { get; set; }
    }

    public class CmsPageActionlink
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }

    public class CmsPages
    {
        private List<CmsPage> _AllItems;

        private List<CmsPage> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<CmsPage>("sp_cms_pages_select", force);
        }

        public CmsPages()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<CmsPage> GetAllPages(bool showHierarchicalPageNames = false, int? excludedPageIdBranch = null)
        {
            if (showHierarchicalPageNames)
            {
                List<CmsPage> pageList = new List<CmsPage>();
                GetHierarchicalTitles(_AllItems.Clone(), ref pageList, null, string.Empty, excludedPageIdBranch); //Clone requires the class has [Serializable] attribute
                return pageList;
            }
            else
            {
                return _AllItems;
            }
        }
        private void GetHierarchicalTitles(List<CmsPage> originalPageList, ref List<CmsPage> pageList, int? currentPageIdParent, string levelChars, int? excludedPageIdBranch)
        {
            List<CmsPage> pages = (from p in originalPageList
                                   where p.PageParentId == currentPageIdParent
                                   orderby p.Ordering
                                   select p).ToList();
            foreach (CmsPage p in pages)
            {
                if (excludedPageIdBranch.IsNull() || p.PageId != excludedPageIdBranch)
                {
                    p.PageName = levelChars + p.PageName;
                    pageList.Add(p);
                    GetHierarchicalTitles(originalPageList, ref pageList, p.PageId, levelChars + "―", excludedPageIdBranch);
                }
            }
        }

        public CmsPage GetHomePage()
        {
            CmsPage result;
            result = (from page in _AllItems
                      where page.IsHomePage
                      select page).FirstOrDefault();
            return result;
        }

        public CmsPage GetPageByPageId(int? id)
        {
            CmsPage result;
            result = (from page in _AllItems
                      where page.PageId == id
                      select page).FirstOrDefault();
            return result;
        }

        public CmsPage GetPageBySegments(string segments)
        {
            CmsPage result;
            result = (from page in _AllItems
                      where page.FullSegment.ToLower() == segments.ToLower()
                      select page).FirstOrDefault();
            return result;
        }

        public CmsPageActionlink GetCmsPageActionlink(int? pageId, string langCode)
        {
            CmsPageActionlink result = new CmsPageActionlink();

            CmsPage page = GetPageByPageId(pageId);
            if (page.IsNotNull()) {

                if (page.PageTemplateId.IsNull())
                {
                    result.Url = page.Url + "/";
                }
                else
                {
                    result.Url = "/" + langCode.ToLower() + "/" + page.FullSegment + "/";
                }

                PageLanguage pageLanguage = new PagesLanguages().GetPageLanguage(pageId, langCode);
                if (pageLanguage.IsNotNull() && pageLanguage.MenuName.IsNotEmptyOrWhiteSpace())
                {
                    result.Title = pageLanguage.MenuName;
                }
                else
                {
                    result.Title = page.PageName;
                }
            }

            return result;
        }

        public HtmlString GetTreeTablePageList(string pageName = null)
        {
            StringBuilder treeTablePageList = new StringBuilder();
            BuildTreeTablePageList(_AllItems, 1, null, treeTablePageList, pageName);

            if (treeTablePageList.ToString().IsNotEmptyOrWhiteSpace())
            {
                return new HtmlString(treeTablePageList.ToString());
            }
            else
            {
                return null;
            }
        }
        private void BuildTreeTablePageList(List<CmsPage> cmsPageList, int currentLevel, int? pageParentId, StringBuilder treeTablePageList, string pageName)
        {
            if (cmsPageList != null)
            {
                List<CmsPage> pagesObj = (from page in cmsPageList
                                                 where page.PageParentId == pageParentId
                                                 select page).ToList();

                if (pagesObj != null && pagesObj.Count() > 0)
                {
                    if (currentLevel == 1)
                    {
                        treeTablePageList.Append("<div class=\"CMSPagesTreeControl btn-group\">" + Environment.NewLine +
                                                 "<a class=\"btn btn-default btn-xs\" href=\"#\"><i class=\"fa fa-compress fa-flip-horizontal\"></i> " + Resources.Strings.CollapseAll + "</a>" + Environment.NewLine +
                                                 "<a class=\"btn btn-default btn-xs\" href=\"#\"><i class=\"fa fa-expand fa-flip-horizontal\"></i> " + Resources.Strings.ExpandAll + "</a>" + Environment.NewLine +
                                                 "<a class=\"btn btn-default btn-xs\" href=\"#\"><i class=\"fa fa-arrows-alt\"></i> " + Resources.Strings.ToggleAll + "</a>" + Environment.NewLine +
                                                 "</div>");
                    }
                    treeTablePageList.Append(Environment.NewLine + "<ul");
                    if (currentLevel == 1)
                        treeTablePageList.Append(" class=\"CMSPagesTree\" ");
                    treeTablePageList.Append(">" + Environment.NewLine);
                    if (currentLevel == 1)
                    {
                        treeTablePageList.Append("<li><table class=\"table table-condensed table-bordered table-hover\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><thead><tr class=\"webgrid-header\">" +
                                                 "<th class=\"\">" + Resources.Strings.PageName + "</th>" +
                                                 "<th class=\"col-180\">" + Resources.Strings.Actions + "</th>" +
                                                 "<th class=\"col-60\">" + Resources.Strings.Active + "</th>" +
                                                 "<th class=\"col-90\">" + Resources.Strings.MainMenu + "</th>" +
                                                 "<th class=\"col-90\">" + Resources.Strings.FooterMenu + "</th>" +
                                                 "<th class=\"col-60\">" + Resources.Strings.Sitemap + "</th>" +
                                                 "<th class=\"col-80\">" + Resources.Strings.Restricted + "</th>" +
                                                 "<th class=\"col-80\">" + Resources.Strings.HomePage + "</th>" +
                                                 "</tr></thead></table></li>");
                    }

                    string imgYes = "<i class=\"fa fa-check\"></i>";
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
                    string highlight;

                    AdminPages adminPages = new AdminPages();
                    AdminPage adminPage = adminPages.GetPageByCurrentAction();

                    foreach (CmsPage pageObj in pagesObj)
                    {
                        if (pageName.IsNotEmptyOrWhiteSpace() && pageObj.PageName.Contains(pageName, StringComparison.OrdinalIgnoreCase))
                        {
                            highlight = "success";
                        }
                        else
                        {
                            highlight = "";
                        }

                        treeTablePageList.Append("<li><table id=\"r-" + pageObj.PageId.ToString() + "\" class=\"table table-condensed table-bordered table-hover\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                 "<thead class=\"thead-hide\">" +
                                                 "<th class=\"\">" + Resources.Strings.PageName + "</th>" +
                                                 "<th class=\"col-180\">" + Resources.Strings.Actions + "</th>" +
                                                 "<th class=\"col-60\">" + Resources.Strings.Active + "</th>" +
                                                 "<th class=\"col-90\">" + Resources.Strings.MainMenu + "</th>" +
                                                 "<th class=\"col-90\">" + Resources.Strings.FooterMenu + "</th>" +
                                                 "<th class=\"col-60\">" + Resources.Strings.Sitemap + "</th>" +
                                                 "<th class=\"col-80\">" + Resources.Strings.Restricted + "</th>" +
                                                 "<th class=\"col-80\">" + Resources.Strings.HomePage + "</th>" +
                                                 "</thead>" +
                                                 "<tbody><tr>" +
                                                 "<td class=\"" + highlight + "\">" + pageObj.PageName + "</td>" +
                                                 "<td><div class=\"btn-group\">");
                        if (pageObj.PageTemplateId.IsNotNull() && adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Add))
                        {
                            treeTablePageList.Append("<a href=\"/Admin/CmsPagesAdd/" + pageObj.PageId.ToString() + "\" class=\"btn btn-default btn-xs\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.CmsPagesAdd + "\" ><i class=\"fa fa-plus\"></i></a>");
                        }
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Edit))
                        {
                            treeTablePageList.Append("<a href=\"/Admin/CmsPagesEdit/" + pageObj.PageId.ToString() + "\" class=\"btn btn-default btn-xs\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.CmsPagesEdit + "\" ><i class=\"fa fa-pencil\"></i></a>");
                        }
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Delete))
                        {
                            treeTablePageList.Append("<button type=\"submit\" data-action=\"/Admin/CmsPagesDelete\" data-id=\"" + pageObj.PageId.ToString() + "\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.CmsPagesDelete + "\" class=\"btn btn-default btn-xs btn-admin-pages action-delete\" data-action-delete-item=\"" + pageObj.PageName + "\"><i class=\"fa fa-trash-o\"></i></button>");
                        }
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Edit))
                        {
                            treeTablePageList.Append("<button type=\"submit\" data-action=\"/Admin/CmsPagesMoveUp/#r-" + pageObj.PageId.ToString() + "\" data-id=\"" + pageObj.PageId.ToString() + "\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.MovePageUpInSameLevel + "\" class=\"btn btn-default btn-xs btn-admin-pages action-post-id\" \"><i class=\"fa fa-chevron-up\"></i></button>" +
                                                     "<button type=\"submit\" data-action=\"/Admin/CmsPagesMoveDown/#r-" + pageObj.PageId.ToString() + "\" data-id=\"" + pageObj.PageId.ToString() + "\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.MovePageDownInSameLevel + "\" class=\"btn btn-default btn-xs btn-admin-pages action-post-id\" \"><i class=\"fa fa-chevron-down\"></i></button>");
                        }
                        if (pageObj.PageTemplateId.IsNotNull() && adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Edit))
                        {
                            treeTablePageList.Append("<a href=\"/Admin/CmsPagesLanguages/" + pageObj.PageId.ToString() + "\" class=\"btn btn-default btn-xs\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.PageContent + "\" ><i class=\"fa fa-file\"></i></a>");
                        }
                        treeTablePageList.Append("<a href=\"{$url}\" class=\"btn btn-default btn-xs\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.Preview + ": " + pageObj.PageName + "\" target=\"_blank\" ><i class=\"fa fa-globe\"></i></a>" +
                                                 "</div></td>" +
                                                 "<td class=\"text-center\">" + (pageObj.IsActive.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "<td class=\"text-center\">" + (pageObj.ShowInMainMenu.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "<td class=\"text-center\">" + (pageObj.ShowInBottomMenu.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "<td class=\"text-center\">" + (pageObj.ShowInSitemap.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "<td class=\"text-center\">" + (pageObj.IsAccessRestricted.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "<td class=\"text-center\">" + (pageObj.IsHomePage.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "</tr></tbody></table>" + Environment.NewLine);
                        if (pageObj.PageTemplateId.IsNull())
                            treeTablePageList = treeTablePageList.Replace("{$url}", pageObj.Url);
                        else
                            treeTablePageList = treeTablePageList.Replace("{$url}", HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/" + globalConfiguration.DefaultLanguageCode + "/" + pageObj.FullSegment + "/?preview=true");

                        currentLevel++;

                        BuildTreeTablePageList(cmsPageList, currentLevel, pageObj.PageId, treeTablePageList, pageName);

                        currentLevel--;

                        treeTablePageList.Append("</li>" + Environment.NewLine);
                    }
                    treeTablePageList.Append("</ul>" + Environment.NewLine);
                }
            }
        }

        public int? Add(int? pageParentId, string segment, string pageName, string target, int? PageTemplateId, string url, bool showInMainMenu, bool showInBottomMenu, bool showInSitemap, bool isActive, bool isAccessRestricted, bool isHomePage)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_pages_insert",
                    "@PageParentId", pageParentId,
                    "@Segment", segment,
                    "@PageName", pageName,
                    "@Target", target,
                    "@PageTemplateId", PageTemplateId,
                    "@Url", url,
                    "@ShowInMainMenu", showInMainMenu,
                    "@ShowInBottomMenu", showInBottomMenu,
                    "@ShowInSitemap", showInSitemap,
                    "@IsActive", isActive,
                    "@IsAccessRestricted", isAccessRestricted,
                    "@IsHomePage", isHomePage,
                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(int pageId, int? pageParentId, string segment, string pageName, string target, int? PageTemplateId, string url, bool showInMainMenu, bool showInBottomMenu, bool showInSitemap, bool isActive, bool isAccessRestricted, bool isHomePage)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_pages_update",
                    "@PageId", pageId,
                    "@PageParentId", pageParentId,
                    "@Segment", segment,
                    "@PageName", pageName,
                    "@Target", target,
                    "@PageTemplateId", PageTemplateId,
                    "@Url", url,
                    "@ShowInMainMenu", showInMainMenu,
                    "@ShowInBottomMenu", showInBottomMenu,
                    "@ShowInSitemap", showInSitemap,
                    "@IsActive", isActive,
                    "@IsAccessRestricted", isAccessRestricted,
                    "@IsHomePage", isHomePage,
                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Delete(int pageId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_pages_delete", "@PageId", pageId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? MoveUp(int pageId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_pages_move_up", "@PageId", pageId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? MoveDown(int pageId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_pages_move_down", "@PageId", pageId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}