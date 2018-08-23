using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCwCMS.ModuleConnectors
{
    public class SharedContentsConnector : IModuleConnector
    {
        public List<SelectListItem> GetSelectItemList()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SharedContents sharedContents = new SharedContents();
            result.AddRange(sharedContents.GetAllSharedContentsAsSelectListItems());

            return result;
        }

        public string GetContent(HtmlHelper htmlHelper, FrontEndCmsPage model, string id)
        {
            string result = string.Empty;

            SharedContent sharedContent = new SharedContents().GetSharedContent(id, model.LanguageCode, true);
            if (sharedContent.IsNotNull())
            {
                //include any extra modules, excluding {$Content} and other {$SharedContent-...} to avoid recursion
                string controllerAction;
                string[] controllerActionArray;

                bool isModuleFound;

                foreach (string sharedContentPart in ExtensionsHelper.GetHtmlCodeParts(sharedContent.HtmlCode))
                {
                    isModuleFound = false;
                    foreach (IModuleConnector moduleConnector in ModuleConnectorsHelper.GetModuleConnectors())
                    {
                        foreach (SelectListItem module in moduleConnector.GetSelectItemList())
                        {
                            if (!sharedContentPart.StartsWith("{$SharedContent") && sharedContentPart.ToLower() == module.Value.ToLower())
                            {
                                isModuleFound = true;
                                controllerAction = sharedContentPart.Substring(2, sharedContentPart.Length - 3);
                                controllerActionArray = controllerAction.Split('-');
                                result += moduleConnector.GetContent(htmlHelper, model, controllerActionArray[1]);
                            }
                        }
                    }

                    if (!isModuleFound)
                    {
                        if (sharedContentPart != "{$Content}" && !sharedContentPart.StartsWith("{$SharedContent") && sharedContentPart.StartsWith("{$") && sharedContentPart.EndsWith("}") && sharedContentPart.Contains('-'))
                        {
                            controllerAction = sharedContentPart.Substring(2, sharedContentPart.Length - 3);
                            controllerActionArray = controllerAction.Split('-');
                            result += htmlHelper.Action(controllerActionArray[1], "FrontEnd" + controllerActionArray[0], model).ToString();
                        }
                        else
                        {
                            result += sharedContentPart;
                        }
                    }
                }
            }

            return result;
        }

        public bool IsFileUsed(string filePath)
        {
            bool result = false;

            SharedContents sharedContents = new SharedContents();
            if (sharedContents.IsNotNull())
            {
                result = sharedContents.IsFileUsed(filePath);
            }

            return result;
        }
    }
}