using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using MVCwCMS.Helpers;
using System.Text.RegularExpressions;

namespace MVCwCMS.ViewModels
{
    public class BackEndPageTemplatesAdd
    {
        [DataAnnotationsDisplay("TemplateName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string Title { get; set; }

        [AllowHtml]
        //[CustomValidation(typeof(NoDuplicateModules), "Validate")]
        [DataAnnotationsDisplay("HtmlCode")]
        [DataAnnotationsRequired]
        public string HtmlCode { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }
    }
}
