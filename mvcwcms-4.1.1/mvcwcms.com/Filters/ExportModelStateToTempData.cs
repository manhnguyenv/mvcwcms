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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ExportModelStateToTempDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Only exports when ModelState is not valid
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                //Exports if we are redirecting
                if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
                {
                    filterContext.Controller.TempData["ModelStateTempDataTransfer"] = filterContext.Controller.ViewData.ModelState;
                }
            }

            //Exports the success messages
            List<string> keys = filterContext.Controller.ViewData.Keys.Where(k => k.StartsWith("SuccessMessage")).ToList();
            if (keys.Count > 0)
            {
                //Exports if we are redirecting
                if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
                {
                    filterContext.Controller.TempData["SuccessMessagesTempDataTransfer"] = filterContext.Controller.ViewData.Where(v => v.Key.StartsWith("SuccessMessage"));
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}