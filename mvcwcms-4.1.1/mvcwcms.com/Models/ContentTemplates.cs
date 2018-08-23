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
    public class ContentTemplate
    {
        public int ContentTemplateId { get; set; }
        public string title { get; set; } //Lower case to match the TinyMCE template plugin
        public string description { get; set; } //Lower case to match the TinyMCE template plugin
        public string content { get; set; } //Lower case to match the TinyMCE template plugin
        public bool IsActive { get; set; }
    }

    public class ContentTemplates
    {
        private static object ThisLock = new object();

        private List<ContentTemplate> _AllItems;

        private List<ContentTemplate> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<ContentTemplate>("sp_cms_content_templates_select", force);
        }

        public ContentTemplates()
        {
            _AllItems = GetAllItems();
        }

        public List<ContentTemplate> GetAllContentTemplates(string title = null, string description = null, bool? isActive = null)
        {
            List<ContentTemplate> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (isActive.IsNull() || i.IsActive == isActive)
                             && (title.IsNull() || i.title.Contains(title, StringComparison.OrdinalIgnoreCase))
                             && (description.IsNull() || i.description.Contains(description, StringComparison.OrdinalIgnoreCase))
                          select i).ToList();
            }

            return result;
        }

        public ContentTemplate GetContentTemplateById(int? contentTemplateId)
        {
            ContentTemplate result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.ContentTemplateId == contentTemplateId
                          select i).FirstOrDefault();
            }

            return result;
        }

        public int? Delete(int contentTemplateId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_content_templates_delete", "@ContentTemplateId", contentTemplateId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string title, string description, string content, bool isActive)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                string lastInsertedId = db.ExecScalarProc("sp_cms_content_templates_insert", "@Title", title.Trim(), "@Description", description.Trim(), "@Content", content, "@IsActive", isActive, returnValue).ConvertTo<string>(string.Empty, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
                
            }

            return result;
        }

        public int? Edit(int contentTemplateId, string title, string description, string content, bool isActive)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                string lastInsertedId = db.ExecScalarProc("sp_cms_content_templates_update", "@ContentTemplateId", contentTemplateId, "@Title", title.Trim(), "@Description", description.Trim(), "@Content", content, "@IsActive", isActive, returnValue).ConvertTo<string>(string.Empty, true);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }        
    }
}