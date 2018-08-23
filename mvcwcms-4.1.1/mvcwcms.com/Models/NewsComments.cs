using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace MVCwCMS.Models
{
    public class NewsComment
    {
        public int CommentId { get; set; }
        public int NewsId { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public string SubscriptionEmail { get; set; }
        public string FirstName { get; set; }
    }

    public class NewsComments
    {
        private List<NewsComment> _AllItems;

        private List<NewsComment> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<NewsComment>("sp_cms_news_comments_select", force);
        }

        public NewsComments()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<NewsComment> GetNewsComments(int? newsId = null, bool? isActive = null, string subscriptionEmail = null, string firstName = null)
        {
            List<NewsComment> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where (newsId.IsNull() || sc.NewsId == newsId)
                             && (isActive.IsNull() || sc.IsActive == isActive)
                             && (subscriptionEmail.IsNull() || sc.SubscriptionEmail.Contains(subscriptionEmail, StringComparison.OrdinalIgnoreCase))
                             && (firstName.IsNull() || sc.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase))
                          select sc).ToList();
            }

            return result;
        }

        public NewsComment GetNewsComment(int commentId)
        {
            NewsComment result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.CommentId == commentId
                          select sc).FirstOrDefault();
            }

            return result; 
        }

        public int? Delete(int commentId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_news_comments_delete", "@CommentId", commentId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(int newsId, bool isActive, string comment, DateTime commentDate, string subscriptionEmail)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_news_comments_insert", "@NewsId", newsId, "@IsActive", isActive, "@Comment", comment, "@CommentDate", commentDate, "@SubscriptionEmail", subscriptionEmail, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(int commentId, bool isActive, string comment)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_news_comments_update", "@CommentId", commentId, "@IsActive", isActive, "@Comment", comment, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}