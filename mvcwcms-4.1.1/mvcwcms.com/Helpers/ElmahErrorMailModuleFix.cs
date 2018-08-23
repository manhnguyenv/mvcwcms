using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using System.Web;
using Elmah;

namespace MVCwCMS.Helpers
{
    public class ElmahErrorMailModuleFix : ErrorMailModule
    {
        private bool _reportAsynchronously2;

        protected override void OnInit(HttpApplication application)
        {
            base.OnInit(application);
            IDictionary config = (IDictionary)this.GetConfig();
            if (config == null)
                return;
            _reportAsynchronously2 = Convert.ToBoolean(GetSetting(config, "async", bool.TrueString));
        }

        protected override void OnError(Exception e, HttpContext context)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            ExceptionFilterEventArgs args = new ExceptionFilterEventArgs(e, (object)context);
            this.OnFiltering(args);
            if (args.Dismissed)
                return;
            //FIX STARTS
            //Error error = new Error(e, context);
            Error error = ElmahErrorLogModuleFix.CreateErrorSafe(e, context);
            //FIX ENDS
            if (this._reportAsynchronously2)
                this.ReportErrorAsync(error);
            else
                this.ReportError(error);
        }

        /// <summary>
        /// Elmah method in ErrorMailModule.cs
        /// </summary>
        /// <param name="config"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string GetSetting(IDictionary config, string name, string defaultValue)
        {
            string str = ElmahErrorLogModuleFix.NullString((string)config[(object)name]);
            if (str.Length == 0)
            {
                if (defaultValue == null)
                    throw new global::Elmah.ApplicationException(string.Format("The required configuration setting '{0}' is missing for the error mailing module.", (object)name));
                str = defaultValue;
            }
            return str;
        }
    }
}