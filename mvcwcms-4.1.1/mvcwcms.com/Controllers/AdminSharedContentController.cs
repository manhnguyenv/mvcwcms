using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MVCwCMS.Filters;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/SharedContent/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult SharedContent(BackEndSharedContentList backEndSharedContentList)
        {
            SharedContents sharedContents = new SharedContents();
            backEndSharedContentList.SharedContentList = sharedContents.GetSharedContents(backEndSharedContentList.SharedContentCode, backEndSharedContentList.IsActive);
            if (backEndSharedContentList.SharedContentList.IsNull() || backEndSharedContentList.SharedContentList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndSharedContentList);
        }

        // /Admin/SharedContentAddEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult SharedContentAddEdit(string id)
        {
            BackEndSharedContentAddEdit backEndSharedContent = new BackEndSharedContentAddEdit();

            SharedContents sharedContents = new SharedContents();
            SharedContent sharedContent;

            BackEndSharedContentLanguagesAddEdit backEndSharedContentLanguages;

            List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
            if (allActiveLanguages.IsNotNull() && allActiveLanguages.Count > 0)
            {
                if (id.IsNotEmptyOrWhiteSpace())
                {
                    List<SharedContent> backEndSharedContentList = sharedContents.GetSharedContents(id);
                    if (backEndSharedContentList.IsNotNull() && backEndSharedContentList.Count > 0)
                    {
                        backEndSharedContent.SharedContentCode = id;
                        backEndSharedContent.NewSharedContentCode = id;
                        backEndSharedContent.IsActive = backEndSharedContentList[0].IsActive;
                        
                        foreach (Language language in allActiveLanguages)
                        {
                            backEndSharedContentLanguages = new BackEndSharedContentLanguagesAddEdit();
                            backEndSharedContentLanguages.LanguageCode = language.LanguageCode;
                            backEndSharedContentLanguages.LanguageName = language.LanguageName;

                            sharedContent = sharedContents.GetSharedContent(id, language.LanguageCode);
                            if (sharedContent.IsNotNull())
                            {
                                backEndSharedContentLanguages.HtmlCode = sharedContent.HtmlCode;
                            }

                            backEndSharedContent.SharedContentLanguages.Add(backEndSharedContentLanguages);
                        }
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                    }
                }
                else
                {
                    foreach (Language language in allActiveLanguages)
                    {
                        backEndSharedContentLanguages = new BackEndSharedContentLanguagesAddEdit();
                        backEndSharedContentLanguages.LanguageCode = language.LanguageCode;
                        backEndSharedContentLanguages.LanguageName = language.LanguageName;

                        backEndSharedContent.SharedContentLanguages.Add(backEndSharedContentLanguages);
                    }
                }
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
            }

            return View(backEndSharedContent);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult SharedContentAddEdit(BackEndSharedContentAddEdit backEndSharedContent)
        {
            if (ModelState.IsValidOrRefresh())
            {
                SharedContents sharedContents = new SharedContents();
                int? result;
                bool isLoopSuccessful = true;
                foreach (BackEndSharedContentLanguagesAddEdit backEndSharedContentLanguages in backEndSharedContent.SharedContentLanguages)
                {
                    result = sharedContents.AddEdit(backEndSharedContent.SharedContentCode, backEndSharedContent.NewSharedContentCode, backEndSharedContentLanguages.LanguageCode, backEndSharedContent.IsActive, backEndSharedContentLanguages.HtmlCode);
                    switch (result)
                    {
                        case 0:
                            //success
                            break;
                        case 2:
                            isLoopSuccessful = false;
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                            ViewData.IsFormVisible(false);
                            break;
                        case 3:
                            isLoopSuccessful = false;
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_SharedContent.SharedContentCodeAlreadyExists);
                            break;
                        default:
                            isLoopSuccessful = false;
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                            break;
                    }
                    if (!isLoopSuccessful)
                        break;
                }
                if (isLoopSuccessful)
                {
                    if (backEndSharedContent.SharedContentCode.IsEmptyOrWhiteSpace())
                    {
                        ModelState.Clear();
                        backEndSharedContent = new BackEndSharedContentAddEdit();
                        BackEndSharedContentLanguagesAddEdit backEndSharedContentLanguages;
                        List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
                        foreach (Language language in allActiveLanguages)
                        {
                            backEndSharedContentLanguages = new BackEndSharedContentLanguagesAddEdit();
                            backEndSharedContentLanguages.LanguageCode = language.LanguageCode;
                            backEndSharedContentLanguages.LanguageName = language.LanguageName;

                            backEndSharedContent.SharedContentLanguages.Add(backEndSharedContentLanguages);
                        }

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    }
                }
            }

            return View(backEndSharedContent);
        }
        

        //  /Admin/SharedContentDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult SharedContentDelete(string deleteId)
        {
            SharedContents sharedContents = new SharedContents();
            switch (sharedContents.Delete(deleteId))
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

            return RedirectToAction("SharedContent");
        }
    }
}
