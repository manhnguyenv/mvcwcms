using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MVCwCMS.ViewModels
{
    public class BackEndSubscriptionsList
    {
        [DataAnnotationsDisplay("Email")]
        [DataAnnotationsStringLengthMax(255)]
        public string Email { get; set; }

        [DataAnnotationsDisplay("Status")]
        public int? SubscriptionStatusId { get; set; }

        [DataAnnotationsDisplay("FirstName")]
        [DataAnnotationsStringLengthMax(255)]
        public string FirstName { get; set; }

        [DataAnnotationsDisplay("LastName")]
        [DataAnnotationsStringLengthMax(255)]
        public string LastName { get; set; }

        [DataAnnotationsDisplay("JoinDateFrom")]
        public string JoinDateFrom { get; set; }

        [DataAnnotationsDisplay("JoinDateTo")]
        public string JoinDateTo { get; set; }

        [DataAnnotationsDisplay("Newsletter")]
        public bool? WantsNewsletter { get; set; }

        public List<Subscription> SubscriptionsList { get; set; }
    }
}
