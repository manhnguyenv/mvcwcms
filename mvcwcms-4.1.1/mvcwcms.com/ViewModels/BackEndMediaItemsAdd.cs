using MVCwCMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MVCwCMS.ViewModels
{
    public class BackEndMediaItemsAdd
    {
        [DataAnnotationsDisplay("MediaGalleryCode")]
        public string MediaGalleryCode { get; set; }

        [DataAnnotationsDisplay("AllActive")]
        public bool IsAllActive { get; set; }

        [DataAnnotationsDisplay("Photos")]
        public string Photos { get; set; }

        [DataAnnotationsDisplay("YouTubeVideos")]
        public string YouTubeVideos { get; set; }
    }
}
