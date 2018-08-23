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
    public class BackEndLanguagesList
    {
        [DataAnnotationsDisplay("LanguageCode")]
        [DataAnnotationsStringLengthMax(2)]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        [DataAnnotationsStringLengthMax(255)]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        public List<Language> LanguageList { get; set; }
    }
}
