using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Helpers
{
    public class ControllerAction
    {
        public string ControllerActionID { get; set; }
        public string ControllerActionName { get; set; }
    }

    public static class ControllerHelper
    {
        public static object GetRoutesConstraints(string controllerPrefix)
        {
            List<string> filteredControllers = GetAllControllers(controllerPrefix);

            return new
            {
                controller = "(" + string.Join("|", filteredControllers) + ")"
            };
        }

        public static List<ControllerAction> GetAllControllersActions()
        {
            List<ControllerAction> result = new List<ControllerAction>();

            ControllerAction controllerAction;
            string controllerActionID, controllerName;
            List<string> frontEndControllers = GetAllControllers("FrontEnd");
            foreach (string frontEndController in frontEndControllers)
            {
                Type t = Type.GetType("MVCwCMS.Controllers." + frontEndController + "Controller");
                if (t.IsNotNull())
                {
                    controllerName = frontEndController.Replace("FrontEnd", "");
                    MethodInfo[] methods = t.GetMethods();
                    foreach (MethodInfo method in methods)
                    {
                        if (method.IsPublic && typeof(ActionResult).IsAssignableFrom(method.ReturnParameter.ParameterType))
                        {
                            controllerActionID = "{$" + controllerName + "-" + method.Name + "}";
                            int x = result.Count(ca => ca.ControllerActionID == controllerActionID);
                            if (x == 0)
                            {
                                controllerAction = new ControllerAction();
                                controllerAction.ControllerActionID = controllerActionID;
                                //controllerAction.ControllerActionName = controllerName + (method.Name.ToLower() == "index" ? "" : " - " + method.Name);
                                controllerAction.ControllerActionName = controllerName + " -> " + method.Name;
                                result.Add(controllerAction);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static List<string> GetAllControllers(string controllerPrefix)
        {
            return (from t in System.Reflection.Assembly.GetCallingAssembly().GetTypes()
                    where t.IsNotNull() &&
                        t.IsPublic &&
                        !t.IsAbstract &&
                        t.Name.StartsWith(controllerPrefix, StringComparison.OrdinalIgnoreCase) &&
                        t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
                        typeof(IController).IsAssignableFrom(t)
                    select t.Name.Substring(0, t.Name.Length - 10)).ToList();
        }
    }
}