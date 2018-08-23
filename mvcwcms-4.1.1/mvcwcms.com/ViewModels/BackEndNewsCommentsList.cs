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
    public class BackEndNewsCommentsList
    {
        public int NewsId { get; set; }

        [DataAnnotationsDisplay("NewsTitle")]
        public string NewsTitle { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool? IsActive { get; set; }

        public List<NewsComment> NewsCommentList { get; set; }
    }
}
