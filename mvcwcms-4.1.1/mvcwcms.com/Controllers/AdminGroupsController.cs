using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using MVCwCMS.Filters;
using System.Linq;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/Groups/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult Groups(BackEndGroupsList backEndGroupsList)
        {
            Groups groups = new Groups();
            backEndGroupsList.GroupList = groups.GetAllGroups(backEndGroupsList.GroupName);
            if (backEndGroupsList.GroupList.IsNull() || backEndGroupsList.GroupList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndGroupsList);
        }

        //  /Admin/GroupsAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult GroupsAdd()
        {
            return View();
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult GroupsAdd(BackEndGroupsAdd backEndGroupsAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Groups groups = new Groups();
                int? result = groups.Add(backEndGroupsAdd.GroupName);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndGroupsAdd = new BackEndGroupsAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.GroupNameAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndGroupsAdd);
        }

        //  /Admin/GroupsEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult GroupsEdit(int id)
        {
            BackEndGroupsEdit backEndGroupsEdit = new BackEndGroupsEdit();

            Groups groups = new Groups();
            Group group = groups.GetGroupById(id);
            if (group.IsNotNull())
            {
                backEndGroupsEdit.GroupName = group.GroupName;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndGroupsEdit);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult GroupsEdit(BackEndGroupsEdit backEndGroupsEdit, int id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Groups groups = new Groups();
                int? result = groups.Edit(id, backEndGroupsEdit.GroupName);
                switch (result)
                {
                    case 0:
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                        break;
                    case 3:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.GroupNameAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }
            
            return View(backEndGroupsEdit);
        }

        //  /Admin/LanguagesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult GroupsDelete(int deleteId)
        {
            Groups groups = new Groups();
            switch (groups.Delete(deleteId))
            {
                case 0:
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted);
                    break;
                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    ViewData.IsFormVisible(false);
                    break;
                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemUsedSomewhereElse);
                    break;
                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("Groups");
        }
    }
}
