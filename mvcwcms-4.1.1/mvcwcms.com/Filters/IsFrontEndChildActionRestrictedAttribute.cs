using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCwCMS.Filters
{
    /// <summary>
    /// Allows the signed-in users to execute front-end child actions. Usually applied to [HttpPost] action methods only because the [HttpGet] action methods are secured via the DefaultController class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IsFrontEndChildActionRestrictedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            FrontEndCmsPage page = filterContext.ActionParameters["page"] as FrontEndCmsPage;

            if (page.IsNotNull())
            {
                CmsPages cmsPages = new CmsPages();

                if (FrontEndSessions.CurrentSubscription.IsNull())
                {
                    SubscriptionConfiguration subscriptionConfiguration = new SubscriptionConfigurations().GetSubscriptionConfiguration();
                    CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.SignInPageId, page.LanguageCode);
                    if (cmsPageActionlink.IsNotNull())
                    {
                        filterContext.Result = new ChildActionRedirectResult(cmsPageActionlink.Url + "?ReturnUrl=" + HttpUtility.UrlEncode(filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri));
                    }
                }
            }
        }
    }
}