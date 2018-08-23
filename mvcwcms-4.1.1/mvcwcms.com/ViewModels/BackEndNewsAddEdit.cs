using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using MVCwCMS.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace MVCwCMS.ViewModels
{
    public class BackEndNewsAddEdit
    {
        public int? NewsId { get; set; }

        [DataAnnotationsDisplay("NewsDate")]
        [DataAnnotationsRequired]
        public string NewsDate { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }

        [DataAnnotationsDisplay("CategoryName")]
        [DataAnnotationsRequired]
        public int CategoryId { get; set; }

        [DataAnnotationsDisplay("MainImageFilePath")]
        public string MainImageFilePath { get; set; }

        public List<BackEndNewsLanguagesAddEdit> NewsLanguages { get; set; }

        public BackEndNewsAddEdit()
        {
            NewsLanguages = new List<BackEndNewsLanguagesAddEdit>();
        }
    }

    public class BackEndNewsLanguagesAddEdit
    {
        [DataAnnotationsDisplay("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("NewsTitle")]
        [DataAnnotationsRequired(resourceName: "DataAnnotationsRequiredAllLanguages")]
        public string NewsTitle { get; set; }

        [AllowHtml]
        [DataAnnotationsDisplay("NewsContent")]
        [DataAnnotationsRequired(resourceName: "DataAnnotationsRequiredAllLanguages")]
        public string NewsContent { get; set; }
    }
}
