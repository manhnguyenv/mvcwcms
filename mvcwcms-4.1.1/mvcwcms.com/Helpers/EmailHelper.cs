using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace MVCwCMS.Helpers
{
    public class EmailHelper : MailMessage
    {
        private bool _isInitValid = true;

        public static string DefaultFrom { get; set; }
        public static string DefaultTo { get; set; }

        /// <summary>
        /// Instantiates a MailMessage object using default From and To
        /// </summary>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body that can contain HTML tags</param>
        public EmailHelper(string Subject, string Body)
        {
            EmailInit(DefaultFrom, DefaultTo, Subject, Body);
        }

        /// <summary>
        /// Instantiates a MailMessage object
        /// </summary>
        /// <param name="from">A valid email address. If null it will use DefaultFrom</param>
        /// <param name="to">A comma separated list of valid email addresses. If null it will use DefaultTo</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body that can contain HTML tags</param>
        public EmailHelper(string from, string to, string Subject, string Body)
        {
            if (string.IsNullOrEmpty(from))
                from = DefaultFrom;
            if (string.IsNullOrEmpty(to))
                to = DefaultTo;
            EmailInit(from, to, Subject, Body);
        }

        /// <summary>
        /// Sends the configured email
        /// </summary>
        /// <returns>Returns true if the operation is successful</returns>
        public bool Send()
        {
            bool result = _isInitValid;
            if (_isInitValid)
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Send(this);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                    result = false;
                }
            }
            return result;
        }

        private void EmailInit(string From, string To, string Subject, string Body)
        {
            try
            {
                base.From = new MailAddress(From);
                foreach (string emailTo in To.Split(','))
                {
                    base.To.Add(new MailAddress(emailTo.Trim()));
                }
                base.Subject = Subject;
                base.Body = Body;
                base.IsBodyHtml = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                _isInitValid = false;
            }
        }

        /// <summary>
        /// Validates an email address
        /// </summary>
        /// <param name="email">The Email address to validate</param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            bool result;

            try
            {
                var addr = new MailAddress(email);
                result = true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                result = false;
            }

            return result;
        }
    }
}
