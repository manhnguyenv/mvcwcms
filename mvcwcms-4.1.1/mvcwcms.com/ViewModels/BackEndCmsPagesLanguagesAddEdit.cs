using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class BackEndCmsPagesLanguagesAddEdit
    {
        [DataAnnotationsDisplay("PageId")]
        public int? PageId { get; set; }

        [DataAnnotationsDisplay("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("PageName")]
        public string PageName { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("MenuName")]
        public string MenuName { get; set; }

        [DataAnnotationsDisplay("MetaTitle")]
        [DataAnnotationsStringLengthMax(255)]
        public string MetaTagTitle { get; set; }

        [DataAnnotationsDisplay("MetaKeywords")]
        [StringLength(500, ErrorMessageResourceType = typeof(Resources.Strings), ErrorMessageResourceName = "DataAnnotationsStringLengthMax")]
        public string MetaTagKeywords { get; set; }

        [DataAnnotationsDisplay("MetaDescription")]
        [DataAnnotationsStringLengthMax(1000)]
        public string MetaTagDescription { get; set; }

        [DataAnnotationsDisplay("Robots")]
        [DataAnnotationsStringLengthMax(255)]
        public string Robots { get; set; }

        [AllowHtml]
        [DataAnnotationsDisplay("Content")]
        public string HtmlCode { get; set; }
    }
}