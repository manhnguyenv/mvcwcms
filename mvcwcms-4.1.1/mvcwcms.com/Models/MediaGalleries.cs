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
    public class MediaGallery
    {
        public bool IsActive { get; set; }
        public string MediaGalleryCode { get; set; }
        public DateTime PublishDate { get; set; }
        public string UserName { get; set; }
        public string LanguageCode { get; set; }
        public string MediaGalleryTitle { get; set; }
    }

    public class MediaGalleries
    {
        private List<MediaGallery> _AllItems;

        private List<MediaGallery> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<MediaGallery>("sp_cms_media_galleries_select", force);
        }

        public MediaGalleries()
        {
            _AllItems = GetAllItems();
        }

        public List<MediaGallery> GetMediaGalleries(string mediaGalleryCode = null, bool? isActive = null, string languageCode = null)
        {
            List<MediaGallery> result = null;

            if (_AllItems.IsNotNull())
            {
                if (languageCode.IsEmptyOrWhiteSpace())
                {
                    languageCode = ConfigurationManager.AppSettings["AdminLanguageCode"];
                }

                result = (from i in _AllItems
                          where (isActive.IsNull() || i.IsActive == isActive)
                             && (mediaGalleryCode.IsNull() || i.MediaGalleryCode.Contains(mediaGalleryCode, StringComparison.OrdinalIgnoreCase))
                             && i.LanguageCode.ToLower() == languageCode.ToLower()
                          select i).ToList();
            }

            return result;
        }

        public MediaGallery GetMediaGallery(string mediaGalleryCode, string languageCode = null)
        {
            MediaGallery result = null;

            if (_AllItems.IsNotNull() && mediaGalleryCode.IsNotEmptyOrWhiteSpace())
            {
                if (languageCode.IsEmptyOrWhiteSpace())
                {
                    languageCode = ConfigurationManager.AppSettings["AdminLanguageCode"];
                }

                result = (from i in _AllItems
                          where i.MediaGalleryCode.ToLower() == mediaGalleryCode.ToLower()
                          && i.LanguageCode.ToLower() == languageCode.ToLower()
                          select i).FirstOrDefault();
            }

            return result;
        }

        public List<SelectListItem> GetAllMediaGalleriesAsSelectListItems()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.IsActive
                          select sc.MediaGalleryCode)
                          .Distinct()
                          .Select(scc => new SelectListItem()
                          {
                              Text = "Media Gallery -> " + scc,
                              Value = "{$MediaGallery-" + scc + "}"
                          }).ToList();
            }
            return result;
        }

        public int? Delete(string mediaGalleryCode)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_media_galleries_delete", "@MediaGalleryCode", mediaGalleryCode, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? AddEdit(string curremtMediaGalleryCode, string newMediaGalleryCode, string languageCode, bool isActive, DateTime? publishDate, string userName, string mediaGalleryTitle)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_media_galleries_insert_update",
                    "@CurrentMediaGalleryCode", curremtMediaGalleryCode,
                    "@NewMediaGalleryCode", newMediaGalleryCode,
                    "@LanguageCode", languageCode,
                    "@IsActive", isActive,
                    "@PublishDate", publishDate,
                    "@UserName", userName,
                    "@MediaGalleryTitle", mediaGalleryTitle,
                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}