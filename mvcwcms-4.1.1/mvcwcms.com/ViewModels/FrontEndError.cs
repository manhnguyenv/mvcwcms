using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class FrontEndError
    {
        public string LanguageFolder { get; set; }
        public int StatusCode { get; set; }
        public string ErroMessage { get; set; }
    }
}