using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MVCwCMS.ViewModels
{
    public class BackEndContentTemplateList
    {
        [DataAnnotationsDisplay("TemplateName")]
        [DataAnnotationsStringLengthMax(255)]
        public string Title { get; set; }

        [DataAnnotationsDisplay("TemplateDescription")]
        [DataAnnotationsStringLengthMax(255)]
        public string Description { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        public List<ContentTemplate> ContentTemplateList { get; set; }
    }
}
