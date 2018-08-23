using MVCwCMS.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class BackEndNewsConfigurationEdit
    {
        [DataAnnotationsDisplay("Active")]
        public bool IsNewsActive { get; set; }

        [DataAnnotationsDisplay("NewsPage")]
        public int? NewsPageId { get; set; }

        [DataAnnotationsDisplay("NumberOfNewsInSummary")]
        public int NumberOfNewsInSummary { get; set; }

        [DataAnnotationsDisplay("CommentAutoApproved")]
        public bool IsCommentAutoApproved { get; set; }
    }
}
