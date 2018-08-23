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
    public class BackEndGroupsList
    {
        [DataAnnotationsDisplay("GroupName")]
        [DataAnnotationsStringLengthMax(255)]
        public string GroupName { get; set; }

        public List<Group> GroupList { get; set; }
    }
}
