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
    public class BackEndNewsList
    {
        [DataAnnotationsDisplay("NewsTitle")]
        public string NewsTitle { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        [DataAnnotationsDisplay("CategoryName")]
        public int? CategoryId { get; set; }

        [DataAnnotationsDisplay("DateFrom")]
        public string NewsDateFrom { get; set; }

        [DataAnnotationsDisplay("DateTo")]
        public string NewsDateTo { get; set; }

        public List<SingleNews> NewsList { get; set; }

        public string FrontEndUrl { get; set; }
    }
}
