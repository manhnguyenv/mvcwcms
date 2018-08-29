using MVCwCMS.Filters;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/PageTemplates/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult PageTemplates(BackEndPageTemplatesList backEndPageTemplatesList)
        {
            PageTemplates pageTemplates = new PageTemplates();
            backEndPageTemplatesList.PageTemplateList = pageTemplates.GetAllPageTemplates(backEndPageTemplatesList.Title, backEndPageTemplatesList.IsActive);
            if (backEndPageTemplatesList.PageTemplateList.IsNull() || backEndPageTemplatesList.PageTemplateList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndPageTemplatesList);
        }

        //  /Admin/PageTemplatesAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult PageTemplatesAdd()
        {
            return View();
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult PageTemplatesAdd(BackEndPageTemplatesAdd backEndPageTemplatesAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                PageTemplates pageTemplates = new PageTemplates();
                int? result = pageTemplates.Add(backEndPageTemplatesAdd.Title, backEndPageTemplatesAdd.HtmlCode, backEndPageTemplatesAdd.IsActive);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndPageTemplatesAdd = new BackEndPageTemplatesAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageTemplateNameAlreadyExists);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndPageTemplatesAdd);
        }

        //  /Admin/PageTemplatesEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult PageTemplatesEdit(int id)
        {
            BackEndPageTemplatesEdit backEndPageTemplatesEdit = new BackEndPageTemplatesEdit();

            PageTemplates pageTemplates = new PageTemplates();
            PageTemplate pageTemplate = pageTemplates.GetPageTemplateById(id);
            if (pageTemplate.IsNotNull())
            {
                backEndPageTemplatesEdit.Title = pageTemplate.Title;
                backEndPageTemplatesEdit.HtmlCode = pageTemplate.HtmlCode;
                backEndPageTemplatesEdit.IsActive = pageTemplate.IsActive;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndPageTemplatesEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult PageTemplatesEdit(BackEndPageTemplatesEdit backEndPageTemplatesEdit, int id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                PageTemplates pageTemplates = new PageTemplates();
                int? result = pageTemplates.Edit(id, backEndPageTemplatesEdit.Title, backEndPageTemplatesEdit.HtmlCode, backEndPageTemplatesEdit.IsActive);
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
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageTemplateNameAlreadyExists);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndPageTemplatesEdit);
        }

        //  /Admin/PageTemplatesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult PageTemplatesDelete(int deleteId)
        {
            PageTemplates pageTemplates = new PageTemplates();
            switch (pageTemplates.Delete(deleteId))
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

            return RedirectToAction("PageTemplates");
        }
    }
}