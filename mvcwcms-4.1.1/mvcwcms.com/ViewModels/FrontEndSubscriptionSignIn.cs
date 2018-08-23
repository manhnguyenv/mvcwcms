using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCwCMS.ViewModels
{
    public class FrontEndSubscriptionSignIn
    {
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("Email")]
        [EmailAddress]
        [DataAnnotationsRequired]
        public string Email { get; set; }

        [DataAnnotationsDisplay("Password")]
        [DataAnnotationsRequired]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}