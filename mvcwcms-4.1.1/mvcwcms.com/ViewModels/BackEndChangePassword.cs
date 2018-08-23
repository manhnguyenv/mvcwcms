using MVCwCMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndChangePassword
    {
        [DataAnnotationsDisplay("CurrentPassword")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string CurrentPassword { get; set; }

        [DataAnnotationsDisplay("NewPassword")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthRange(6, 255)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Strings), ErrorMessageResourceName = "NewPasswordAndConfirmationPasswordDoNotMatch")]
        [DataAnnotationsDisplay("ConfirmationPassword")]
        [DataAnnotationsRequired]
        public string ConfirmationPassword { get; set; }
    }
}