using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.ModuleConnectors
{
    public class MediaGalleriesConnector : IModuleConnector
    {
        public List<SelectListItem> GetSelectItemList()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            MediaGalleries mediaGalleries = new MediaGalleries();
            result.AddRange(mediaGalleries.GetAllMediaGalleriesAsSelectListItems());

            return result;
        }

        public string GetContent(HtmlHelper htmlHelper, ViewModels.FrontEndCmsPage model, string id)
        {
            StringBuilder result = new StringBuilder();

            MediaGallery mediaGallery = new MediaGalleries().GetMediaGallery(id, model.LanguageCode);
            if (mediaGallery.IsNotNull())
            {
                result.AppendLine("<div class=\"panel panel-default\">");
                result.AppendLine("<div class=\"panel-heading\">");
                result.AppendLine("<h3 class=\"panel-title\">" + mediaGallery.MediaGalleryTitle + "</h3>");
                result.AppendLine("</div>");
                result.AppendLine("<div class=\"panel-body\">");
                MediaItems mediaItems = new MediaItems();
                List<MediaItem> mediaItemList = mediaItems.GetMediaItems(mediaGallery.MediaGalleryCode, true);
                result.AppendLine("<div class=\"galleria\">");
                if (mediaItemList.IsNotNull())
                {
                    string thumbImg;
                    foreach (MediaItem mediaItem in mediaItemList)
                    {
                        thumbImg = "";
                        switch (mediaItem.MediaTypeId)
                        {
                            case 1: //Photo
                                thumbImg = HtmlHelpers.HtmlHelpers.GetThumbFromBigPhoto(mediaItem.MediaItemPath);
                                break;
                            case 2: //YouTube Video
                                thumbImg = HtmlHelpers.HtmlHelpers.GetThumbFromYouTubeVideo(mediaItem.MediaItemPath);
                                                  
                                break;
                        }
                        if (thumbImg.IsNotEmptyOrWhiteSpace())
                        {
                            result.AppendLine("<a href=\"" + mediaItem.MediaItemPath + "\"><img src=\"" + thumbImg + "\" data-title=\"" + GetMediaTitleFromLanguageCode(mediaItem.MediaItemLanguagesTitles, model.LanguageCode) + "\" data-description=\"\" /></a>");
                        }
                    }   
                }
                result.AppendLine("</div>");
                result.AppendLine("</div>");
                result.AppendLine("</div>");
            }

            return result.ToString();
        }

        public bool IsFileUsed(string filePath)
        {
            bool result = false;

            MediaItems mediaItems = new MediaItems();
            if (mediaItems.IsNotNull())
            {
                result = mediaItems.IsFileUsed(filePath);
            }

            return result;
        }

        private string GetMediaTitleFromLanguageCode(List<MediaItemLanguageTitle> mediaItemLanguagesTitles, string languageCode)
        {
            string result = string.Empty;

            foreach (MediaItemLanguageTitle mediaItemLanguageTitle in mediaItemLanguagesTitles)
            {
                if (mediaItemLanguageTitle.LanguageCode.ToLower() == languageCode.ToLower())
                {
                    result = mediaItemLanguageTitle.MediaItemTitle;
                    break;
                }
            }

            return result;
        }
    }
}