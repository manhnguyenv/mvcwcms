using MVCwCMS.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndNewsCommentsEdit
    {
        public int NewsId { get; set; }

        [DataAnnotationsDisplay("NewsTitle")]
        public string NewsTitle { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }

        [DataAnnotationsDisplay("Comment")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(1000)]
        public string Comment { get; set; }
    }
}
