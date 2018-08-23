using CsvHelper.Configuration;
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
    public sealed class SubscriptionsMap : CsvClassMap<Subscription>
    {
        public SubscriptionsMap()
        {
            Map(m => m.Email).Name(Resources.Strings.Email);
            Map(m => m.FirstName).Name(Resources.Strings.FirstName);
            Map(m => m.LastName).Name(Resources.Strings.LastName);
            Map(m => m.BirthDate).Name(Resources.Strings.Birthdate);
            Map(m => m.PhoneNumber).Name(Resources.Strings.PhoneNumber);
            Map(m => m.Address).Name(Resources.Strings.Address);
            Map(m => m.City).Name(Resources.Strings.City);
            Map(m => m.PostCode).Name(Resources.Strings.PostCode);
            Map(m => m.CountryCode).Name(Resources.Strings.CountryCode);
            Map(m => m.CountryName).Name(Resources.Strings.CountryName);
            Map(m => m.SubscriptionStatusName).Name(Resources.Strings.Status);
            Map(m => m.WantsNewsletter).Name(Resources.Strings.Newsletter);
            Map(m => m.JoinDate).Name(Resources.Strings.JoinDate);
            Map(m => m.IpAddress).Name(Resources.Strings.IpAddress);
            Map(m => m.Notes).Name(Resources.Strings.Notes);
        }
    }

    public class Subscription
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int SubscriptionStatusId { get; set; }
        public string SubscriptionStatusName { get; set; }
        public bool WantsNewsletter { get; set; }
        public DateTime JoinDate { get; set; }
        public string IpAddress { get; set; }
        public string ActivationKey { get; set; }
        public string PasswordResetKey { get; set; }
        public string Notes { get; set; }
    }

    public class Subscriptions
    {
        private List<Subscription> _AllItems;

        private List<Subscription> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<Subscription>("sp_cms_subscriptions_select", force);
        }

        public Subscriptions()
        {
            _AllItems = GetAllItems();
        }

        public List<Subscription> GetAllSubscriptions(string subscriptionEmail = null, int? subscriptionStatusId = null, string firstName = null, string lastName = null, string joinDateFrom = null, string joinDateTo = null, bool? wantsNewsletter = null)
        {
            List<Subscription> result = new List<Subscription>();

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (subscriptionEmail.IsNull() || i.Email.Contains(subscriptionEmail, StringComparison.OrdinalIgnoreCase))
                          && (subscriptionStatusId.IsNull() || i.SubscriptionStatusId == subscriptionStatusId)
                          && (firstName.IsNull() || i.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase))
                          && (lastName.IsNull() || i.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))
                          && (joinDateFrom.IsNull() || i.JoinDate >= joinDateFrom.ToDateTime())
                          && (joinDateTo.IsNull() || i.JoinDate <= joinDateTo.ToDateTime())
                          && (wantsNewsletter.IsNull() || i.WantsNewsletter == wantsNewsletter)
                          select i).ToList();
            }

            return result;
        }

        public Subscription GetSubscriptionByPasswordResetKey(string passwordResetKey)
        {
            Subscription result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.PasswordResetKey == passwordResetKey
                          select i).FirstOrDefault();
            }

            return result;
        }

        public Subscription GetSubscriptionByEmail(string subscriptionEmail)
        {
            Subscription result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.Email == subscriptionEmail
                          select i).FirstOrDefault();
            }

            return result;
        }

        public Subscription GetSubscriptionByActivationKey(string activationKey)
        {
            Subscription result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.ActivationKey == activationKey
                          select i).FirstOrDefault();
            }

            return result;
        }

        public Subscription GetSubscriptionActiveByEmailAndPassword(string subscriptionEmail, string password)
        {
            Subscription result = null;

            if (_AllItems.IsNotNull())
            {
                string salt = (from i in _AllItems
                               where i.Email.ToLower() == subscriptionEmail.ToLower()
                               select i.Salt).FirstOrDefault();

                string EncryptedPassword = SecurityHelper.EncryptPassword(password, salt);

                result = (from i in _AllItems
                          where i.Email.ToLower() == subscriptionEmail.ToLower() && i.Password == EncryptedPassword
                          && i.SubscriptionStatusId == 2
                          select i).FirstOrDefault();
            }

            return result;
        }

        public int? ActivativateSubscription(string activationKey)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_activation", "@ActivationKey", activationKey, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? ChangePassword(string subscriptionEmail, string salt, string currentPassword, string newPassword)
        {
            int? result;

            string CurrentEncryptedPassword = SecurityHelper.EncryptPassword(currentPassword, salt);
            string NewEncryptedPassword = SecurityHelper.EncryptPassword(newPassword, salt);

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_change_password", "@Email", subscriptionEmail, "@CurrentPassword", CurrentEncryptedPassword, "@NewPassword", NewEncryptedPassword, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Delete(string subscriptionEmail)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_delete", "@Email", subscriptionEmail, returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Add(string subscriptionEmail, string password, string firstName, string lastName, DateTime? birthDate, string phoneNumber, string address, string city, string postCode, string countryCode, int subscriptionStatusId, bool wantsNewsletter, DateTime joinDate, string ipAddress, string activationKey, string notes)
        {
            int? result;

            string salt;
            string encryptedPassword = SecurityHelper.EncryptPassword(password, out salt);

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_insert",
                                    "@Email", subscriptionEmail,
                                    "@Password", encryptedPassword,
                                    "@Salt", salt,
                                    "@FirstName", firstName,
                                    "@LastName", lastName,
                                    "@BirthDate", birthDate,
                                    "@PhoneNumber", phoneNumber,
                                    "@Address", address,
                                    "@City", city,
                                    "@PostCode", postCode,
                                    "@CountryCode", countryCode,
                                    "@SubscriptionStatusId", subscriptionStatusId,
                                    "@WantsNewsletter", wantsNewsletter,
                                    "@JoinDate", joinDate,
                                    "@IpAddress", ipAddress,
                                    "@ActivationKey", activationKey,
                                    "@Notes", notes,
                                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? Edit(string subscriptionEmail, string password, string firstName, string lastName, DateTime? birthDate, string phoneNumber, string address, string city, string postCode, string countryCode, int subscriptionStatusId, bool wantsNewsletter, string notes)
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
                db.ExecNonQueryProc("sp_cms_subscriptions_update",
                                    "@Email", subscriptionEmail,
                                    "@Password", encryptedPassword,
                                    "@Salt", salt,
                                    "@FirstName", firstName,
                                    "@LastName", lastName,
                                    "@BirthDate", birthDate,
                                    "@PhoneNumber", phoneNumber,
                                    "@Address", address,
                                    "@City", city,
                                    "@PostCode", postCode,
                                    "@CountryCode", countryCode,
                                    "@SubscriptionStatusId", subscriptionStatusId,
                                    "@WantsNewsletter", wantsNewsletter,
                                    "@Notes", notes,
                                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? EditProfile(string subscriptionEmail, string firstName, string lastName, DateTime? birthDate, string phoneNumber, string address, string city, string postCode, string countryCode, bool wantsNewsletter)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_profile_update",
                                    "@Email", subscriptionEmail,
                                    "@FirstName", firstName,
                                    "@LastName", lastName,
                                    "@BirthDate", birthDate,
                                    "@PhoneNumber", phoneNumber,
                                    "@Address", address,
                                    "@City", city,
                                    "@PostCode", postCode,
                                    "@CountryCode", countryCode,
                                    "@WantsNewsletter", wantsNewsletter,
                                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public int? EditPasswordResetKey(string subscriptionEmail, string passwordResetKey)
        {
            int? result;

            using (AdoHelper db = new AdoHelper())
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                db.ExecNonQueryProc("sp_cms_subscriptions_pw_resetkey_update",
                                    "@Email", subscriptionEmail,
                                    "@PasswordResetKey", passwordResetKey,
                                    returnValue);
                result = db.GetParamReturnValue(returnValue);
                if (result == 0)
                    _AllItems = GetAllItems(true);
            }

            return result;
        }

        public bool IsPasswordResetKeyValid(string passwordResetKey)
        {
            bool result = false;

            if (_AllItems.IsNotNull())
            {
                result = ((from i in _AllItems
                           where i.PasswordResetKey == passwordResetKey
                           select i).Count() > 0);
            }

            return result;
        }

        public int? ResetPassword(string passwordResetKey, string newPassword)
        {
            int? result = 2;

            Subscription subscription = GetSubscriptionByPasswordResetKey(passwordResetKey);
            if (subscription.IsNotNull())
            {
                string NewEncryptedPassword = SecurityHelper.EncryptPassword(newPassword, subscription.Salt);

                using (AdoHelper db = new AdoHelper())
                {
                    var returnValue = db.CreateParamReturnValue("returnValue");
                    db.ExecNonQueryProc("sp_cms_subscriptions_pw_change_with_resetkey_update",
                                        "@PasswordResetKey", passwordResetKey,
                                        "@NewPassword", NewEncryptedPassword,
                                        returnValue);
                    result = db.GetParamReturnValue(returnValue);
                    if (result == 0)
                        _AllItems = GetAllItems(true);
                }
            }

            return result;
        }

        public int? ChangePassword(Subscription subscription, string currentPassword, string newPassword)
        {
            int? result = 4; //currentPassword doesn't match
            if (subscription.Password == SecurityHelper.EncryptPassword(currentPassword, subscription.Salt))
            {
                result = Edit(subscription.Email,
                              newPassword,
                              subscription.FirstName,
                              subscription.LastName,
                              subscription.BirthDate,
                              subscription.PhoneNumber,
                              subscription.Address,
                              subscription.City,
                              subscription.PostCode,
                              subscription.CountryCode,
                              subscription.SubscriptionStatusId,
                              subscription.WantsNewsletter,
                              subscription.Notes);
            }
            return result;
        }
    }
}