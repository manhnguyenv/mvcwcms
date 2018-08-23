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
    public class BackEndUsersList
    {
        [DataAnnotationsDisplay("Username")]
        [DataAnnotationsStringLengthMax(255)]
        public string Username { get; set; }

        [DataAnnotationsDisplay("FullName")]
        [DataAnnotationsStringLengthMax(255)]
        public string FullName { get; set; }

        [DataAnnotationsDisplay("SubscriptionEmail")]
        [DataAnnotationsStringLengthMax(255)]
        public string Email { get; set; }

        [DataAnnotationsDisplay("Group")]
        public int? GroupId { get; set; }

        public List<User> UserList { get; set; }
    }
}
