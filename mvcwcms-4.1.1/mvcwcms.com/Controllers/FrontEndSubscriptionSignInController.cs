using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndSubscriptionSignInController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page, string a, string ReturnUrl)
        {
            if (a.IfEmpty("").ToLower() == "sign-out")
            {
                FrontEndSessions.CurrentSubscription = null;

                //return Content("<script>window.location.href='/" + page.LanguageCode + "';</script>");
                return ChildActionRedirect("~/" + page.LanguageCode + "/");
            }

            FrontEndSubscriptionSignIn frontEndSubscriptionSignIn = new FrontEndSubscriptionSignIn()
            {
                LanguageCode = page.LanguageCode
            };
            if (ReturnUrl.IsNotEmptyOrWhiteSpace())
            {
                frontEndSubscriptionSignIn.ReturnUrl = ReturnUrl;
            }

            return View(frontEndSubscriptionSignIn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndSubscriptionSignIn frontEndSubscriptionSignIn)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Subscription subscription = new Subscriptions().GetSubscriptionActiveByEmailAndPassword(frontEndSubscriptionSignIn.Email, frontEndSubscriptionSignIn.Password);
                if (subscription.IsNotNull())
                {
                    FrontEndSessions.CurrentSubscription = subscription;

                    if (frontEndSubscriptionSignIn.ReturnUrl.IsNotEmptyOrWhiteSpace())
                    {
                        //return Content("<script>window.location.href='" + frontEndSubscriptionSignIn.ReturnUrl + "';</script>");
                        return ChildActionRedirect(frontEndSubscriptionSignIn.ReturnUrl);
                    }
                    else
                    {
                        //return Content("<script>window.location.href='/" + page.LanguageCode + "';</script>");
                        return ChildActionRedirect("~/" + page.LanguageCode + "/");
                    }
                }
                else
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_Subscription.SubscriptionEmailOrPasswordNotValid);
                }
            }

            return View(frontEndSubscriptionSignIn);
        }
    }
}