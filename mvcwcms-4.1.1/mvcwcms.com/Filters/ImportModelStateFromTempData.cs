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
    public sealed class ImportModelStateFromTempDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Imports ModelState
            ModelStateDictionary modelState = filterContext.Controller.TempData["ModelStateTempDataTransfer"] as ModelStateDictionary;
            if (modelState != null)
            {
                //Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    filterContext.Controller.ViewData.ModelState.Merge(modelState);
                }
                else if (filterContext.Result is RedirectResult)
                {
                    //Keep ModelStateTempDataTransfer
                    filterContext.Controller.TempData.Keep("ModelStateTempDataTransfer");
                }
                else
                {
                    //Otherwise remove it.
                    filterContext.Controller.TempData.Remove("ModelStateTempDataTransfer");
                }
            }

            //Imports SuccessMessages into ViewData
            IEnumerable<KeyValuePair<string, object>> successMessages = filterContext.Controller.TempData["SuccessMessagesTempDataTransfer"] as IEnumerable<KeyValuePair<string, object>>;
            if (successMessages != null)
            {
                //Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    filterContext.Controller.ViewData.AddRangeUnique(successMessages);
                }
                else if (filterContext.Result is RedirectResult) {
                    //Keep SuccessMessagesTempDataTransfer
                    filterContext.Controller.TempData.Keep("SuccessMessagesTempDataTransfer");
                }
                else
                {
                    //Otherwise remove it.
                    filterContext.Controller.TempData.Remove("SuccessMessagesTempDataTransfer");
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}