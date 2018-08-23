using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public class FrontEndBaseController : Controller
    {
        public FrontEndBaseController()
            : base()
        { 
            //Constructor
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        /// <summary>
        /// Creates a System.Web.Mvc.ChildActionRedirectResult object that 302 redirects (temporary) to the specified url without raising any "Child actions are not allowed to perform redirect actions" exceptions.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The redirect result object.</returns>
        protected ChildActionRedirectResult ChildActionRedirect(string url)
        {
            return new ChildActionRedirectResult(url);
        }

        /// <summary>
        /// Creates a System.Web.Mvc.ChildActionRedirectResult object that 301 redirects (permanent) to the specified url without raising any "Child actions are not allowed to perform redirect actions" exceptions.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The redirect result object.</returns>
        protected ChildActionRedirectResult ChildActionRedirectPermanent(string url)
        {
            return new ChildActionRedirectResult(url, true);
        }
    }
}
