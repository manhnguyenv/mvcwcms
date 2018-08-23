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
    public class PageLanguage
    {
        public int? PageId { get; set; }
        public string LanguageCode { get; set; }
        public string MenuName { get; set; }
        public string MetaTagTitle { get; set; }
        public string MetaTagKeywords { get; set; }
        public string MetaTagDescription { get; set; }
        public string Robots { get; set; }
        public string HtmlCode { get; set; }
    }

    public class PagesLanguages
    {
        private List<PageLanguage> _AllItems;

        private List<PageLanguage> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<PageLanguage>("sp_cms_pages_languages_select", force);
        }

        public PagesLanguages()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<PageLanguage> GetAllPagesLanguages()
        {
            return _AllItems;
        }

        public PageLanguage GetPageLanguage(int? pageId, string languageCode)
        {
            PageLanguage result = null;
            if (_AllItems.IsNotNull())
            {
                result = (from page in _AllItems
                          where page.PageId == pageId && page.LanguageCode == languageCode
                          select page).FirstOrDefault();
            }
            return result;
        }

        public bool IsFileUsed(string filePath)
        {
            bool result = false;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.HtmlCode.Contains(filePath, StringComparison.OrdinalIgnoreCase)
                          select i).Count() > 0;
            }

            return result;
        }

        public int? AddEdit(int? pageId, string languageCode, string menuName, string metaTagTitle, string metaTagKeywords, string metaTagDescription, string robots, string htmlCode)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                string lastInsertedId = db.ExecScalarProc("sp_cms_pages_languages_insert_update",
                    "@PageId", pageId,
                    "@LanguageCode", languageCode,
                    "@MenuName", menuName,
                    "@MetaTagTitle", metaTagTitle,
                    "@MetaTagKeywords", metaTagKeywords,
                    "@MetaTagDescription", metaTagDescription,
                    "@Robots", robots,
                    "@HtmlCode", htmlCode,
                    returnValue).ConvertTo<string>(string.Empty, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);

                return result;
            }
        }

    }
}