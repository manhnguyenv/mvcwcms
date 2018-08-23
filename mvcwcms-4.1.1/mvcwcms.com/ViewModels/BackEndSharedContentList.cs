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
    public class BackEndSharedContentList
    {
        [DataAnnotationsDisplay("SharedContentCode")]
        [DataAnnotationsOnlyLetters]
        [DataAnnotationsStringLengthMax(255)]
        public string SharedContentCode { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        public List<SharedContent> SharedContentList { get; set; }
    }
}
