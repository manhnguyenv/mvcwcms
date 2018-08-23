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
    public class SharedContent
    {
        public string SharedContentCode { get; set; }
        public string LanguageCode { get; set; }
        public bool IsActive { get; set; }
        public string HtmlCode { get; set; }
    }

    public class SharedContents
    {
        private List<SharedContent> _AllItems;

        private List<SharedContent> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<SharedContent>("sp_cms_shared_content_select", force);
        }

        public SharedContents()
        {
            _AllItems = GetAllItems();
        }

        public void ForceCache()
        {
            _AllItems = GetAllItems(true);
        }

        public List<SelectListItem> GetAllSharedContentsAsSelectListItems()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.IsActive
                          select sc.SharedContentCode)
                          .Distinct()
                          .Select(scc => new SelectListItem()
                          {
                              Text = "Shared Content -> " + scc,
                              Value = "{$SharedContent-" + scc + "}"
                          }).ToList();
            }
            return result;
        }

        public List<SharedContent> GetSharedContents(string sharedContentCode = null, bool? isActive = null)
        {
            List<SharedContent> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where (isActive.IsNull() || sc.IsActive == isActive)
                             && (sharedContentCode.IsNull() || sc.SharedContentCode.Contains(sharedContentCode, StringComparison.OrdinalIgnoreCase))
                          select sc).Distinct(i => i.SharedContentCode).ToList();
            }

            return result;
        }

        public SharedContent GetSharedContent(string sharedContentCode, string languageCode, bool? isActive = null)
        {
            SharedContent result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.SharedContentCode == sharedContentCode
                          && sc.LanguageCode == languageCode
                          && (isActive.IsNull() || sc.IsActive == isActive)
                          select sc).FirstOrDefault();
            }

            return result;
        }

        public List<SharedContent> GetSharedContents(string sharedContentCode)
        {
            List<SharedContent> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from sc in _AllItems
                          where sc.SharedContentCode.ToLower() == sharedContentCode.ToLower()
                          select sc).ToList();
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

        public int? AddEdit(string sharedContentCode, string newSharedContentCode, string languageCode, bool isActive, string htmlCode)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                string lastInsertedId = db.ExecScalarProc("sp_cms_shared_content_insert_update",
                    "@SharedContentCode", sharedContentCode,
                    "@NewSharedContentCode", newSharedContentCode,
                    "@LanguageCode", languageCode,
                    "@IsActive", isActive,
                    "@HtmlCode", htmlCode,
                    returnValue).ConvertTo<string>(string.Empty, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);

                return result;
            }
        }

        public int? Delete(string sharedContentCode)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_shared_content_delete", "@SharedContentCode", sharedContentCode, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}