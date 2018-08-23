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
    public class BackEndMediaGalleriesAddEdit
    {
        public string MediaGalleryCode { get; set; }

        [DataAnnotationsDisplay("MediaGalleryCode")]
        [DataAnnotationsOnlyLetters]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(50)]
        public string NewMediaGalleryCode { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }

        [DataAnnotationsDisplay("PublishDate")]
        [DataAnnotationsRequired]
        public string PublishDate { get; set; }

        public List<BackEndMediaGalleriesLanguagesAddEdit> MediaGalleryLanguages { get; set; }

        public BackEndMediaGalleriesAddEdit()
        {
            MediaGalleryLanguages = new List<BackEndMediaGalleriesLanguagesAddEdit>();
        }
    }

    public class BackEndMediaGalleriesLanguagesAddEdit
    {
        [DataAnnotationsDisplay("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("MediaGalleryTitle")]
        [DataAnnotationsRequired(resourceName: "DataAnnotationsRequiredAllLanguages")]
        public string MediaGalleryTitle { get; set; }
    }
}
