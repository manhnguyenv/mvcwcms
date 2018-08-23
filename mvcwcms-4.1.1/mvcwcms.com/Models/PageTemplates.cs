using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Models
{
    public class PageTemplate
    {
        public int PageTemplateId { get; set; }
        public string Title { get; set; }
        public string HtmlCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class PageTemplates
    {
        private static object ThisLock = new object();

        private List<PageTemplate> _AllItems;

        private List<PageTemplate> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<PageTemplate>("sp_cms_page_templates_select", force);
        }

        public PageTemplates()
        {
            _AllItems = GetAllItems();
        }

        public List<PageTemplate> GetAllPageTemplates(string title = null, bool? isActive = null)
        {
            List<PageTemplate> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (isActive.IsNull() || i.IsActive == isActive)
                             && (title.IsNull() || i.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                          select i).ToList();
            }

            return result;
        }

        public PageTemplate GetPageTemplateById(int? pageTemplateId)
        {
            PageTemplate result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.PageTemplateId == pageTemplateId
                          select i).FirstOrDefault();
            }

            return result;
        }

        public int? Delete(int pageTemplateId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_page_templates_delete", "@PageTemplateId", pageTemplateId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string title, string htmlCode, bool isActive)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                string lastInsertedId = db.ExecScalarProc("sp_cms_page_templates_insert", "@Title", title.Trim(), "@HtmlCode", htmlCode, "@IsActive", isActive, returnValue).ConvertTo<string>(string.Empty, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
                
            }

            return result;
        }

        public int? Edit(int pageTemplateId, string title, string htmlCode, bool isActive)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                string lastInsertedId = db.ExecScalarProc("sp_cms_page_templates_update", "@PageTemplateId", pageTemplateId, "@Title", title, "@HtmlCode", htmlCode, "@IsActive", isActive, returnValue).ConvertTo<string>(string.Empty, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
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
    }
}