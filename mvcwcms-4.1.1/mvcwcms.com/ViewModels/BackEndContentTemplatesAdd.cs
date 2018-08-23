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
    public class BackEndContentTemplatesAdd
    {
        [DataAnnotationsDisplay("TemplateName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string Title { get; set; }

        [DataAnnotationsDisplay("TemplateDescription")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string Description { get; set; }

        [AllowHtml]
        [DataAnnotationsDisplay("HtmlCode")]
        [DataAnnotationsRequired]
        public string Content { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }
    }
}
