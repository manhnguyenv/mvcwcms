using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCwCMS.ViewModels
{
    public class BackEndCmsPagesList
    {
        [DataAnnotationsDisplay("PageName")]
        [DataAnnotationsStringLengthMax(255)]
        public string PageName { get; set; }

        public HtmlString TreeTablePageList { get; set; }
    }
}