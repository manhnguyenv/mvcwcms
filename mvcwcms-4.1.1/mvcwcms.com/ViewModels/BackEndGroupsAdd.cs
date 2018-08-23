using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndGroupsAdd
    {
        [DataAnnotationsDisplay("GroupName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string GroupName { get; set; }
    }
}
