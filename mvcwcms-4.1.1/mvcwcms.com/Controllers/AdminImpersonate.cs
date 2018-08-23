using MVCwCMS.Models;
using System.Web.Mvc;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Web;
using MVCwCMS.Helpers;
using System.Web.Script.Serialization;
using MVCwCMS.Filters;
using System.Linq;

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