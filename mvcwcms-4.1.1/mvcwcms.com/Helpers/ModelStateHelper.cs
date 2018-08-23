using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public enum ModelStateResult
{
    Error,
    Success
}

public static class ModelStateHelper
{
    /// <summary>
    /// Adds the specified message to the success or error collection
    /// </summary>
    /// <param name="modelStateDictionary"></param>
    /// <param name="viewData"></param>
    /// <param name="modelStateResult"></param>
    /// <param name="message"></param>
    public static void AddResult(this ModelStateDictionary modelStateDictionary, ViewDataDictionary viewData, ModelStateResult modelStateResult, string message)
    {
        switch (modelStateResult)
        {
            case ModelStateResult.Success:
                viewData["SuccessMessage" + viewData.Count.ToString()] = message;
                break;
            default:
                modelStateDictionary.AddModelError("ErrorMessage", message);
                break;
        }
    }

    /// <summary>
    /// Set the ViewData value to determine if the form is visible or not
    /// </summary>
    /// <param name="viewData"></param>
    /// <param name="value">True is the form is visible</param>
    public static void IsFormVisible(this ViewDataDictionary viewData, bool value)
    {
        viewData["ModelStateResultIsFormVisible"] = value;
    }
    
    /// <summary>
    /// Determines if the form is visible or not as specified in the ViewData
    /// </summary>
    /// <param name="modelStateDictionary"></param>
    /// <returns></returns>
    public static bool IsFormVisible(this ViewDataDictionary viewData)
    {
        if (viewData.ContainsKey("ModelStateResultIsFormVisible"))
        {
            return viewData["ModelStateResultIsFormVisible"].ConvertTo<bool>(true, true);
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// If the IsRefresh QueryString parameter is true it clears the ModelStateDictionary and returns false else behaves exactly as ModelState.IsValid property
    /// </summary>
    /// <param name="modelStateDictionary"></param>
    /// <returns></returns>
    public static bool IsValidOrRefresh(this ModelStateDictionary modelStateDictionary)
    {
        bool isRefresh = HttpContext.Current.Request["IsRefresh"].ConvertTo<bool>(false, true);

        if (isRefresh)
        {
            modelStateDictionary.Clear();
        }

        return (!isRefresh && modelStateDictionary.IsValid);
    }
}