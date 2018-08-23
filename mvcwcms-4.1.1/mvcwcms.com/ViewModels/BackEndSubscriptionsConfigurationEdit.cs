using MVCwCMS.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class BackEndSubscriptionsConfigurationEdit
    {
        [DataAnnotationsDisplay("Active")]
        public bool IsSubscriptionActive { get; set; }

        [DataAnnotationsDisplay("RegistrationPage")]
        public int? RegisterPageId { get; set; }

        [DataAnnotationsDisplay("SignInPage")]
        public int? SignInPageId { get; set; }

        [DataAnnotationsDisplay("PasswordForgotPage")]
        public int? ForgotPasswordPageId { get; set; }

        [DataAnnotationsDisplay("PasswordChangePage")]
        public int? ChangePasswordPageId { get; set; }

        [DataAnnotationsDisplay("ProfilePage")]
        public int? ProfilePageId { get; set; }
    }
}
