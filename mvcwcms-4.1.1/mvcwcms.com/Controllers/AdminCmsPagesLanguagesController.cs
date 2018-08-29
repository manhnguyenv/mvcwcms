using MVCwCMS.Filters;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/CmsPagesLanguages/
        [HttpGet]
        [IsRestricted]
        public ActionResult CmsPagesLanguages(int id)
        {
            List<BackEndCmsPagesLanguagesAddEdit> backEndCmsPagesLanguagesAddEditList = new List<BackEndCmsPagesLanguagesAddEdit>();

            CmsPages CmsPages = new CmsPages();
            CmsPage CmsPage;
            BackEndCmsPagesLanguagesAddEdit backEndCmsPagesLanguagesAddEdit;
            PagesLanguages backEndCmsPagesLanguages = new PagesLanguages();
            PageLanguage backEndCmsPageLanguage;
            foreach (Language language in new Languages().GetAllLanguages(isActive: true))
            {
                backEndCmsPagesLanguagesAddEdit = new BackEndCmsPagesLanguagesAddEdit();
                CmsPage = CmsPages.GetPageByPageId(id);
                if (CmsPage.IsNotNull())
                {
                    backEndCmsPagesLanguagesAddEdit.PageId = CmsPage.PageId;
                    backEndCmsPagesLanguagesAddEdit.LanguageCode = language.LanguageCode;
                    backEndCmsPagesLanguagesAddEdit.PageName = CmsPage.PageName;
                    backEndCmsPagesLanguagesAddEdit.LanguageName = language.LanguageName;

                    backEndCmsPageLanguage = backEndCmsPagesLanguages.GetPageLanguage(CmsPage.PageId, language.LanguageCode);
                    if (backEndCmsPageLanguage.IsNotNull())
                    {
                        backEndCmsPagesLanguagesAddEdit.MenuName = backEndCmsPageLanguage.MenuName;
                        backEndCmsPagesLanguagesAddEdit.MetaTagTitle = backEndCmsPageLanguage.MetaTagTitle;
                        backEndCmsPagesLanguagesAddEdit.MetaTagKeywords = backEndCmsPageLanguage.MetaTagKeywords;
                        backEndCmsPagesLanguagesAddEdit.MetaTagDescription = backEndCmsPageLanguage.MetaTagDescription;
                        backEndCmsPagesLanguagesAddEdit.Robots = backEndCmsPageLanguage.Robots;
                        backEndCmsPagesLanguagesAddEdit.HtmlCode = backEndCmsPageLanguage.HtmlCode;
                    }
                    else
                    {
                        backEndCmsPagesLanguages.AddEdit(id, backEndCmsPagesLanguagesAddEdit.LanguageCode, CmsPage.PageName, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                        backEndCmsPageLanguage = backEndCmsPagesLanguages.GetPageLanguage(CmsPage.PageId, language.LanguageCode);
                    }
                }
                else
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    ViewData.IsFormVisible(false);
                    break;
                }
                backEndCmsPagesLanguagesAddEditList.Add(backEndCmsPagesLanguagesAddEdit);
            }

            return View(backEndCmsPagesLanguagesAddEditList);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult CmsPagesLanguages(List<BackEndCmsPagesLanguagesAddEdit> backEndCmsPagesLanguagesAddEditList, int id)
        {
            PagesLanguages backEndCmsPagesLanguages = new PagesLanguages();
            int? result;
            bool isLoopSuccessful = true;
            foreach (BackEndCmsPagesLanguagesAddEdit backEndCmsPagesLanguagesAddEdit in backEndCmsPagesLanguagesAddEditList)
            {
                result = backEndCmsPagesLanguages.AddEdit(id, backEndCmsPagesLanguagesAddEdit.LanguageCode, backEndCmsPagesLanguagesAddEdit.MenuName, backEndCmsPagesLanguagesAddEdit.MetaTagTitle, backEndCmsPagesLanguagesAddEdit.MetaTagKeywords, backEndCmsPagesLanguagesAddEdit.MetaTagDescription, backEndCmsPagesLanguagesAddEdit.Robots, backEndCmsPagesLanguagesAddEdit.HtmlCode);
                switch (result)
                {
                    case 0:
                        //success
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
                ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
            }

            return View(backEndCmsPagesLanguagesAddEditList);
        }
    }
}