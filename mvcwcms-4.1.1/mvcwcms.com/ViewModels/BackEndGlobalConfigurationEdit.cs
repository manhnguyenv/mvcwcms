using MVCwCMS.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCwCMS.ViewModels
{
    public class BackEndGlobalConfigurationEdit
    {
        [DataAnnotationsDisplay("SiteName")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string SiteName { get; set; }

        [DataAnnotationsDisplay("MetaTitle")]
        [DataAnnotationsStringLengthMax(255)]
        public string MetaTitle { get; set; }

        [DataAnnotationsDisplay("MetaKeywords")]
        [DataAnnotationsStringLengthMax(500)]
        public string MetaKeywords { get; set; }

        [DataAnnotationsDisplay("MetaDescription")]
        [DataAnnotationsStringLengthMax(1000)]
        public string MetaDescription { get; set; }

        [DataAnnotationsDisplay("Robots")]
        [DataAnnotationsStringLengthMax(255)]
        public string Robots { get; set; }

        [DataAnnotationsDisplay("NotificationEmail")]
        [DataAnnotationsEmailAddress]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string NotificationEmail { get; set; }

        [DataAnnotationsDisplay("Canonicalize")]
        public bool IsCanonicalizeActive { get; set; }

        [DataAnnotationsDisplay("HostNameLabel")]
        [DataAnnotationsStringLengthMax(20)]
        public string HostNameLabel { get; set; }

        [DataAnnotationsDisplay("DomainName")]
        [DataAnnotationsDomainName]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string DomainName { get; set; }

        [DataAnnotationsDisplay("BingVerificationCode")]
        [DataAnnotationsStringLengthMax(1000)]
        public string BingVerificationCode { get; set; }

        [DataAnnotationsDisplay("GoogleVerificationCode")]
        [DataAnnotationsStringLengthMax(1000)]
        public string GoogleVerificationCode { get; set; }

        [AllowHtml]
        [DataAnnotationsDisplay("StatsScript")]
        public string GoogleAnalyticsTrackingCode { get; set; }

        [DataAnnotationsDisplay("GoogleSearchCode")]
        [DataAnnotationsStringLengthMax(1000)]
        public string GoogleSearchCode { get; set; }

        [DataAnnotationsDisplay("SiteOffline")]
        public bool IsOffline { get; set; }

        [DataAnnotationsDisplay("OfflineCode")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string OfflineCode { get; set; }

        [DataAnnotationsDisplay("ServerTimeZone")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string ServerTimeZone { get; set; }

        [DataAnnotationsDisplay("DateFormat")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(20)]
        public string DateFormat { get; set; }

        [DataAnnotationsDisplay("TimeFormat")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(20)]
        public string TimeFormat { get; set; }

        [DataAnnotationsDisplay("DefaultLanguage")]
        [DataAnnotationsRequired]
        public string DefaultLanguageCode { get; set; }

        [DataAnnotationsDisplay("DefaultErrorPageTemplate")]
        [DataAnnotationsRequired]
        public int DefaultErrorPageTemplateId { get; set; }
    }
}
