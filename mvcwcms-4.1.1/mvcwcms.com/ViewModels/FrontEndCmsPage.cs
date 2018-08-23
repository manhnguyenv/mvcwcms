using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class FrontEndCmsPage
    {
        public int? PageId { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageFolder { get; set; }
        public string Parameter { get; set; }
        public string MetaTagTitle { get; set; }
        public string MetaTagKeywords { get; set; }
        public string MetaTagDescription { get; set; }
        public string Robots { get; set; }
        public int? PageTemplateId { get; set; }
        public int? StatusCode { get; set; }
    }
}