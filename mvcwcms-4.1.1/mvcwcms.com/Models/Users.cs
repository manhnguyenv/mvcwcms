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
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
    }

    public class Users
    {
        private List<User> _AllItems;

        private List<User> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<User>("sp_admin_users_select", force);
        }

        public Users()
        {
            _AllItems = GetAllItems();
        }

        public List<User> GetAllUsers(string userName = null, string fullName = null, string email = null, int? groupId = null)
        {
            List<User> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (userName.IsNull() || i.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                          && (fullName.IsNull() || i.FullName.Contains(fullName, StringComparison.OrdinalIgnoreCase))
                          && (email.IsNull() || i.Email.Contains(email, StringComparison.OrdinalIgnoreCase))
                          && (groupId.IsNull() || i.GroupId == groupId)
                          select i).ToList();
            }

            return result;
        }

        public User GetUserByUserName(string userName)
        {
            User result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.UserName.ToLower() == userName.ToLower()
                          select i).FirstOrDefault();
            }

            return result;
        }

        public User GetUserByUserNameAndPassword(string userName, string password)
        {
            User result = null;

            if (_AllItems.IsNotNull())
            {
                string userNameSalt = (from i in _AllItems
                                       where i.UserName.ToLower() == userName.ToLower()
                                       select i.Salt).FirstOrDefault();

                string EncryptedPassword = SecurityHelper.EncryptPassword(password, userNameSalt);

                result = (from i in _AllItems
                          where i.UserName.ToLower() == userName.ToLower() && i.Password == EncryptedPassword
                          select i).FirstOrDefault();
            }

            return result;
        }

        public int? ChangePassword(string userName, string salt, string currentPassword, string newPassword)
        {
            int? result;

            string CurrentEncryptedPassword = SecurityHelper.EncryptPassword(currentPassword, salt);
            string NewEncryptedPassword = SecurityHelper.EncryptPassword(newPassword, salt);

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_users_change_password", "@UserName", userName, "@CurrentPassword", CurrentEncryptedPassword, "@NewPassword", NewEncryptedPassword, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    BackEndSessions.CurrentUser.Password = NewEncryptedPassword;
            }

            return result;
        }

        public int? Delete(string userName)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_users_delete", "@UserName", userName, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string username, string password, string fullName, string email, int? groupId)
        {
            int? result;

            string salt;
            string encryptedPassword = SecurityHelper.EncryptPassword(password, out salt);

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_users_insert", "@UserName", username, "@Password", encryptedPassword, "@Salt", salt, "@FullName", fullName, "@Email", email, "@GroupId", groupId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(string username, string password, string fullName, string email, int? groupId)
        {
            int? result;

            string salt = null;
            string encryptedPassword = null;
            if (password.IsNotEmptyOrWhiteSpace())
            {
                encryptedPassword = SecurityHelper.EncryptPassword(password, out salt);
            }

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_admin_users_update", "@UserName", username, "@Password", encryptedPassword, "@Salt", salt, "@FullName", fullName, "@Email", email, "@GroupId", groupId, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }
    }
}