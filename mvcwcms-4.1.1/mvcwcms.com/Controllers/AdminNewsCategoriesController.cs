using MVCwCMS.Filters;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/NewsCategories/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult NewsCategories(BackEndNewsCategoryList backEndNewsCategoryList)
        {
            NewsCategories newsCategories = new NewsCategories();
            backEndNewsCategoryList.NewsCategoryList = newsCategories.GetNewsCategories(backEndNewsCategoryList.CategoryName, backEndNewsCategoryList.IsActive);
            if (backEndNewsCategoryList.NewsCategoryList.IsNull() || backEndNewsCategoryList.NewsCategoryList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndNewsCategoryList);
        }

        // /Admin/NewsCategoriesAddEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult NewsCategoriesAddEdit(int? id)
        {
            BackEndNewsCategoryAddEdit backEndNewsCategoryAddEdit = new BackEndNewsCategoryAddEdit();

            NewsCategories newsCategories = new NewsCategories();
            NewsCategory newsCategory;

            BackEndNewsCategoryLanguagesAddEdit backEndNewsCategoryLanguagesAddEdit;

            List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
            if (allActiveLanguages.IsNotNull() && allActiveLanguages.Count > 0)
            {
                if (id.IsNotNull())
                {
                    List<NewsCategory> backEndNewsCategoryList = newsCategories.GetNewsCategories(id);
                    if (backEndNewsCategoryList.IsNotNull() && backEndNewsCategoryList.Count > 0)
                    {
                        backEndNewsCategoryAddEdit.CategoryId = id;
                        backEndNewsCategoryAddEdit.IsActive = backEndNewsCategoryList[0].IsActive;

                        foreach (Language language in allActiveLanguages)
                        {
                            backEndNewsCategoryLanguagesAddEdit = new BackEndNewsCategoryLanguagesAddEdit();
                            backEndNewsCategoryLanguagesAddEdit.LanguageCode = language.LanguageCode;
                            backEndNewsCategoryLanguagesAddEdit.LanguageName = language.LanguageName;

                            newsCategory = newsCategories.GetNewsCategory(id, language.LanguageCode);
                            if (newsCategory.IsNotNull())
                            {
                                backEndNewsCategoryLanguagesAddEdit.CategoryName = newsCategory.CategoryName;
                            }

                            backEndNewsCategoryAddEdit.NewsCategoryLanguages.Add(backEndNewsCategoryLanguagesAddEdit);
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
                        backEndNewsCategoryLanguagesAddEdit = new BackEndNewsCategoryLanguagesAddEdit();
                        backEndNewsCategoryLanguagesAddEdit.LanguageCode = language.LanguageCode;
                        backEndNewsCategoryLanguagesAddEdit.LanguageName = language.LanguageName;

                        backEndNewsCategoryAddEdit.NewsCategoryLanguages.Add(backEndNewsCategoryLanguagesAddEdit);
                    }
                }
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
            }

            return View(backEndNewsCategoryAddEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult NewsCategoriesAddEdit(BackEndNewsCategoryAddEdit backEndNewsCategoryAddEdit)
        {
            if (ModelState.IsValidOrRefresh())
            {
                NewsCategories newsCategories = new NewsCategories();
                int? result;
                bool isLoopSuccessful = true;
                int? lastInsertedId = null;
                int? currentId = backEndNewsCategoryAddEdit.CategoryId;
                foreach (BackEndNewsCategoryLanguagesAddEdit backEndNewsCategoryLanguagesAddEdit in backEndNewsCategoryAddEdit.NewsCategoryLanguages)
                {
                    if (currentId.IsNull())
                    {
                        currentId = lastInsertedId;
                    }
                    result = newsCategories.AddEdit(currentId,
                                                    backEndNewsCategoryLanguagesAddEdit.LanguageCode,
                                                    backEndNewsCategoryAddEdit.IsActive,
                                                    backEndNewsCategoryLanguagesAddEdit.CategoryName,
                                                    out lastInsertedId);
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
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_News.CategoryNameAlreadyExists + " (" + backEndNewsCategoryLanguagesAddEdit.LanguageName + ")");
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
                    News news = new News();
                    news.ForceCache();

                    if (backEndNewsCategoryAddEdit.CategoryId.IsNull())
                    {
                        ModelState.Clear();
                        backEndNewsCategoryAddEdit = new BackEndNewsCategoryAddEdit();
                        BackEndNewsCategoryLanguagesAddEdit backEndNewsCategoryLanguagesAddEdit;
                        List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
                        foreach (Language language in allActiveLanguages)
                        {
                            backEndNewsCategoryLanguagesAddEdit = new BackEndNewsCategoryLanguagesAddEdit();
                            backEndNewsCategoryLanguagesAddEdit.LanguageCode = language.LanguageCode;
                            backEndNewsCategoryLanguagesAddEdit.LanguageName = language.LanguageName;

                            backEndNewsCategoryAddEdit.NewsCategoryLanguages.Add(backEndNewsCategoryLanguagesAddEdit);
                        }

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    }
                }
            }

            return View(backEndNewsCategoryAddEdit);
        }

        //  /Admin/NewsCategoriesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult NewsCategoriesDelete(int deleteId)
        {
            NewsCategories newsCategories = new NewsCategories();
            switch (newsCategories.Delete(deleteId))
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

            return RedirectToAction("NewsCategories");
        }
    }
}