using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using MVCwCMS.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MVCwCMS.ViewModels
{
    public class BackEndSharedContentAddEdit
    {
        public string SharedContentCode { get; set; }

        [DataAnnotationsDisplay("SharedContentCode")]
        [DataAnnotationsOnlyLetters]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string NewSharedContentCode { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }

        public List<BackEndSharedContentLanguagesAddEdit> SharedContentLanguages { get; set; }

        public BackEndSharedContentAddEdit()
        {
            SharedContentLanguages = new List<BackEndSharedContentLanguagesAddEdit>();
        }
    }

    public class BackEndSharedContentLanguagesAddEdit
    {
        [DataAnnotationsDisplay("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        public string LanguageName { get; set; }

        [AllowHtml]
        [DataAnnotationsDisplay("Content")]
        public string HtmlCode { get; set; }
    }
}
