using MVCwCMS;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCwCMS.HtmlHelpers
{
    public static class HtmlHelpers
    {
        public static string CountryListTitle(string commaSeparatedCountries)
        {
            string result = string.Empty;
            if (commaSeparatedCountries.IsNotEmptyOrWhiteSpace())
            {
                GeoIpCountries countries = new GeoIpCountries();

                List<string> countryList = commaSeparatedCountries.Split(',').ToList();
                foreach (string country in countryList)
                {
                    result += countries.GetCountryByCode(country).CountryName + ", ";
                }
                result = result.Remove(result.Length - 2);
            }
            return result;
        }

        public static string CountryListTooltip(string commaSeparatedCountries)
        {
            string result = string.Empty;
            if (commaSeparatedCountries.IsNotEmptyOrWhiteSpace())
            {
                GeoIpCountries countries = new GeoIpCountries();

                List<string> countryList = commaSeparatedCountries.Split(',').ToList();
                foreach (string country in countryList)
                {
                    result += "<span title=\"" + countries.GetCountryByCode(country).CountryName + "\" data-toggle=\"tooltip\">" + country + "</span> ";
                }
            }
            return result;
        }

        public static IHtmlString CountryListTooltip(this HtmlHelper htmlHelper, string commaSeparatedCountries)
        {
            string result = CountryListTooltip(commaSeparatedCountries);
            return htmlHelper.Raw(result);
        }

        public static string FilePathPreview(string filePath)
        {
            string result = string.Empty;
            if (filePath.ToLower().EndsWith(".swf"))
            {
                result = "<div class=\"filepath-preview\" title=\"" + filePath + "\"></div>";
            }
            else if (filePath.ToLower().EndsWith(".jpg") || filePath.ToLower().EndsWith(".jpeg") || filePath.ToLower().EndsWith(".gif") || filePath.ToLower().EndsWith(".png"))
            {
                string photoThumb = GetThumbFromBigPhoto(filePath);
                result = "<img src=\"" + photoThumb + "\" height=\"70\" alt=\"Preview\" title=\"<img src='" + filePath + "'/>\" data-toggle=\"tooltip\" />";
            }
            else if (filePath.ToLower().Contains("youtube.com") || filePath.ToLower().Contains("youtu.be"))
            {
                string youTubeVideoThumb = GetThumbFromYouTubeVideo(filePath);
                if (youTubeVideoThumb.IsNotEmptyOrWhiteSpace())
                {
                    result = "<img src=\"" + youTubeVideoThumb + "\" height=\"70\" alt=\"Preview\" title=\"<img src='" + youTubeVideoThumb + "'/>\" data-toggle=\"tooltip\" />";
                }
            }
            else if (filePath.IsNotEmptyOrWhiteSpace())
            {
                result = "<a href=\"" + filePath + "\" target=\"_blank\"><i class=\"fa fa-external-link fa-5x\"></i></a>";
            }
            else
            {
                result = "";
            }
            return result;
        }

        public static IHtmlString FilePathPreview(this HtmlHelper htmlHelper, string filePath)
        {
            string result = filePath.StartsWith("{$") ? filePath.Substring(2) : FilePathPreview(filePath); //If it starts with {$ then it's a Code else it's an Image/Flash
            return htmlHelper.Raw(result);
        }

        public static string GetThumbFromBigPhoto(string bigPhotoUrl)
        {
            return bigPhotoUrl.Replace("Images/", "_thumbs/Images/");
        }

        public static string GetThumbFromYouTubeVideo(string youTubeVideoUrl)
        {
            string result = string.Empty;

            string videoId = string.Empty;
            if (youTubeVideoUrl.ToLower().Contains("youtube.com"))
            {
                Uri uri = new Uri(youTubeVideoUrl);
                if (uri.Query.IsNotEmptyOrWhiteSpace())
                {
                    videoId = HttpUtility.ParseQueryString(uri.Query).Get("v");
                }
            }
            else
            {
                videoId = youTubeVideoUrl.Replace("https://youtu.be/", "");
            }

            if (videoId.IsNotEmptyOrWhiteSpace())
            {
                result = "//img.youtube.com/vi/" + videoId + "/0.jpg";
            }

            return result;
        }
    }
}
