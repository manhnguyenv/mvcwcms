using MVCwCMS.Models;
using MVCwCMS.Helpers;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCwCMS.Filters;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndSubscriptionPasswordChangeController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page)
        {
            return View();
        }

        [HttpPost]
        [IsFrontEndChildActionRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndSubscriptionPasswordChange frontEndSubscriptionPasswordChange)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Subscriptions subscriptions = new Subscriptions();
                int? result = subscriptions.ChangePassword(FrontEndSessions.CurrentSubscription, frontEndSubscriptionPasswordChange.CurrentPassword, frontEndSubscriptionPasswordChange.NewPassword);
                switch (result)
                {
                    case 0:
                        //Refresh the CurrentSubscription
                        FrontEndSessions.CurrentSubscription = subscriptions.GetSubscriptionByEmail(FrontEndSessions.CurrentSubscription.Email);

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.PasswordSuccessfullyChanged);
                        ViewData.IsFormVisible(false);
                        break;
                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                        break;
                    case 3:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.EmailAlreadyExists);
                        break;
                    case 4:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.CurrentPasswordNotValid);
                        break;
                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(frontEndSubscriptionPasswordChange);
        }
    }
}
