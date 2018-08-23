using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class BackEndPageTemplatesEdit
    {
        [DataAnnotationsDisplay("Title")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string Title { get; set; }

        [AllowHtml]
        [DataAnnotationsDisplay("HtmlCode")]
        [DataAnnotationsRequired]
        public string HtmlCode { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }
    }
}
