using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCwCMS.ViewModels
{
    public class BackEndLogin
    {
        [DataAnnotationsDisplay("Username")]
        [DataAnnotationsRequired]
        public string Username { get; set; }

        [DataAnnotationsDisplay("Password")]
        [DataAnnotationsRequired]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}