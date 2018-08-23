using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace MVCwCMS.Models
{
    public class MediaItem
    {
        public bool IsActive { get; set; }
        public bool IsMainItem { get; set; }
        public string MediaGalleryCode { get; set; }
        public string MediaItemPath { get; set; }
        public int MediaItemId { get; set; }
        public string MediaTypeName { get; set; }
        public int MediaTypeId { get; set; }
        public int Ordering { get; set; }
        public string LanguageCodesTitles { get; set; }
        public bool IsMarkedForDeletion { get; set; }
        public List<MediaItemLanguageTitle> MediaItemLanguagesTitles { get; set; }
    }

    public class MediaItemLanguageTitle
    {
        [DataAnnotationsDisplay("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("MediaItemTitle")]
        public string MediaItemTitle { get; set; }
    }

    public class MediaItems
    {
        private List<MediaItem> _AllItems;

        private List<MediaItem> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<MediaItem>("sp_cms_media_items_select", force, true, OverwriteResult);
        }
        private List<MediaItem> OverwriteResult(List<MediaItem> mediaItems)
        {
            Languages languages = new Languages();
            List<MediaItemLanguageTitle> tempList;
            foreach (MediaItem mediaItem in mediaItems)
            {
                if (mediaItem.LanguageCodesTitles.IsNotEmptyOrWhiteSpace())
                {
                    tempList = (from i in mediaItem.LanguageCodesTitles.Split('|').ToList()
                                select new MediaItemLanguageTitle()
                                {
                                    LanguageCode = i.Split('~')[0],
                                    LanguageName = languages.GetLanguageByCode(i.Split('~')[0]).LanguageName,
                                    MediaItemTitle = i.Split('~')[1]
                                }).ToList();
                }
                else
                {
                    tempList = new List<MediaItemLanguageTitle>();
                }
                mediaItem.MediaItemLanguagesTitles = new List<MediaItemLanguageTitle>();
                mediaItem.MediaItemLanguagesTitles.AddRange(tempList);

                mediaItem.LanguageCodesTitles = string.Empty;
            }
            return mediaItems;
        }

        public MediaItems()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<MediaItem> GetMediaItems(string mediaGalleryCode, bool? isActive = null, int? mediaTypeId = null)
        {
            List<MediaItem> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (isActive.IsNull() || i.IsActive == isActive)
                             && i.MediaGalleryCode.ToLower() == mediaGalleryCode.IfEmpty("").ToLower()
                             && (mediaTypeId.IsNull() || i.MediaTypeId == mediaTypeId)
                          select i).ToList();
            }

            return result;
        }

        public Dictionary<string, string> GetlanguageCodesTitles(List<MediaItem> mediaItems)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var languageCodesTitles = (from i in mediaItems
                                       select new
                                       {
                                           languageCodeTitle = i.LanguageCodesTitles.Split('|')
                                       }).ToList();
            foreach (var i in languageCodesTitles)
            {
                string[] temp = i.languageCodeTitle.ToString().Split('~');
                result.Add(temp[0], temp[1]);
            }
            return result;
        }

        public bool IsFileUsed(string filePath)
        {
            bool result = false;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.MediaItemPath.Contains(filePath, StringComparison.OrdinalIgnoreCase)
                          select i).Count() > 0;
            }

            return result;
        }

        public int? Delete(int mediaItemId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_media_items_delete", "@MediaItemId", mediaItemId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string mediaGalleryCode, string mediaItemPath, int mediaTypeId, bool isActive, string languageCodes)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_media_items_insert",
                                    "@MediaGalleryCode", mediaGalleryCode,
                                    "@MediaItemPath", mediaItemPath,
                                    "@MediaTypeId", mediaTypeId,
                                    "@IsActive", isActive,
                                    "@LanguageCodes", languageCodes,
                                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(int mediaItemId, bool isMainItem, int ordering, bool isActive, string languageCodesMediaTitles)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_media_items_update",
                    "@MediaItemId", mediaItemId,
                    "@IsMainItem", isMainItem,
                    "@Ordering", ordering,
                    "@IsActive", isActive,
                    "@LanguageCodesMediaTitles", languageCodesMediaTitles,
                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}