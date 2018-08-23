using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.Resources;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndErrorController : FrontEndBaseController
    {
        public ActionResult Index(FrontEndCmsPage page)
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
            FrontEndError error = new FrontEndError()
            {
                LanguageFolder = page.LanguageFolder,
                StatusCode = page.StatusCode.ConvertTo<int>(),
                ErroMessage = ("Error" + page.StatusCode.ConvertTo<int>().ToString()).GetStringFromResource()
            };

            return View(error);
        }
    }
}
