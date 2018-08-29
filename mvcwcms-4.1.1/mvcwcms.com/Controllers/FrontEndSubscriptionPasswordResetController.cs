using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndSubscriptionPasswordResetController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page, string PasswordResetKey)
        {
            FrontEndSubscriptionPasswordReset frontEndSubscriptionPasswordReset = new FrontEndSubscriptionPasswordReset()
            {
                LanguageCode = page.LanguageCode
            };

            if (PasswordResetKey.IsNotEmptyOrWhiteSpace())
            {
                if (!(new Subscriptions().IsPasswordResetKeyValid(PasswordResetKey)))
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_Subscription.PasswordResetUrlNotValid);
                    ViewData.IsFormVisible(false);
                }
                frontEndSubscriptionPasswordReset.PasswordResetKey = PasswordResetKey;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_Subscription.PasswordResetUrlNotValid);
                ViewData.IsFormVisible(false);
            }

            return View(frontEndSubscriptionPasswordReset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndSubscriptionPasswordReset frontEndSubscriptionPasswordReset)
        {
            ViewBag.IsPasswordResetKeyValid = true;
            if (ModelState.IsValidOrRefresh())
            {
                Subscriptions subscriptions = new Subscriptions();
                int? result = subscriptions.ResetPassword(frontEndSubscriptionPasswordReset.PasswordResetKey, frontEndSubscriptionPasswordReset.NewPassword);
                switch (result)
                {
                    case 0:
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.PasswordSuccessfullyChanged);
                        ViewData.IsFormVisible(false);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_Subscription.PasswordResetUrlNotValid);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(frontEndSubscriptionPasswordReset);
        }
    }
}