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
    public class SubscriptionStatus
    {
        public int SubscriptionStatusId { get; set; }
        public string SubscriptionStatusName { get; set; }
    }

    public class SubscriptionStatuses
    {
        private List<SubscriptionStatus> _AllItems;

        private List<SubscriptionStatus> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<SubscriptionStatus>("sp_cms_subscription_statuses_select", force);
        }

        public SubscriptionStatuses()
        {
            _AllItems = GetAllItems();
        }

        public List<SubscriptionStatus> GetAllSubscriptionStatuses(int? subscriptionStatusId = null, string subscriptionStatusName = null)
        {
            List<SubscriptionStatus> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (subscriptionStatusId.IsNull() || i.SubscriptionStatusId == subscriptionStatusId)
                             && (subscriptionStatusName.IsNull() || i.SubscriptionStatusName.Contains(subscriptionStatusName, StringComparison.OrdinalIgnoreCase))
                          select i).ToList();
            }

            return result;
        }

        public SubscriptionStatus GetSubscriptionStatusById(int subscriptionStatusId)
        {
            SubscriptionStatus result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.SubscriptionStatusId == subscriptionStatusId
                          select i).FirstOrDefault();
            }

            return result;
        }
    }
}