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
    public class BackEndNewsCategoryAddEdit
    {
        public int? CategoryId { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }

        public List<BackEndNewsCategoryLanguagesAddEdit> NewsCategoryLanguages { get; set; }

        public BackEndNewsCategoryAddEdit()
        {
            NewsCategoryLanguages = new List<BackEndNewsCategoryLanguagesAddEdit>();
        }
    }

    public class BackEndNewsCategoryLanguagesAddEdit
    {
        [DataAnnotationsDisplay("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("CategoryName")]
        [DataAnnotationsRequired(resourceName: "DataAnnotationsRequiredAllLanguages")]
        public string CategoryName { get; set; }
    }
}
