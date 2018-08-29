using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/Impersonate/
        [ChildActionOnly]
        [HttpGet]
        public ActionResult Impersonate()
        {
            return PartialView();
        }

        [ChildActionOnly]
        [HttpPost]
        public ActionResult Impersonate(BackEndImpersonate backEndImpersonate)
        {
            if (ModelState.IsValidOrRefresh())
            {
                BackEndSessions.CurrentUser = new Users().GetUserByUserName(backEndImpersonate.Username);

                AdminPages backEndPages = new AdminPages();
                BackEndSessions.CurrentMenu = backEndPages.GetMenuByGroupId(BackEndSessions.CurrentUser.GroupId);

                //Remove other specific sessions
                List<string> sessionsToRemove = Session.Keys.Cast<string>().Where(key => key.StartsWith("Data_") || key.StartsWith("Querystring_")).ToList();
                foreach (string key in sessionsToRemove)
                {
                    Session.Remove(key);
                }
            }

            return PartialView(backEndImpersonate);
        }
    }
}