using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Helpers
{
    public class ChildActionRedirectResult : RedirectResult
    {
        public ChildActionRedirectResult(string url)
            : base(url)
        {
        }

        public ChildActionRedirectResult(string url, bool permanent)
            : base(url, permanent)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context.IsNull())
            {
                throw new ArgumentNullException("context");
            }
            //if (context.IsChildAction)
            //{
            //    throw new InvalidOperationException("Child actions are not allowed to perform redirect actions");
            //}

            string destinationUrl = UrlHelper.GenerateContentUrl(Url, context.HttpContext);
            context.Controller.TempData.Keep();

            if (Permanent)
            {
                context.HttpContext.Response.RedirectPermanent(destinationUrl, false);
            }
            else
            {
                context.HttpContext.Response.Redirect(destinationUrl, false);
            }
        }
    }
}