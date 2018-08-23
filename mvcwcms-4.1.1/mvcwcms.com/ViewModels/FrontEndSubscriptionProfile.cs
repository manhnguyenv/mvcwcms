using MVCwCMS.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class FrontEndSubscriptionProfile
    {
        public string LanguageCode { get; set; }

        [DataAnnotationsDisplay("Email")]
        public string Email { get; set; }

        [DataAnnotationsDisplay("FirstName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string FirstName { get; set; }

        [DataAnnotationsDisplay("LastName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string LastName { get; set; }

        [DataAnnotationsDisplay("Birthdate")]
        public string Birthdate { get; set; }

        [DataAnnotationsDisplay("PhoneNumber")]
        [DataAnnotationsStringLengthMax(255)]
        public string PhoneNumber { get; set; }

        [DataAnnotationsDisplay("Address")]
        [DataAnnotationsStringLengthMax(255)]
        public string Address { get; set; }

        [DataAnnotationsDisplay("City")]
        [DataAnnotationsStringLengthMax(255)]
        public string City { get; set; }

        [DataAnnotationsDisplay("PostCode")]
        [DataAnnotationsStringLengthMax(255)]
        public string PostCode { get; set; }

        [DataAnnotationsDisplay("Country")]
        [DataAnnotationsStringLengthMax(2)]
        public string CountryCode { get; set; }

        [DataAnnotationsDisplay("JoinNewsletter")]
        public bool WantsNewsletter { get; set; }

        [DataAnnotationsDisplay("JoinDate")]
        public string JoinDate { get; set; }
    }
}
