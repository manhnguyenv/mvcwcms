using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace MVCwCMS.Models
{
    public class SingleNews
    {
        public int NewsId { get; set; }
        public string LanguageCode { get; set; }
        public DateTime NewsDate { get; set; }
		public string UserName { get; set; }
        public bool IsActive { get; set; }
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
        public bool IsCategoryActive { get; set; }
		public string MainImageFilePath { get; set; }
		public string NewsTitle { get; set; }
        public string NewsContent { get; set; }
    }

    public class NewsArchive
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class News
    {
        private List<SingleNews> _AllItems;

        private List<SingleNews> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<SingleNews>("sp_cms_news_select", force);
        }

        public News()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<SingleNews> GetNews(int newsId)
        {
            List<SingleNews> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.NewsId == newsId
                          select sc).ToList();
            }

            return result;
        }

        public List<SingleNews> GetNews(string languageCode, int? newsId = null, string newsTitle = null, bool? isActive = null, int? categoryId = null, DateTime? newsDate = null, bool? isCategoryActive = null, string newsDateFrom = null, string newsDateTo = null)
        {
            List<SingleNews> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where (isActive.IsNull() || sc.IsActive == isActive)
                             && (newsId.IsNull() || sc.NewsId == newsId)
                             && (isCategoryActive.IsNull() || sc.IsCategoryActive == isCategoryActive)
                             && (newsTitle.IsNull() || sc.NewsTitle.Contains(newsTitle, StringComparison.OrdinalIgnoreCase))
                             && (languageCode.IsEmptyOrWhiteSpace() || sc.LanguageCode.ToLower() == languageCode.ToLower())
                             && (categoryId.IsNull() || sc.CategoryId == categoryId)
                             && (newsDate.IsNull() || sc.NewsDate.IsBetween<DateTime>(newsDate.Value.GetFirstDayOfMonth(), newsDate.Value.GetLastDayOfMonth().AddDays(1)))
                             && (newsDateFrom.IsNull() || sc.NewsDate >= newsDateFrom.ToDateTime())
                             && (newsDateTo.IsNull() || sc.NewsDate <= newsDateTo.ToDateTime())
                          select sc).ToList();
            }

            return result;
        }

        public SingleNews GetSingleNews(int? newsId, string languageCode)
        {
            SingleNews result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.NewsId == newsId
                          && sc.LanguageCode.ToLower() == languageCode.ToLower()
                          select sc).FirstOrDefault();
            }

            return result;
        }

        public bool IsFileUsed(string filePath)
        {
            bool result = false;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.NewsContent.Contains(filePath, StringComparison.OrdinalIgnoreCase)
                          || i.MainImageFilePath == filePath
                          select i).Count() > 0;
            }

            return result;
        }

        public List<NewsArchive> GetNewsArchives()
        {
            List<NewsArchive> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.IsActive
                          && i.IsCategoryActive
                          orderby i.NewsDate descending
                          select new NewsArchive()
                          {
                              Text = i.NewsDate.ToString("MMMM") + " " + i.NewsDate.Year,
                              Value = i.NewsDate.Month + "-" + i.NewsDate.Year
                          }).Distinct(i => i.Value).ToList();
            }

            return result;
        }

        public int? AddEdit(int? newsId, DateTime? newsDate, string userName, bool? isActive, int? categoryId, string mainImageFilePath, string languageCode, string newsTitle, string newsContent, out int? lastInsertedId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                lastInsertedId = db.ExecScalarProc("sp_cms_news_insert_update",
                    "@NewsId", newsId,
                    "@NewsDate", newsDate,
                    "@UserName", userName,
                    "@IsActive", isActive,
                    "@CategoryId", categoryId,
                    "@MainImageFilePath", mainImageFilePath,
                    "@LanguageCode", languageCode,
                    "@NewsTitle", newsTitle,
                    "@NewsContent", newsContent,
                    returnValue).ConvertTo<int?>(null, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);

                return result;
            }
        }

        public int? Delete(int newsId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_news_delete", "@NewsId", newsId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}