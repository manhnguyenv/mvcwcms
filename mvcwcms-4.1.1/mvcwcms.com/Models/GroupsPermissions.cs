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
    public enum PermissionCode
    {
        Browse,
        Read,
        Edit,
        Add,
        Delete
    }

    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }

    public class Permission
    {
        public PermissionCode PermissionCode { get; set; }
        public bool PermissionValue { get; set; }
    }

    public class GroupPermission
    {
        public int? GroupId { get; set; }

        public List<Permission> Permissions { get; set; }
    }

    public class Groups
    {
        private List<Group> _AllItems;

        private List<Group> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<Group>("sp_admin_groups_select", force);
        }

        public Groups()
        {
            _AllItems = GetAllItems();
        }

        public List<Group> GetAllGroups(string groupName = null)
        {
            List<Group> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (groupName.IsNull() || i.GroupName.Contains(groupName, StringComparison.OrdinalIgnoreCase))
                          select i).ToList();
            }

            return result;
        }

        public Group GetGroupById(int? id)
        {
            Group result;
            result = (from page in _AllItems
                      where page.GroupId == id
                      select page).FirstOrDefault();
            return result;
        }

        public int? Delete(int groupId)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_groups_delete", "@GroupId", groupId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string groupName)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_groups_insert", "@GroupName", groupName, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(int groupId, string groupName)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_groups_update", "@GroupId", groupId, "@GroupName", groupName, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}