using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using System.Web;
using Elmah;

namespace MVCwCMS.Helpers
{
    public class ElmahErrorLogModuleFix : ErrorLogModule
    {
        protected override void LogException(Exception e, HttpContext context)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            ExceptionFilterEventArgs args = new ExceptionFilterEventArgs(e, (object)context);
            this.OnFiltering(args);
            if (args.Dismissed)
                return;
            ErrorLogEntry entry = (ErrorLogEntry)null;
            try
            {
                //FIX STARTS
                //Error error = new Error(e, context);
                Error error = CreateErrorSafe(e, context);
                //FIX ENDS
                ErrorLog errorLog = this.GetErrorLog(context);
                error.ApplicationName = errorLog.ApplicationName;
                string id = errorLog.Log(error);
                entry = new ErrorLogEntry(errorLog, id, error);
            }
            catch (Exception ex)
            {
                Trace.WriteLine((object)ex);
            }
            if (entry == null)
                return;
            this.OnLogged(new ErrorLoggedEventArgs(entry));
        }

        public static Error CreateErrorSafe(Exception e, HttpContext context)
        {
            try
            {
                var safeFormCollection = new NameValueCollection();
                var form = context.Request.Form;
                var additionalMessage = string.Empty;
                foreach (var key in form.AllKeys)
                {
                    try
                    {
                        safeFormCollection.Add(key, form[key]);
                    }
                    catch (Exception)
                    {
                        safeFormCollection.Add(key, "_invalid input data_");
                        additionalMessage += "Form parameter with name=" + key + " has dangerous value. " + Environment.NewLine;
                    }
                }

                //if no invalid values in form then do as elmah does
                if (string.IsNullOrEmpty(additionalMessage))
                {
                    return new Error(e, context);
                }

                var exception = new Exception(additionalMessage, e);
                var error = new Error(exception);
                error.HostName = TryGetMachineName(context, null);

                IPrincipal user = context.User;
                if (user != null && NullString(user.Identity.Name).Length > 0)
                    error.User = user.Identity.Name;
                HttpRequest request = context.Request;
                //this._serverVariables = Error.CopyCollection(request.ServerVariables);
                error.ServerVariables.Add(CopyCollection(request.ServerVariables));
                if (error.ServerVariables != null && error.ServerVariables["AUTH_PASSWORD"] != null)
                    error.ServerVariables["AUTH_PASSWORD"] = "*****";
                error.QueryString.Add(CopyCollection(request.QueryString));
                error.Form.Add(CopyCollection(safeFormCollection));
                error.Cookies.Add(CopyCollection(request.Cookies));
                return error;
            }
            catch (Exception logEx)
            {
                return new Error(new Exception("Error when trying to process error catched by elmah", logEx));
            }
        }

        /// <summary>
        /// Elmah dll method in Environment.cs
        /// </summary>
        /// <param name="context"></param>
        /// <param name="unknownName"></param>
        /// <returns></returns>
        public static string TryGetMachineName(HttpContext context, string unknownName)
        {
            if (context != null)
            {
                try
                {
                    return context.Server.MachineName;
                }
                catch (HttpException)
                {
                }
                catch (SecurityException)
                {
                }
            }
            try
            {
                return System.Environment.MachineName;
            }
            catch (SecurityException)
            {
            }
            return NullString(unknownName);
        }

        /// <summary>
        /// Elmah method in Mask.cs
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NullString(string s)
        {
            if (s != null)
                return s;
            else
                return string.Empty;
        }

        /// <summary>
        /// Elmah method in Error.cs
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static NameValueCollection CopyCollection(NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0)
                //FIX HERE: cannot allow reutrn null collection as elmah does, because of exception. fix as below
                //return (NameValueCollection)null;
                return new NameValueCollection();
            //FIX ENDS
            else
                return new NameValueCollection(collection);
        }

        /// <summary>
        /// Elmah method in Error.cs
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private static NameValueCollection CopyCollection(HttpCookieCollection cookies)
        {
            if (cookies == null || cookies.Count == 0)
                //FIX HERE: cannot allow reutrn null collection as elmah does, because of exception. fix as below
                //return (NameValueCollection)null;
                return new NameValueCollection();
            //FIX ENDS
            NameValueCollection nameValueCollection = new NameValueCollection(cookies.Count);
            for (int index = 0; index < cookies.Count; ++index)
            {
                HttpCookie httpCookie = cookies[index];
                nameValueCollection.Add(httpCookie.Name, httpCookie.Value);
            }
            return nameValueCollection;
        }
    }
}