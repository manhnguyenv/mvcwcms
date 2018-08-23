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
    public class SubscriptionConfiguration
    {
        public bool IsSubscriptionActive { get; set; }
        public int? RegisterPageId { get; set; }
        public int? SignInPageId { get; set; }
        public int? ForgotPasswordPageId { get; set; }
        public int? ChangePasswordPageId { get; set; }
        public int? ProfilePageId { get; set; }
    }

    public class SubscriptionConfigurations
    {
        private List<SubscriptionConfiguration> _AllItems;

        private List<SubscriptionConfiguration> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<SubscriptionConfiguration>("sp_cms_subscriptions_config_select", force);
        }

        public SubscriptionConfigurations()
        {
            _AllItems = GetAllItems();
        }

        public SubscriptionConfiguration GetSubscriptionConfiguration()
        {
            SubscriptionConfiguration result = null;

            if (_AllItems.IsNotNull())
                result = (from i in _AllItems select i).FirstOrDefault();

            return result;
        }

        public int? Edit(bool isSubscriptionActive, int? registerPageId, int? signInPageId, int? forgotPasswordPageId, int? changePasswordPageId, int? profilePageId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_config_update",
                                    "@IsSubscriptionActive", isSubscriptionActive,
                                    "@RegisterPageId", registerPageId,
                                    "@SignInPageId", signInPageId,
                                    "@ForgotPasswordPageId", forgotPasswordPageId,
                                    "@ChangePasswordPageId", changePasswordPageId,
                                    "@ProfilePageId", profilePageId,
                                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                {
                    _AllItems = GetAllItems(true);
                }
            }

            return result;
        }
    }
}