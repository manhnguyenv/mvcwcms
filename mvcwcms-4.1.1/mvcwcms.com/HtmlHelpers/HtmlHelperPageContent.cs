using MVCwCMS;
using MVCwCMS.Models;
using MVCwCMS.Helpers;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCwCMS.ModuleConnectors;
using System.Reflection;

namespace MVCwCMS.HtmlHelpers
{
    public static class HtmlHelperPageContent
    {
        public static IHtmlString GetPageContent(this HtmlHelper htmlHelper, FrontEndCmsPage model)
        {
            string result = string.Empty;

            PageLanguage backEndCmsPageLanguage = new PagesLanguages().GetPageLanguage(model.PageId, model.LanguageCode);

            string controllerAction;
            string[] controllerActionArray;

            if (model.PageTemplateId.IsNotNull())
            {
                PageTemplate pageTemplate = new PageTemplates().GetPageTemplateById(model.PageTemplateId);

                IEnumerable<IModuleConnector> moduleConnectors = ModuleConnectorsHelper.GetModuleConnectors();

                bool isModuleFound;

                foreach (string templatePart in ExtensionsHelper.GetHtmlCodeParts(pageTemplate.HtmlCode))
                {
                    isModuleFound = false;
                    foreach (IModuleConnector moduleConnector in moduleConnectors)
                    {
                        foreach (SelectListItem module in moduleConnector.GetSelectItemList())
                        {
                            if (templatePart.ToLower() == module.Value.ToLower())
                            {
                                isModuleFound = true;
                                controllerAction = templatePart.Substring(2, templatePart.Length - 3);
                                controllerActionArray = controllerAction.Split('-');
                                result += moduleConnector.GetContent(htmlHelper, model, controllerActionArray[1]);
                            }
                        }
                    }

                    if (!isModuleFound)
                    {
                        if (templatePart == "{$Content}")
                        {
                            if (backEndCmsPageLanguage.IsNotNull())
                            {
                                //include any extra modules
                                foreach (string pageLanguagePart in ExtensionsHelper.GetHtmlCodeParts(backEndCmsPageLanguage.HtmlCode))
                                {
                                    isModuleFound = false;
                                    foreach (IModuleConnector moduleConnector in moduleConnectors)
                                    {
                                        foreach (SelectListItem module in moduleConnector.GetSelectItemList())
                                        {
                                            if (pageLanguagePart.ToLower() == module.Value.ToLower())
                                            {
                                                isModuleFound = true;
                                                controllerAction = pageLanguagePart.Substring(2, pageLanguagePart.Length - 3);
                                                controllerActionArray = controllerAction.Split('-');
                                                result += moduleConnector.GetContent(htmlHelper, model, controllerActionArray[1]);
                                            }
                                        }
                                    }

                                    if (!isModuleFound)
                                    {
                                        if (pageLanguagePart.StartsWith("{$") && pageLanguagePart.EndsWith("}") && pageLanguagePart.Contains('-'))
                                        {
                                            controllerAction = pageLanguagePart.Substring(2, pageLanguagePart.Length - 3);
                                            controllerActionArray = controllerAction.Split('-');
                                            try
                                            {
                                                result += htmlHelper.Action(controllerActionArray[1], "FrontEnd" + controllerActionArray[0], model).ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                            }
                                        }
                                        else
                                        {
                                            result += pageLanguagePart;
                                        }
                                    }
                                }
                            }
                        }
                        else if (templatePart.StartsWith("{$") && templatePart.EndsWith("}") && templatePart.Contains('-'))
                        {
                            controllerAction = templatePart.Substring(2, templatePart.Length - 3);
                            controllerActionArray = controllerAction.Split('-');
                            result += htmlHelper.Action(controllerActionArray[1], "FrontEnd" + controllerActionArray[0], model).ToString();
                        }
                        else
                        {
                            result += templatePart;
                        }
                    }
                }
            }

            return htmlHelper.Raw(result.ReplaceGlobalTokens());
        }
    }
}
