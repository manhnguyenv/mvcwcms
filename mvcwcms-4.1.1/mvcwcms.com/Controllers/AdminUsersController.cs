using MVCwCMS.Filters;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/Users/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult Users(BackEndUsersList backEndUsersList)
        {
            Users users = new Users();
            backEndUsersList.UserList = users.GetAllUsers(backEndUsersList.Username, backEndUsersList.FullName, backEndUsersList.Email, backEndUsersList.GroupId);
            if (backEndUsersList.UserList.IsNull() || backEndUsersList.UserList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndUsersList);
        }

        //  /Admin/UsersAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult UsersAdd()
        {
            return View();
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult UsersAdd(BackEndUsersAdd backEndUsersAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Users users = new Users();
                int? result = users.Add(backEndUsersAdd.Username, backEndUsersAdd.Password, backEndUsersAdd.FullName, backEndUsersAdd.Email, backEndUsersAdd.GroupId);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndUsersAdd = new BackEndUsersAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UsernameAlreadyExists);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndUsersAdd);
        }

        //  /Admin/UsersEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult UsersEdit(string id)
        {
            BackEndUsersEdit backEndUsersEdit = new BackEndUsersEdit();

            Users users = new Users();
            User user = users.GetUserByUserName(id);
            if (user.IsNotNull())
            {
                backEndUsersEdit.Username = user.UserName;
                backEndUsersEdit.FullName = user.FullName;
                backEndUsersEdit.Email = user.Email;
                backEndUsersEdit.GroupId = user.GroupId;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndUsersEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult UsersEdit(BackEndUsersEdit backEndUsersEdit, string id)
        {
            Users users = new Users();
            int? result = users.Edit(id, backEndUsersEdit.Password, backEndUsersEdit.FullName, backEndUsersEdit.Email, backEndUsersEdit.GroupId);
            switch (result)
            {
                case 0:

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    ViewData.IsFormVisible(false);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return View(backEndUsersEdit);
        }

        //  /Admin/UsersDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult UsersDelete(string deleteId)
        {
            Users users = new Users();
            switch (users.Delete(deleteId))
            {
                case 0:
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;

                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemUsedSomewhereElse);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("Users");
        }
    }
}