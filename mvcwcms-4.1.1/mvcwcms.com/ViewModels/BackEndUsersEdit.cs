using MVCwCMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndUsersEdit
    {
        [DataAnnotationsDisplay("Username")]
        public string Username { get; set; }

        [DataAnnotationsDisplay("Password")]
        [DataAnnotationsStringLengthRange(6, 255)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Strings), ErrorMessageResourceName = "PasswordAndConfirmationPasswordDoNotMatch")]
        [DataAnnotationsDisplay("ConfirmationPassword")]
        public string ConfirmationPassword { get; set; }

        [DataAnnotationsDisplay("FullName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string FullName { get; set; }

        [DataAnnotationsDisplay("SubscriptionEmail")]
        [EmailAddress]
        [DataAnnotationsStringLengthMax(255)]
        public string Email { get; set; }

        [DataAnnotationsDisplay("Group")]
        [DataAnnotationsRequired]
        public int? GroupId { get; set; }
    }
}