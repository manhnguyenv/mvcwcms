using MVCwCMS.Models;
using System.Web.Mvc;
using System.Linq;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using MVCwCMS.Filters;
using System;
using MVCwCMS.Helpers;
using System.Configuration;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/MediaItems/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult MediaItems(BackEndMediaItemsList backEndMediaItemsList, string id)
        {
            backEndMediaItemsList.MediaGalleryCode = id;

            MediaGallery mediaGallery = new MediaGalleries().GetMediaGallery(id);
            if (mediaGallery.IsNotNull())
            {
                MediaItems mediaItems = new MediaItems();
                backEndMediaItemsList.MediaItemsList = mediaItems.GetMediaItems(id, backEndMediaItemsList.IsActive, backEndMediaItemsList.MediaTypeId);
                if (backEndMediaItemsList.MediaItemsList.IsNull() || backEndMediaItemsList.MediaItemsList.Count == 0)
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
                }
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndMediaItemsList);
        }

        //  /Admin/MediaItemsAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult MediaItemsAdd(string id)
        {
            BackEndMediaItemsAdd backEndMediaItemsAdd = new BackEndMediaItemsAdd()
            {
                MediaGalleryCode = id
            };

            MediaGallery mediaGallery = new MediaGalleries().GetMediaGallery(id);
            if (mediaGallery.IsNull())
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndMediaItemsAdd);
        }
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult MediaItemsAdd(BackEndMediaItemsAdd backEndMediaItemsAdd, string id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                List<string> multiFilePath;
                if (backEndMediaItemsAdd.Photos.IsNotEmptyOrWhiteSpace())
                {
                    multiFilePath = backEndMediaItemsAdd.Photos.Split(',').ToList().Select(s => s.Trim()).ToList();
                    ProcessMultiFilePath(multiFilePath, 1, backEndMediaItemsAdd);
                }

                if (backEndMediaItemsAdd.YouTubeVideos.IsNotEmptyOrWhiteSpace())
                {
                    multiFilePath = backEndMediaItemsAdd.YouTubeVideos.Split(',').ToList().Select(s => s.Trim()).ToList();
                    ProcessMultiFilePath(multiFilePath, 2, backEndMediaItemsAdd);
                }

                //Do not use ModelState.Clear(); to avoid losing all the previous results
                ModelState.Remove("IsAllActive");
                ModelState.Remove("Photos");
                ModelState.Remove("YouTubeVideos");
                backEndMediaItemsAdd = new BackEndMediaItemsAdd()
                {
                    MediaGalleryCode = id
                };
            }

            return View(backEndMediaItemsAdd);
        }
        private void ProcessMultiFilePath(List<string> multiFilePath, int mediaTtpeId, BackEndMediaItemsAdd backEndMediaItemsAdd)
        {
            MediaItems mediaItems = new MediaItems();

            string allActiveLanguages = new Languages().GetAllLanguages(isActive: true).Select(i => i.LanguageCode).ToCSV('|');

            foreach (string filePath in multiFilePath)
            {
                int? result = mediaItems.Add(backEndMediaItemsAdd.MediaGalleryCode, filePath, mediaTtpeId, backEndMediaItemsAdd.IsAllActive, allActiveLanguages);
                switch (result)
                {
                    case 0:
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded + ": " + filePath);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_MediaGalleries.MediaItemAlreadyExists + ": " + filePath);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + ": " + filePath);
                        break;
                }
            }
        }

        //  /Admin/MediaItemsEdit/
        [HttpPost]
        [IsRestricted]
        [ExportModelStateToTempData]
        [ValidateAntiForgeryToken]
        public ActionResult MediaItemsEdit(BackEndMediaItemsList backEndMediaItemsList, string id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                MediaItems mediaItems = new MediaItems();
                foreach (MediaItem mediaItem in backEndMediaItemsList.MediaItemsList)
                {
                    if (mediaItem.IsMarkedForDeletion)
                    {
                        switch (mediaItems.Delete(mediaItem.MediaItemId))
                        {
                            case 0:
                                ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted + ": " + mediaItem.MediaItemId);
                                break;
                            case 2:
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist + ": " + mediaItem.MediaItemId);
                                break;
                            case 3:
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemUsedSomewhereElse + ": " + mediaItem.MediaItemId);
                                break;
                            default:
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + ": " + mediaItem.MediaItemId);
                                break;
                        }
                    }
                    else
                    {
                        switch (mediaItems.Edit(mediaItem.MediaItemId, (backEndMediaItemsList.SelectedIsMainItem == mediaItem.MediaItemId), mediaItem.Ordering, mediaItem.IsActive, GetLanguageCodesMediaTitles(mediaItem.MediaItemLanguagesTitles)))
                        {
                            case 0:
                                ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited + ": " + mediaItem.MediaItemId);
                                break;
                            case 2:
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist + ": " + mediaItem.MediaItemId);
                                break;
                            default:
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + ": " + mediaItem.MediaItemId);
                                break;
                        }
                    }
                }
                
            }

            return RedirectToAction("MediaItems", new { id = id });
        }
        private string GetLanguageCodesMediaTitles(List<MediaItemLanguageTitle> mediaItemLanguagesTitles)
        {
            string result = string.Empty;

            foreach (MediaItemLanguageTitle mediaItemLanguageTitle in mediaItemLanguagesTitles)
            {
                result += mediaItemLanguageTitle.LanguageCode + "~" + mediaItemLanguageTitle.MediaItemTitle + "|";
            }

            return result.TrimEnd(new char[] { '|' });
        }
    }
}
