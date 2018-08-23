using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using MVCwCMS.Filters;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/AdminPages/
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult AdminPages(BackEndAdminPagesList backEndAdminPagesList)
        {
            AdminPages adminPages = new AdminPages();
            backEndAdminPagesList.TreeTablePageList = adminPages.GetTreeTablePageList(backEndAdminPagesList.PageName);
            if (backEndAdminPagesList.TreeTablePageList.IsNull())
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndAdminPagesList);
        }

        //  /Admin/AdminPagesAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult AdminPagesAdd(int? id)
        {
            BackEndAdminPagesAdd backEndAdminPagesAdd = new BackEndAdminPagesAdd()
            {
                PageParentId = id
            };
            return View(backEndAdminPagesAdd);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult AdminPagesAdd(BackEndAdminPagesAdd backEndAdminPagesAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                string groupsPermissions = string.Empty;
                foreach (GroupPermission g in backEndAdminPagesAdd.GroupsPermissions)
                {
                    foreach (Permission p in g.Permissions)
                    {
                        if (p.PermissionValue)
                            groupsPermissions += g.GroupId + "," + p.PermissionCode.ToString().ToLower() + "|";
                    }
                    
                }

                AdminPages adminPages = new AdminPages();
                int? result = adminPages.Add(backEndAdminPagesAdd.PageParentId, backEndAdminPagesAdd.PageName, backEndAdminPagesAdd.Target, backEndAdminPagesAdd.Url, backEndAdminPagesAdd.ShowInMenu, backEndAdminPagesAdd.IsActive, backEndAdminPagesAdd.CssClass, groupsPermissions);
                switch (result)
                {
                    case 0:
                        BackEndSessions.CurrentMenu = adminPages.GetMenuByGroupId(BackEndSessions.CurrentUser.GroupId);

                        ModelState.Clear();
                        backEndAdminPagesAdd = new BackEndAdminPagesAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndAdminPagesAdd);
        }

        //  /Admin/AdminPagesEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult AdminPagesEdit(int id)
        {
            BackEndAdminPagesEdit backEndAdminPagesEdit = new BackEndAdminPagesEdit();

            AdminPages adminPages = new AdminPages();
            AdminPage adminPage = adminPages.GetPageByPageId(id);
            if (adminPage.IsNotNull())
            {
                backEndAdminPagesEdit.PageId = adminPage.PageId;
                backEndAdminPagesEdit.PageParentId = adminPage.PageParentId;
                backEndAdminPagesEdit.PageName = adminPage.PageName;
                backEndAdminPagesEdit.Target = adminPage.Target;
                backEndAdminPagesEdit.Url = adminPage.Url;
                backEndAdminPagesEdit.ShowInMenu = adminPage.ShowInMenu;
                backEndAdminPagesEdit.IsActive = adminPage.IsActive;
                backEndAdminPagesEdit.CssClass = adminPage.CssClass;
                backEndAdminPagesEdit.GroupsPermissions = adminPage.GroupsPermissions;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndAdminPagesEdit);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult AdminPagesEdit(BackEndAdminPagesEdit backEndAdminPagesEdit, int id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                string groupsPermissions = string.Empty;
                foreach (GroupPermission g in backEndAdminPagesEdit.GroupsPermissions)
                {
                    foreach (Permission p in g.Permissions)
                    {
                        if (p.PermissionValue)
                            groupsPermissions += g.GroupId + "," + p.PermissionCode.ToString().ToLower() + "|";
                    }

                }

                AdminPages adminPages = new AdminPages();
                int? result = adminPages.Edit(id, backEndAdminPagesEdit.PageParentId, backEndAdminPagesEdit.PageName, backEndAdminPagesEdit.Target, backEndAdminPagesEdit.Url, backEndAdminPagesEdit.ShowInMenu, backEndAdminPagesEdit.IsActive, backEndAdminPagesEdit.CssClass, groupsPermissions);
                switch (result)
                {
                    case 0:
                        BackEndSessions.CurrentMenu = adminPages.GetMenuByGroupId(BackEndSessions.CurrentUser.GroupId);

                        
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                        break;
                    case 3:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndAdminPagesEdit);
        }

        //  /Admin/AdminPagesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult AdminPagesDelete(int deleteId)
        {
            AdminPages adminPages = new AdminPages();
            switch (adminPages.Delete(deleteId))
            {
                case 0:
                    BackEndSessions.CurrentMenu = adminPages.GetMenuByGroupId(BackEndSessions.CurrentUser.GroupId);

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted);
                    break;
                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;
                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageHasSubPages);
                    break;
                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("AdminPages");
        }

        //  /Admin/AdminPagesMoveUp/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult AdminPagesMoveUp(int postId)
        {
            AdminPages adminPages = new AdminPages();
            switch (adminPages.MoveUp(postId))
            {
                case 0:
                    BackEndSessions.CurrentMenu = adminPages.GetMenuByGroupId(BackEndSessions.CurrentUser.GroupId);

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.PageSuccessfullyMoved);
                    break;
                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;
                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageCannotBeMoved);
                    break;
                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("AdminPages");
        }

        //  /Admin/AdminPagesMoveDown/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult AdminPagesMoveDown(int postId)
        {
            AdminPages adminPages = new AdminPages();
            switch (adminPages.MoveDown(postId))
            {
                case 0:
                    BackEndSessions.CurrentMenu = adminPages.GetMenuByGroupId(BackEndSessions.CurrentUser.GroupId);

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.PageSuccessfullyMoved);
                    break;
                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;
                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageCannotBeMoved);
                    break;
                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("AdminPages");
        }
    }
}
