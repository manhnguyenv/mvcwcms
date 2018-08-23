using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MvcPaging;

namespace MVCwCMS.ViewModels
{
    public class FrontEndNews
    {
        public string LanguageCode { get; set; }

        public int? NewsId { get; set; }
        public DateTime? NewsDate { get; set; }
        public string UserName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string MainImageFilePath { get; set; }
        public string NewsTitle { get; set; }
        public string NewsContent { get; set; }

        public int? FilterCategoryId { get; set; }
        public string FilterNewsDate { get; set; }
        public IPagedList<SingleNews> NewsList { get; set; }

        public List<NewsComment> NewsCommentList { get; set; }

        [DataAnnotationsDisplay("NewsComment")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(1000)]
        [DataAnnotationsOnlySafeCharacters]
        public string NewsComment { get; set; }
    }
}