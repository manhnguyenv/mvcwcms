using MVCwCMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndLanguagesAdd
    {
        [DataAnnotationsDisplay("LanguageCode")]
        [DataAnnotationsOnlyLetters]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(2)]
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("LanguageName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string LanguageName { get; set; }

        [DataAnnotationsDisplay("LanguageNameOriginal")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string LanguageNameOriginal { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }
    }
}
