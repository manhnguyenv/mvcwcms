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
    public class BackEndMediaGalleriesList
    {
        [DataAnnotationsDisplay("MediaGalleryCode")]
        [DataAnnotationsStringLengthMax(50)]
        public string MediaGalleryCode { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        public List<MediaGallery> MediaGalleryList { get; set; }
    }
}
