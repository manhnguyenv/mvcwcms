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
        //  /Admin/CmsPages/
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult CmsPages(BackEndCmsPagesList backEndCmsPagesList)
        {
            CmsPages CmsPages = new CmsPages();
            backEndCmsPagesList.TreeTablePageList = CmsPages.GetTreeTablePageList(backEndCmsPagesList.PageName);
            if (backEndCmsPagesList.TreeTablePageList.IsNull())
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndCmsPagesList);
        }

        //  /Admin/CmsPagesAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult CmsPagesAdd(int? id)
        {
            BackEndCmsPagesAdd backEndCmsPagesAdd = new BackEndCmsPagesAdd()
            {
                PageParentId = id
            };
            return View(backEndCmsPagesAdd);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult CmsPagesAdd(BackEndCmsPagesAdd backEndCmsPagesAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                CmsPages CmsPages = new CmsPages();
                int? result = CmsPages.Add(backEndCmsPagesAdd.PageParentId, backEndCmsPagesAdd.Segment, backEndCmsPagesAdd.PageName, backEndCmsPagesAdd.Target, backEndCmsPagesAdd.PageTemplateId, backEndCmsPagesAdd.Url, backEndCmsPagesAdd.ShowInMainMenu, backEndCmsPagesAdd.ShowInBottomMenu, backEndCmsPagesAdd.ShowInSitemap, backEndCmsPagesAdd.IsActive, backEndCmsPagesAdd.IsAccessRestricted, backEndCmsPagesAdd.IsHomePage);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndCmsPagesAdd = new BackEndCmsPagesAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageSegmentAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndCmsPagesAdd);
        }

        //  /Admin/CmsPagesEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult CmsPagesEdit(int id)
        {
            BackEndCmsPagesEdit backEndCmsPagesEdit = new BackEndCmsPagesEdit();

            CmsPages CmsPages = new CmsPages();
            CmsPage CmsPage = CmsPages.GetPageByPageId(id);
            if (CmsPage.IsNotNull())
            {
                backEndCmsPagesEdit.PageId = CmsPage.PageId;
                backEndCmsPagesEdit.PageParentId = CmsPage.PageParentId;
                backEndCmsPagesEdit.Segment = CmsPage.Segment;
                backEndCmsPagesEdit.PageName = CmsPage.PageName;
                backEndCmsPagesEdit.Target = CmsPage.Target;
                backEndCmsPagesEdit.PageTemplateId = CmsPage.PageTemplateId;
                backEndCmsPagesEdit.Url = CmsPage.Url;
                backEndCmsPagesEdit.ShowInMainMenu = CmsPage.ShowInMainMenu;
                backEndCmsPagesEdit.ShowInBottomMenu = CmsPage.ShowInBottomMenu;
                backEndCmsPagesEdit.ShowInSitemap = CmsPage.ShowInSitemap;
                backEndCmsPagesEdit.IsActive = CmsPage.IsActive;
                backEndCmsPagesEdit.IsAccessRestricted = CmsPage.IsAccessRestricted;
                backEndCmsPagesEdit.IsHomePage = CmsPage.IsHomePage;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndCmsPagesEdit);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult CmsPagesEdit(BackEndCmsPagesEdit backEndCmsPagesEdit, int id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                CmsPages CmsPages = new CmsPages();
                int? result = CmsPages.Edit(id, backEndCmsPagesEdit.PageParentId, backEndCmsPagesEdit.Segment, backEndCmsPagesEdit.PageName, backEndCmsPagesEdit.Target, backEndCmsPagesEdit.PageTemplateId, backEndCmsPagesEdit.Url, backEndCmsPagesEdit.ShowInMainMenu, backEndCmsPagesEdit.ShowInBottomMenu, backEndCmsPagesEdit.ShowInSitemap, backEndCmsPagesEdit.IsActive, backEndCmsPagesEdit.IsAccessRestricted, backEndCmsPagesEdit.IsHomePage);
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
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.PageSegmentAlreadyExists);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndCmsPagesEdit);
        }

        //  /Admin/CmsPagesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult CmsPagesDelete(int deleteId)
        {
            CmsPages CmsPages = new CmsPages();
            switch (CmsPages.Delete(deleteId))
            {
                case 0:
                    PagesLanguages pagesLanguages = new PagesLanguages();
                    pagesLanguages.ForceCache();

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

            return RedirectToAction("CmsPages");
        }

        //  /Admin/CmsPagesMoveUp/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult CmsPagesMoveUp(int postId)
        {
            CmsPages CmsPages = new CmsPages();
            switch (CmsPages.MoveUp(postId))
            {
                case 0:
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

            return RedirectToAction("CmsPages");
        }

        //  /Admin/CmsPagesMoveDown/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult CmsPagesMoveDown(int postId)
        {
            CmsPages CmsPages = new CmsPages();
            switch (CmsPages.MoveDown(postId))
            {
                case 0:
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

            return RedirectToAction("CmsPages");
        }
    }
}
