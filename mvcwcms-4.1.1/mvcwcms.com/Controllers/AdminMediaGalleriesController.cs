using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using MVCwCMS.Filters;
using System;
using MVCwCMS.Helpers;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/MediaGalleries/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult MediaGalleries(BackEndMediaGalleriesList backEndMediaGalleriesList)
        {
            MediaGalleries mediaGalleries = new MediaGalleries();
            backEndMediaGalleriesList.MediaGalleryList = mediaGalleries.GetMediaGalleries(backEndMediaGalleriesList.MediaGalleryCode, backEndMediaGalleriesList.IsActive);
            if (backEndMediaGalleriesList.MediaGalleryList.IsNull() || backEndMediaGalleriesList.MediaGalleryList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndMediaGalleriesList);
        }

        // /Admin/MediaGalleriesAddEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult MediaGalleriesAddEdit(string id)
        {
            BackEndMediaGalleriesAddEdit backEndMediaGalleries = new BackEndMediaGalleriesAddEdit();

            MediaGalleries mediaGalleries = new MediaGalleries();
            MediaGallery mediaGallery;

            BackEndMediaGalleriesLanguagesAddEdit backEndMediaGalleriesLanguages;

            List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
            if (allActiveLanguages.IsNotNull() && allActiveLanguages.Count > 0)
            {
                if (id.IsNotEmptyOrWhiteSpace())
                {
                    List<MediaGallery> backEndMediaGalleryList = mediaGalleries.GetMediaGalleries(id);
                    if (backEndMediaGalleryList.IsNotNull() && backEndMediaGalleryList.Count > 0)
                    {
                        backEndMediaGalleries.MediaGalleryCode = id;
                        backEndMediaGalleries.NewMediaGalleryCode = id;

                        foreach (Language language in allActiveLanguages)
                        {
                            backEndMediaGalleriesLanguages = new BackEndMediaGalleriesLanguagesAddEdit();
                            backEndMediaGalleriesLanguages.LanguageCode = language.LanguageCode;
                            backEndMediaGalleriesLanguages.LanguageName = language.LanguageName;

                            mediaGallery = mediaGalleries.GetMediaGallery(id, language.LanguageCode);
                            if (mediaGallery.IsNotNull())
                            {
                                backEndMediaGalleries.IsActive = mediaGallery.IsActive;
                                backEndMediaGalleries.PublishDate = mediaGallery.PublishDate.ToDateTimeString();

                                backEndMediaGalleriesLanguages.MediaGalleryTitle = mediaGallery.MediaGalleryTitle;
                            }

                            backEndMediaGalleries.MediaGalleryLanguages.Add(backEndMediaGalleriesLanguages);
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
                    backEndMediaGalleries.PublishDate = DateTime.Now.ToDateTimeString();

                    foreach (Language language in allActiveLanguages)
                    {
                        backEndMediaGalleriesLanguages = new BackEndMediaGalleriesLanguagesAddEdit();
                        backEndMediaGalleriesLanguages.LanguageCode = language.LanguageCode;
                        backEndMediaGalleriesLanguages.LanguageName = language.LanguageName;

                        backEndMediaGalleries.MediaGalleryLanguages.Add(backEndMediaGalleriesLanguages);
                    }
                }
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
            }

            return View(backEndMediaGalleries);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult MediaGalleriesAddEdit(BackEndMediaGalleriesAddEdit backEndMediaGalleries)
        {
            if (ModelState.IsValidOrRefresh())
            {
                MediaGalleries mediaGalleries = new MediaGalleries();
                int? result;
                bool isLoopSuccessful = true;
                foreach (BackEndMediaGalleriesLanguagesAddEdit backEndMediaGalleriesLanguages in backEndMediaGalleries.MediaGalleryLanguages)
                {
                    result = mediaGalleries.AddEdit(backEndMediaGalleries.MediaGalleryCode,
                                                    backEndMediaGalleries.NewMediaGalleryCode,
                                                    backEndMediaGalleriesLanguages.LanguageCode,
                                                    backEndMediaGalleries.IsActive,
                                                    backEndMediaGalleries.PublishDate.ToDateTime(),
                                                    BackEndSessions.CurrentUser.UserName,
                                                    backEndMediaGalleriesLanguages.MediaGalleryTitle);
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
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_MediaGalleries.MediaGalleryCodeAlreadyExists);
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
                    if (backEndMediaGalleries.MediaGalleryCode.IsEmptyOrWhiteSpace())
                    {
                        ModelState.Clear();
                        backEndMediaGalleries = new BackEndMediaGalleriesAddEdit();
                        backEndMediaGalleries.PublishDate = DateTime.Now.ToDateTimeString();
                        BackEndMediaGalleriesLanguagesAddEdit backEndMediaGalleriesLanguages;
                        List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
                        foreach (Language language in allActiveLanguages)
                        {
                            backEndMediaGalleriesLanguages = new BackEndMediaGalleriesLanguagesAddEdit();
                            backEndMediaGalleriesLanguages.LanguageCode = language.LanguageCode;
                            backEndMediaGalleriesLanguages.LanguageName = language.LanguageName;

                            backEndMediaGalleries.MediaGalleryLanguages.Add(backEndMediaGalleriesLanguages);
                        }

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    }
                }
            }

            return View(backEndMediaGalleries);
        }

        //  /Admin/MediaGalleriesDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult MediaGalleriesDelete(string deleteId)
        {
            MediaGalleries mediaGalleries = new MediaGalleries();
            switch (mediaGalleries.Delete(deleteId))
            {
                case 0:
                    MediaItems mediaItems = new MediaItems();
                    mediaItems.ForceCache();

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

            return RedirectToAction("MediaGalleries");
        }
    }
}
