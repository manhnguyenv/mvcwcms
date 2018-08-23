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
        //  /Admin/Languages/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult Languages(BackEndLanguagesList backEndLanguagesList)
        {
            Languages languages = new Languages();
            backEndLanguagesList.LanguageList = languages.GetAllLanguages(backEndLanguagesList.LanguageCode, backEndLanguagesList.LanguageName, backEndLanguagesList.IsActive);
            if (backEndLanguagesList.LanguageList.IsNull() || backEndLanguagesList.LanguageList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndLanguagesList);
        }

        //  /Admin/LanguagesAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult LanguagesAdd()
        {
            return View();
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult LanguagesAdd(BackEndLanguagesAdd backEndLanguagesAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Languages languages = new Languages();
                int? result = languages.Add(backEndLanguagesAdd.LanguageCode, backEndLanguagesAdd.LanguageName, backEndLanguagesAdd.LanguageNameOriginal, backEndLanguagesAdd.IsActive);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndLanguagesAdd = new BackEndLanguagesAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.LanguageCodeAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndLanguagesAdd);
        }

        //  /Admin/LanguagesEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult LanguagesEdit(string id)
        {
            BackEndLanguagesEdit backEndLanguagesEdit = new BackEndLanguagesEdit();

            Languages languages = new Languages();
            Language language = languages.GetLanguageByCode(id);
            if (language.IsNotNull())
            {
                backEndLanguagesEdit.LanguageCode = language.LanguageCode;
                backEndLanguagesEdit.LanguageName = language.LanguageName;
                backEndLanguagesEdit.LanguageNameOriginal = language.LanguageNameOriginal;
                backEndLanguagesEdit.IsActive = language.IsActive;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndLanguagesEdit);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult LanguagesEdit(BackEndLanguagesEdit backEndLanguagesEdit, string id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Languages languages = new Languages();
                int? result = languages.Edit(id, backEndLanguagesEdit.LanguageCode, backEndLanguagesEdit.LanguageName, backEndLanguagesEdit.LanguageNameOriginal, backEndLanguagesEdit.IsActive);
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
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.LanguageCodeAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndLanguagesEdit);
        }

        //  /Admin/LanguagesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult LanguagesDelete(string deleteId)
        {
            Languages languages = new Languages();
            switch (languages.Delete(deleteId))
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

            return RedirectToAction("Languages");
        }
    }
}
