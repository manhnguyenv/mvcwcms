using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
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