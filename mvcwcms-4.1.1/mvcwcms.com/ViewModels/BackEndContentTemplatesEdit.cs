using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class BackEndContentTemplatesEdit
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
