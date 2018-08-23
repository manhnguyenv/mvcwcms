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
    public class Language
    {
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
        public string LanguageNameOriginal { get; set; }
        public bool IsActive { get; set; }
    }

    public class Languages
    {
        private List<Language> _AllItems;

        private List<Language> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<Language>("sp_cms_languages_select", force);
        }

        public Languages()
        {
            _AllItems = GetAllItems();
        }

        public List<Language> GetAllLanguages(string languageCode = null, string languageName = null, bool? isActive = null)
        {
            List<Language> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (isActive.IsNull() || i.IsActive == isActive)
                             && (languageCode.IsNull() || i.LanguageCode.Contains(languageCode, StringComparison.OrdinalIgnoreCase))
                             && (languageName.IsNull() || i.LanguageName.Contains(languageName, StringComparison.OrdinalIgnoreCase))
                          select i).ToList();
            }

            return result;
        }

        public Language GetLanguageByCode(string languageCode)
        {
            Language result = null;

            if (_AllItems.IsNotNull() && languageCode.IsNotEmptyOrWhiteSpace())
            {
                result = (from i in _AllItems
                          where i.LanguageCode.ToLower() == languageCode.ToLower()
                          select i).FirstOrDefault();
            }

            return result;
        }

        public int? Delete(string languageCode)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_languages_delete", "@LanguageCode", languageCode, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string languageCode, string languageName, string languageNameOriginal, bool isActive)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_languages_insert", "@LanguageCode", languageCode, "@LanguageName", languageName, "@LanguageNameOriginal", languageNameOriginal, "@IsActive", isActive, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(string currentLanguageCode, string newLanguageCode, string languageName, string languageNameOriginal, bool isActive)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_languages_update", "@CurrentLanguageCode", currentLanguageCode, "@NewLanguageCode", newLanguageCode, "@LanguageName", languageName, "@LanguageNameOriginal", languageNameOriginal, "@IsActive", isActive, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}