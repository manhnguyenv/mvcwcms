using MVCwCMS.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndGroupsEdit
    {
        [DataAnnotationsDisplay("GroupName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string GroupName { get; set; }
    }
}
