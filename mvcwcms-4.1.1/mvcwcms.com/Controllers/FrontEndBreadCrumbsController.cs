using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndBreadCrumbsController : FrontEndBaseController
    {
        public ActionResult Index(FrontEndCmsPage page)
        {
            return View(page);
        }
    }
}