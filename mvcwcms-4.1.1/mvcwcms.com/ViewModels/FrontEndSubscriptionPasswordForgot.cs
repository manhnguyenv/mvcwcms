using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCwCMS.ViewModels
{
    public class FrontEndSubscriptionPasswordForgot
    {
        [DataAnnotationsDisplay("Email")]
        [EmailAddress]
        [DataAnnotationsRequired]
        public string Email { get; set; }

        public string LanguageCode { get; set; }
    }
}