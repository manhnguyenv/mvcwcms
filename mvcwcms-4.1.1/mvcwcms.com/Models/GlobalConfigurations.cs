using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MVCwCMS.Models
{
    public class GlobalConfiguration
    {
        public string SiteName { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string Robots { get; set; }
        public string NotificationEmail { get; set; }
        public bool IsCanonicalizeActive { get; set; }
        public string HostNameLabel { get; set; }
        public string DomainName { get; set; }
        public string BingVerificationCode { get; set; }
        public string GoogleVerificationCode { get; set; }
        public string GoogleAnalyticsTrackingCode { get; set; }
        public string GoogleSearchCode { get; set; }
        public bool IsOffline { get; set; }
        public string OfflineCode { get; set; }
        public string ServerTimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string DefaultLanguageCode { get; set; }
        public int DefaultErrorPageTemplateId { get; set; }
    }

    public class GlobalConfigurations
    {
        private List<GlobalConfiguration> _AllItems;

        private List<GlobalConfiguration> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<GlobalConfiguration>("sp_cms_global_configuration_select", force);
        }

        public GlobalConfigurations()
        {
            _AllItems = GetAllItems();
        }

        public GlobalConfiguration GetGlobalConfiguration()
        {
            GlobalConfiguration result = null;

            if (_AllItems.IsNotNull())
                result = (from i in _AllItems select i).FirstOrDefault();

            return result;
        }

        public int? Edit(
            string SiteName,
            string MetaTitle,
            string MetaKeywords,
            string MetaDescription,
            string Robots,
            string NotificationEmail,
            bool IsCanonicalizeActive,
            string HostNameLabel,
            string DomainName,
            string BingVerificationCode,
            string GoogleVerificationCode,
            string GoogleAnalyticsTrackingCode,
            string GoogleSearchCode,
            bool IsOffline,
            string OfflineCode,
            string ServerTimeZone,
            string DateFormat,
            string TimeFormat,
            string DefaultLanguageCode,
            int DefaultErrorPageTemplateId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_global_configuration_update",
                    "@SiteName", SiteName,
                    "@MetaTitle", MetaTitle,
                    "@MetaKeywords", MetaKeywords,
                    "@MetaDescription", MetaDescription,
                    "@Robots", Robots,
                    "@NotificationEmail", NotificationEmail,
                    "@IsCanonicalizeActive", IsCanonicalizeActive,
                    "@HostNameLabel", HostNameLabel,
                    "@DomainName", DomainName,
                    "@BingVerificationCode", BingVerificationCode,
                    "@GoogleVerificationCode", GoogleVerificationCode,
                    "@GoogleAnalyticsTrackingCode", GoogleAnalyticsTrackingCode,
                    "@GoogleSearchCode", GoogleSearchCode,
                    "@IsOffline", IsOffline,
                    "@OfflineCode", OfflineCode,
                    "@ServerTimeZone", ServerTimeZone,
                    "@DateFormat", DateFormat,
                    "@TimeFormat", TimeFormat,
                    "@DefaultLanguageCode", DefaultLanguageCode,
                    "@DefaultErrorPageTemplateId", DefaultErrorPageTemplateId,
                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                {
                    _AllItems = GetAllItems(true);
                    RegisterEmailHelper();
                }
            }

            return result;
        }

        ///// <summary>
        ///// Registers the default values for the EmailHelper class
        ///// </summary>
        public static void RegisterEmailHelper()
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            EmailHelper.DefaultFrom = "\"" + globalConfiguration.SiteName + "\"<" + globalConfiguration.NotificationEmail + ">";
            EmailHelper.DefaultTo = globalConfiguration.NotificationEmail;
        }
    }
}

