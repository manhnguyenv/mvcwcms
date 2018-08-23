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
    public class BackEndMediaItemsList
    {
        [DataAnnotationsDisplay("MediaGalleryCode")]
        public string MediaGalleryCode { get; set; }

        [DataAnnotationsDisplay("MediaTypeName")]
        public int? MediaTypeId { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        public int? SelectedIsMainItem { get; set; }
        
        public List<MediaItem> MediaItemsList { get; set; }
    }
}
