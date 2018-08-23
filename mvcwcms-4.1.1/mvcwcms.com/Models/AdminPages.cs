using MVCwCMS.Helpers;
using MVCwCMS.Resources;
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
    public class AdminPage
    {
        public int? GroupId { get; set; }
        public string PermissionCode { get; set; }
        public List<GroupPermission> GroupsPermissions { get; set; }
        public int? PageId { get; set; }
        public int? PageParentId { get; set; }
        public string PageName { get; set; }
        public string Target { get; set; }
        public string Url { get; set; }
        public int? Ordering { get; set; }
        public bool ShowInMenu { get; set; }
        public bool IsActive { get; set; }
        public bool IsModal { get; set; }
        public string CssClass { get; set; }
    }

    public class AdminPages
    {
        private List<AdminPage> _AllItems;

        private List<AdminPage> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<AdminPage>("sp_admin_pages_select", force, false);
        }

        public AdminPages()
        {
            _AllItems = GetAllItems();
        }

        public List<AdminPage> GetAllPages(bool showHierarchicalPageNames = false, int? excludedPageIdBranch = null)
        {
            if (showHierarchicalPageNames)
            {
                //Groups pages by PageId, PageParentId, PageName, Target, Url, Ordering, ShowInMenu, IsActive
                List<AdminPage> adminPageList = _AllItems.Clone().GroupBy(i => new
                {
                    i.PageId,
                    i.PageParentId,
                    i.PageName,
                    i.Target,
                    i.Url,
                    i.Ordering,
                    i.ShowInMenu,
                    i.IsActive,
                    i.CssClass
                }).Select(i => i.First()).ToList();

                List<AdminPage> pageList = new List<AdminPage>();
                GetHierarchicalTitles(adminPageList, ref pageList, null, string.Empty, excludedPageIdBranch);
                return pageList;
            }
            else
            {
                return _AllItems;
            }
        }
        private void GetHierarchicalTitles(List<AdminPage> adminPageList, ref List<AdminPage> pageList, int? currentPageIdParent, string levelChars, int? excludedPageIdBranch)
        {
            List<AdminPage> pages = (from p in adminPageList
                                     where p.PageParentId == currentPageIdParent
                                     orderby p.Ordering
                                     select p).ToList();
            foreach (AdminPage p in pages)
            {
                if (excludedPageIdBranch.IsNull() || p.PageId != excludedPageIdBranch)
                {
                    p.PageName = levelChars + p.PageName;
                    pageList.Add(p);
                    GetHierarchicalTitles(adminPageList, ref pageList, p.PageId, levelChars + "―", excludedPageIdBranch);
                }
            }
        }

        public AdminPage GetPageByAction(string action)
        {
            AdminPage result = null;
            if (_AllItems.IsNotNull())
            {
                action = action.ToLower().TrimEnd('/');

                if (action == "index")
                    action = "/admin";
                else
                    action = "/admin/" + action;

                result = (from i in _AllItems
                          where i.Url.ToLower().TrimEnd('/') == action
                          select new AdminPage() {
                              PageId = i.PageId,
                              PageParentId = i.PageParentId,
                              //PageName = Strings.ResourceManager.GetString(i.PageName.Substring(2, i.PageName.Length - 3)).IfEmpty(i.PageName),
                              PageName = (i.PageName.Substring(2, i.PageName.Length - 3)).GetStringFromResource().IfEmpty(i.PageName),
                              Target = i.Target,
                              Url = i.Url,
                              Ordering = i.Ordering,
                              ShowInMenu = i.ShowInMenu,
                              IsActive = i.IsActive,
                              CssClass = i.CssClass,
                              GroupId = i.GroupId,
                              PermissionCode = i.PermissionCode,
                              IsModal = i.IsModal,
                              GroupsPermissions = i.GroupsPermissions
                          }).FirstOrDefault();
            }

            return result;
        }

        public AdminPage GetPageByCurrentAction()
        {
            var rd = HttpContext.Current.Request.RequestContext.RouteData;
            return GetPageByAction(rd.GetRequiredString("action")) ?? new AdminPage();
        }

        public AdminPage GetPageByPageId(int? id)
        {
            AdminPage result;

            //Groups pages by PageId, PageParentId, PageName, Target, Url, Ordering, ShowInMenu, IsActive
            List<AdminPage> adminPageList = (from p in _AllItems
                                             group p by new { p.PageId, p.PageParentId, p.PageName, p.Target, p.Url, p.Ordering, p.ShowInMenu, p.IsActive, p.CssClass } into g
                                             select new AdminPage
                                             {
                                                 PageId = g.First().PageId,
                                                 PageParentId = g.First().PageParentId,
                                                 PageName = g.First().PageName,
                                                 Target = g.First().Target,
                                                 Url = g.First().Url,
                                                 Ordering = g.First().Ordering,
                                                 ShowInMenu = g.First().ShowInMenu,
                                                 IsActive = g.First().IsActive,
                                                 CssClass = g.First().CssClass,
                                                 GroupsPermissions = new List<GroupPermission>()
                                             }).ToList();

            //Populates GroupsPermissions based on current database values
            foreach (AdminPage ap in adminPageList)
            {
                foreach (Group g in new Groups().GetAllGroups())
                {
                    GroupPermission groupPermission = new GroupPermission()
                    {
                        GroupId = g.GroupId,
                        Permissions = new List<Permission>()
                    };
                    foreach (PermissionCode p in (PermissionCode[])Enum.GetValues(typeof(PermissionCode)))
                    {
                        groupPermission.Permissions.Add(new Permission()
                        {
                            PermissionCode = p,
                            PermissionValue = _AllItems.Where(x => x.PageId == ap.PageId && x.GroupId == g.GroupId && x.PermissionCode.ToLower() == p.ToString().ToLower()).Count() > 0
                        });
                    }

                    ap.GroupsPermissions.Add(groupPermission);
                }
            }

            result = (from page in adminPageList
                      where page.PageId == id
                      select page).FirstOrDefault();
            return result;
        }

        public string GetMenuByGroupId(int? groupId)
        {
            StringBuilder result = new StringBuilder();

            List<AdminPage> pageItems = (from i in _AllItems
                                         where i.GroupId == groupId
                                         && i.PermissionCode == PermissionCode.Browse.ToString().ToLower()
                                         && i.IsActive == true
                                         select i).ToList();
            BuildMenu(pageItems, 1, null, result);

            return result.ToString();
        }
        private void BuildMenu(List<AdminPage> pageItems, int currentLevel, int? id_page_parent, StringBuilder result)
        {
            if (pageItems != null)
            {
                string pageTitle;

                List<AdminPage> pagesObj = (from page in pageItems
                                              where page.PageParentId == id_page_parent
                                              && page.ShowInMenu == true
                                              orderby page.Ordering
                                              select page).ToList<AdminPage>();
                pagesObj = pagesObj.Where(page => page.ShowInMenu == true).ToList<AdminPage>();

                if (pagesObj != null && pagesObj.Count() > 0)
                {
                    result.Append(Environment.NewLine + "<ul");
                    if (currentLevel == 1)
                        result.Append(" id=\"side-menu\" class=\"nav\"");
                    else
                        result.Append(" class=\"nav nav-second-level\"");
                    result.Append(">" + Environment.NewLine);

                    string pageName;

                    foreach (AdminPage pageObj in pagesObj)
                    {
                        bool hasChildren = (from page in pageItems
                                            where page.PageParentId == pageObj.PageId
                                            && page.ShowInMenu == true
                                            select page).Count() > 0;

                        //pageName = Strings.ResourceManager.GetString(pageObj.PageName.Substring(2, pageObj.PageName.Length - 3));
                        pageName = (pageObj.PageName.Substring(2, pageObj.PageName.Length - 3)).GetStringFromResource();
                        if (pageName.IsEmptyOrWhiteSpace())
                        {
                            pageName = pageObj.PageName;
                        }

                        pageTitle = pageName.TrimToMaxLength(30, "...");

                        result.Append("<li>");

                        if (pageObj.Url.IsNotEmptyOrWhiteSpace())
                        {
                            result.Append("<a href=\"" + pageObj.Url + "\" title=\"" + pageName + "\" target=\"" + pageObj.Target + "\" class=\"" + pageObj.CssClass + "\">" + pageTitle);
                            if (hasChildren)
                            {
                                result.Append("<span class=\"fa arrow\"></span>");
                            }
                            result.Append("</a>");
                        }
                        else
                        {
                            result.Append("<a href=\"#\">" + pageTitle + "<span class=\"fa arrow\"></span></a>");
                        }

                        currentLevel++;

                        BuildMenu(pageItems, currentLevel, pageObj.PageId, result);

                        currentLevel--;

                        result.Append("</li>" +Environment.NewLine);
                    }

                    result.Append("</ul>" + Environment.NewLine);
                }
            }
        }

        public HtmlString GetTreeTablePageList(string pageName = null)
        {
            StringBuilder treeTablePageList = new StringBuilder();

            //Groups pages by PageId, PageParentId, PageName, Target, Url, Ordering, ShowInMenu, IsActive
            List<AdminPage> adminPageList = _AllItems.GroupBy(i => new
            {
                i.PageId,
                i.PageParentId,
                i.PageName,
                i.Target,
                i.Url,
                i.Ordering,
                i.ShowInMenu,
                i.IsActive,
                i.CssClass
            }).Select(i => i.First()).ToList();

            BuildTreeTablePageList(adminPageList, 1, null, treeTablePageList, pageName);

            if (treeTablePageList.ToString().IsNotEmptyOrWhiteSpace())
            {
                return new HtmlString(treeTablePageList.ToString());
            }
            else
            {
                return null;
            }
        }
        private void BuildTreeTablePageList(List<AdminPage> adminPageList, int currentLevel, int? pageParentId, StringBuilder treeTablePageList, string pageName)
        {
            if (adminPageList != null)
            {
                List<AdminPage> pagesObj = (from page in adminPageList
                                            where page.PageParentId == pageParentId
                                            select page).ToList();

                if (pagesObj != null && pagesObj.Count() > 0)
                {
                    if (currentLevel == 1)
                    {
                        treeTablePageList.Append("<div class=\"AdminPagesTreeControl btn-group\">" + Environment.NewLine +
                                                 "<a class=\"btn btn-default btn-xs\" href=\"#\"><i class=\"fa fa-compress fa-flip-horizontal\"></i> " + Resources.Strings.CollapseAll + "</a>" + Environment.NewLine +
                                                 "<a class=\"btn btn-default btn-xs\" href=\"#\"><i class=\"fa fa-expand fa-flip-horizontal\"></i> " + Resources.Strings.ExpandAll + "</a>" + Environment.NewLine +
                                                 "<a class=\"btn btn-default btn-xs\" href=\"#\"><i class=\"fa fa-arrows-alt\"></i> " + Resources.Strings.ToggleAll + "</a>" + Environment.NewLine +
                                                 "</div>");
                    }
                    treeTablePageList.Append(Environment.NewLine + "<ul");
                    if (currentLevel == 1)
                        treeTablePageList.Append(" class=\"AdminPagesTree\" ");
                    treeTablePageList.Append(">" + Environment.NewLine);
                    if (currentLevel == 1)
                    {
                        treeTablePageList.Append("<li><table class=\"table table-condensed table-bordered table-hover\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><thead><tr class=\"webgrid-header\">" +
                                                 "<th class=\"\">" + Resources.Strings.PageName + "</th>" +
                                                 "<th class=\"col-120\">" + Resources.Strings.Actions + "</th>" +
                                                 "<th class=\"col-60\">" + Resources.Strings.Active + "</th>" +
                                                 "<th class=\"col-100\">" + Resources.Strings.ShowInMenu + "</th>" +
                                                 "</tr></thead></table></li>");
                    }

                    string imgYes = "<i class=\"fa fa-check\"></i>";
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
                    string highlight;

                    AdminPages adminPages = new AdminPages();
                    AdminPage adminPage = adminPages.GetPageByCurrentAction();

                    foreach (AdminPage pageObj in pagesObj)
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
                                                 "<th class=\"col-120\">" + Resources.Strings.Actions + "</th>" +
                                                 "<th class=\"col-60\">" + Resources.Strings.Active + "</th>" +
                                                 "<th class=\"col-100\">" + Resources.Strings.ShowInMenu + "</th>" +
                                                 "</thead>" +
                                                 "<tbody><tr>" +
                                                 "<td class=\"" + highlight + "\">" + pageObj.PageName + "</td>" +
                                                 "<td class=\"text-center\"><div class=\"btn-group\">");
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Add))
                        {
                            treeTablePageList.Append("<a href=\"/Admin/AdminPagesAdd/" + pageObj.PageId.ToString() + "\" class=\"btn btn-default btn-xs\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.AdminPagesAdd + "\" ><i class=\"fa fa-plus\"></i></a>");
                        }
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Edit))
                        {
                            treeTablePageList.Append("<a href=\"/Admin/AdminPagesEdit/" + pageObj.PageId.ToString() + "\" class=\"btn btn-default btn-xs\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.AdminPagesEdit + "\" ><i class=\"fa fa-pencil\"></i></a>");
                        }
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Delete))
                        {
                            treeTablePageList.Append("<button type=\"submit\" data-action=\"/Admin/AdminPagesDelete\" data-id=\"" + pageObj.PageId.ToString() + "\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.AdminPagesDelete + "\" class=\"btn btn-default btn-xs btn-admin-pages action-delete\" data-action-delete-item=\"" + pageObj.PageName + "\"><i class=\"fa fa-trash-o\"></i></button>");
                        }
                        if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Edit))
                        {
                            treeTablePageList.Append("<button type=\"submit\" data-action=\"/Admin/AdminPagesMoveUp/#r-" + pageObj.PageId.ToString() + "\" data-id=\"" + pageObj.PageId.ToString() + "\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.MovePageUpInSameLevel + "\" class=\"btn btn-default btn-xs btn-admin-pages action-post-id\" \"><i class=\"fa fa-chevron-up\"></i></button>" +
                                                     "<button type=\"submit\" data-action=\"/Admin/AdminPagesMoveDown/#r-" + pageObj.PageId.ToString() + "\" data-id=\"" + pageObj.PageId.ToString() + "\" data-toggle=\"tooltip\" title=\"" + Resources.Strings.MovePageDownInSameLevel + "\" class=\"btn btn-default btn-xs btn-admin-pages action-post-id\" \"><i class=\"fa fa-chevron-down\"></i></button>");
                        }                         
                        treeTablePageList.Append("</div></td>" +
                                                 "<td class=\"text-center\">" + (pageObj.IsActive.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "<td class=\"text-center\">" + (pageObj.ShowInMenu.ConvertTo<bool>(false, true) ? imgYes : "") + "</td>" +
                                                 "</tr></tbody></table>" + Environment.NewLine);

                        currentLevel++;

                        BuildTreeTablePageList(adminPageList, currentLevel, pageObj.PageId, treeTablePageList, pageName);

                        currentLevel--;

                        treeTablePageList.Append("</li>" + Environment.NewLine);
                    }
                    treeTablePageList.Append("</ul>" + Environment.NewLine);
                }
            }
        }

        public int? Add(int? pageParentId, string pageName, string target, string url, bool showInMenu, bool isActive, string cssClass, string groupsPermissions)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_pages_insert",
                    "@PageParentId", pageParentId,
                    "@PageName", pageName,
                    "@Target", target,
                    "@Url", url,
                    "@ShowInMenu", showInMenu,
                    "@IsActive", isActive,
                    "@CssClass", cssClass,
                    "@GroupsPermissions", groupsPermissions,
                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(int pageId, int? pageParentId, string pageName, string target, string url, bool showInMenu, bool isActive, string cssClass, string groupsPermissions)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_pages_update",
                    "@PageId", pageId,
                    "@PageParentId", pageParentId,
                    "@PageName", pageName,
                    "@Target", target,
                    "@Url", url,
                    "@ShowInMenu", showInMenu,
                    "@IsActive", isActive,
                    "@CssClass", cssClass,
                    "@GroupsPermissions", groupsPermissions,
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
                db.ExecNonQueryProc("sp_admin_pages_delete", "@PageId", pageId, returnValue);
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
                db.ExecNonQueryProc("sp_admin_pages_move_up", "@PageId", pageId, returnValue);
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
                db.ExecNonQueryProc("sp_admin_pages_move_down", "@PageId", pageId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public bool IsPermissionGranted(int? pageId, PermissionCode permissionCode)
        {
            bool result = false;
            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.GroupId == BackEndSessions.CurrentUser.GroupId
                             && i.PageId == pageId
                             && i.PermissionCode.ToLower() == permissionCode.ToString().ToLower()
                          select i).Count() > 0;
            }
            return result;
        }
    }
}