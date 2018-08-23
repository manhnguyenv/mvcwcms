using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCwCMS.ViewModels
{
    public class FrontEndContact
    {
        [DataAnnotationsDisplay("FullName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string FullName { get; set; }

        [DataAnnotationsDisplay("CompanyName")]
        [DataAnnotationsStringLengthMax(255)]
        public string CompanyName { get; set; }

        [DataAnnotationsDisplay("PhoneNumber")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string PhoneNumber { get; set; }

        [DataAnnotationsDisplay("Email")]
        [EmailAddress]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string Email { get; set; }

        [DataAnnotationsDisplay("Message")]
        [DataAnnotationsRequired]
        public string Message { get; set; }

        /// <summary>
        /// Honey pot to avoid spammers
        /// </summary>
        [DataAnnotationsDisplay("Notes")]
        public string Notes { get; set; }
    }
}