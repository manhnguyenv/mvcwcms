using MVCwCMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class FrontEndSubscriptionPasswordReset
    {
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("NewPassword")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthRange(6, 255)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Strings), ErrorMessageResourceName = "NewPasswordAndConfirmationPasswordDoNotMatch")]
        [DataAnnotationsDisplay("ConfirmationPassword")]
        [DataAnnotationsRequired]
        public string ConfirmationPassword { get; set; }

        public string PasswordResetKey { get; set; }
    }
}