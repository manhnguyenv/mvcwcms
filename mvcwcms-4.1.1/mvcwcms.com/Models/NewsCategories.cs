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
    public class NewsCategory
    {
        public int CategoryId { get; set; }
        public string LanguageCode { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
    }

    public class NewsCategories
    {
        private List<NewsCategory> _AllItems;

        private List<NewsCategory> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<NewsCategory>("sp_cms_news_categories_select", force);
        }

        public NewsCategories()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<NewsCategory> GetNewsCategories(int? categoryId)
        {
            List<NewsCategory> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.CategoryId == categoryId
                          select sc).ToList();
            }

            return result;
        }

        public List<NewsCategory> GetNewsCategories(string categoryName = null, bool? isActive = null)
        {
            List<NewsCategory> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          orderby sc.CategoryName
                          where sc.LanguageCode.ToLower() == ConfigurationManager.AppSettings["AdminLanguageCode"].ToLower()
                             && (isActive.IsNull() || sc.IsActive == isActive)
                             && (categoryName.IsNull() || sc.CategoryName.Contains(categoryName, StringComparison.OrdinalIgnoreCase))
                          select sc).ToList();
            }

            return result;
        }

        public NewsCategory GetNewsCategory(int? categoryId, string languageCode, bool? isActive = null)
        {
            NewsCategory result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.CategoryId == categoryId
                          && sc.LanguageCode == languageCode
                          && (isActive.IsNull() || sc.IsActive == isActive)
                          select sc).FirstOrDefault();
            }

            return result;
        }

        public int? AddEdit(int? categoryId, string languageCode, bool isActive, string categoryName, out int? lastInsertedId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                lastInsertedId = db.ExecScalarProc("sp_cms_news_categories_insert_update",
                    "@CategoryId", categoryId,
                    "@LanguageCode", languageCode,
                    "@IsActive", isActive,
                    "@CategoryName", categoryName,
                    returnValue).ConvertTo<int?>(null, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);

                return result;
            }
        }

        public int? Delete(int categoryId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_news_categories_delete", "@CategoryId", categoryId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}