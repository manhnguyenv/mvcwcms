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
    public class MediaType
    {
        public int MediaTypeId { get; set; }
        public string MediaTypeName { get; set; }
    }

    public class MediaTypes
    {
        private List<MediaType> _AllItems;

        private List<MediaType> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<MediaType>("sp_cms_media_types_select", force);
        }

        public MediaTypes()
        {
            _AllItems = GetAllItems();
        }

        public List<MediaType> GetMediaTypes()
        {
            List<MediaType> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          orderby i.MediaTypeName
                          select i).ToList();
            }

            return result;
        }
    }
}