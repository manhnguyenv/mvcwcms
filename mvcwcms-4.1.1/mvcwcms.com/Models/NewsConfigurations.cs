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
    public class NewsConfiguration
    {
        public bool IsNewsActive { get; set; }
        public int? NewsPageId { get; set; }
        public int NumberOfNewsInSummary { get; set; }
        public bool IsCommentAutoApproved { get; set; }
    }

    public class NewsConfigurations
    {
        private List<NewsConfiguration> _AllItems;

        private List<NewsConfiguration> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<NewsConfiguration>("sp_cms_news_config_select", force);
        }

        public NewsConfigurations()
        {
            _AllItems = GetAllItems();
        }

        public NewsConfiguration GetNewsConfiguration()
        {
            NewsConfiguration result = null;

            if (_AllItems.IsNotNull())
                result = (from i in _AllItems select i).FirstOrDefault();

            return result;
        }

        public int? Edit(bool isNewsActive, int? newsPageId, int? numberOfNewsInSummary, bool isCommentAutoApproved)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_news_config_update",
                                    "@IsNewsActive", isNewsActive,
                                    "@NewsPageId", newsPageId,
                                    "@NumberOfNewsInSummary", numberOfNewsInSummary,
                                    "@IsCommentAutoApproved", isCommentAutoApproved,
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