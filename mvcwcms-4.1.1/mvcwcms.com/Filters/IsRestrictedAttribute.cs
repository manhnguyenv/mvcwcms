using MVCwCMS.Models;
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
    /// Must be applied always before any other action filters
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IsRestrictedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            string action = filterContext.ActionDescriptor.ActionName;

            if (BackEndSessions.CurrentUser.IsNull())
            {
                if (!(controller == "admin" && action.ToLower() == "login"))
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Admin", ReturnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri) }));
            }
            else
            {
                AdminPages backEndPages = new AdminPages();
                AdminPage backEndPage = backEndPages.GetPageByAction(action);
                if (backEndPage.IsNotNull())
                {
                    if (backEndPages.IsPermissionGranted(backEndPage.PageId, PermissionCode.Browse))
                    {
                        if (controller == "admin" && action.ToLower() == "login")
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Admin" }));
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    action = "ErrorPage",
                                    controller = "Admin",
                                    errorPage = action,
                                    errorMessage = Resources.Strings.PageAccessNotAuthorized
                                }
                            )
                        );
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                action = "ErrorPage",
                                controller = "Admin",
                                errorPage = action,
                                errorMessage = Resources.Strings.Error404
                            }
                        )
                    );
                }
            }
        }
    }
}