using MVCwCMS.Filters;
using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ModuleConnectors;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/FileManager/
        [IsRestricted]
        public ActionResult FileManager()
        {
            return View();
        }

        //  /Admin/FileManager/
        [IsRestricted]
        public ActionResult FileManagerIsFileUsed(string f)
        {
            bool isFileUsed = false;

            if (!isFileUsed)
            {
                PagesLanguages backEndCmsPagesContent = new PagesLanguages();
                isFileUsed = backEndCmsPagesContent.IsFileUsed(f);
            }

            if (!isFileUsed)
            {
                PageTemplates backEndCmsPagesTemplates = new PageTemplates();
                isFileUsed = backEndCmsPagesTemplates.IsFileUsed(f);
            }

            if (!isFileUsed)
            {
                foreach (IModuleConnector moduleConnector in ModuleConnectorsHelper.GetModuleConnectors())
                {
                    isFileUsed = moduleConnector.IsFileUsed(f);
                    if (isFileUsed)
                    {
                        break;
                    }
                }
            }

            return Content(isFileUsed.ToString(), "text/plain");
        }
    }
}