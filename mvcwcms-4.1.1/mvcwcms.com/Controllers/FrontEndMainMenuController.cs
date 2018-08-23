using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndMainMenuController : FrontEndBaseController
    {
        public ActionResult Index(FrontEndCmsPage page)
        {
            return View(page);
        }
    }
}