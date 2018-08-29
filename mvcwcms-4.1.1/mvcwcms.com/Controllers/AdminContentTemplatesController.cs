using MVCwCMS.Filters;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/ContentTemplates/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult ContentTemplates(BackEndContentTemplateList backEndContentTemplateList)
        {
            ContentTemplates contentTemplates = new ContentTemplates();
            backEndContentTemplateList.ContentTemplateList = contentTemplates.GetAllContentTemplates(backEndContentTemplateList.Title, backEndContentTemplateList.Description, backEndContentTemplateList.IsActive);
            if (backEndContentTemplateList.ContentTemplateList.IsNull() || backEndContentTemplateList.ContentTemplateList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndContentTemplateList);
        }

        //  /Admin/ContentTemplatesAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult ContentTemplatesAdd()
        {
            return View();
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult ContentTemplatesAdd(BackEndContentTemplatesAdd backEndContentTemplatesAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                ContentTemplates contentTemplates = new ContentTemplates();
                int? result = contentTemplates.Add(backEndContentTemplatesAdd.Title, backEndContentTemplatesAdd.Description, backEndContentTemplatesAdd.Content, backEndContentTemplatesAdd.IsActive);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndContentTemplatesAdd = new BackEndContentTemplatesAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ContentTemplateTitleAlreadyExists);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndContentTemplatesAdd);
        }

        //  /Admin/ContentTemplatesEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult ContentTemplatesEdit(int id)
        {
            BackEndContentTemplatesEdit backEndContentTemplatesEdit = new BackEndContentTemplatesEdit();

            ContentTemplate contentTemplate = new ContentTemplates().GetContentTemplateById(id);
            if (contentTemplate.IsNotNull())
            {
                backEndContentTemplatesEdit.Title = contentTemplate.title;
                backEndContentTemplatesEdit.Description = contentTemplate.description;
                backEndContentTemplatesEdit.Content = contentTemplate.content;
                backEndContentTemplatesEdit.IsActive = contentTemplate.IsActive;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndContentTemplatesEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult ContentTemplatesEdit(BackEndContentTemplatesEdit backEndContentTemplatesEdit, int id)
        {
            ContentTemplates contentTemplates = new ContentTemplates();
            int? result = contentTemplates.Edit(id, backEndContentTemplatesEdit.Title, backEndContentTemplatesEdit.Description, backEndContentTemplatesEdit.Content, backEndContentTemplatesEdit.IsActive);
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
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ContentTemplateTitleAlreadyExists);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return View(backEndContentTemplatesEdit);
        }

        //  /Admin/ContentTemplatesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult ContentTemplatesDelete(int deleteId)
        {
            ContentTemplates contentTemplates = new ContentTemplates();
            switch (contentTemplates.Delete(deleteId))
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

            return RedirectToAction("ContentTemplates");
        }
    }
}