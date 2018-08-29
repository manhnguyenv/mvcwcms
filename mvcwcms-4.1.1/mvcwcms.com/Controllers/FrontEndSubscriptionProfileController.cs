using MVCwCMS.Filters;
using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndSubscriptionProfileController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page)
        {
            FrontEndSubscriptionProfile frontEndSubscriptionProfile = new FrontEndSubscriptionProfile();

            Subscription subscription = new Subscriptions().GetSubscriptionByEmail(FrontEndSessions.CurrentSubscription.Email);
            if (subscription.IsNotNull())
            {
                frontEndSubscriptionProfile.LanguageCode = page.LanguageCode;
                frontEndSubscriptionProfile.Email = subscription.Email;
                frontEndSubscriptionProfile.FirstName = subscription.FirstName;
                frontEndSubscriptionProfile.LastName = subscription.LastName;
                frontEndSubscriptionProfile.Birthdate = subscription.BirthDate.ToDateTimeString();
                frontEndSubscriptionProfile.PhoneNumber = subscription.PhoneNumber;
                frontEndSubscriptionProfile.Address = subscription.Address;
                frontEndSubscriptionProfile.City = subscription.City;
                frontEndSubscriptionProfile.PostCode = subscription.PostCode;
                frontEndSubscriptionProfile.CountryCode = subscription.CountryCode;
                frontEndSubscriptionProfile.WantsNewsletter = subscription.WantsNewsletter;
                frontEndSubscriptionProfile.JoinDate = subscription.JoinDate.ToDateTimeString();
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(frontEndSubscriptionProfile);
        }

        [HttpPost]
        [IsFrontEndChildActionRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndSubscriptionProfile frontEndSubscriptionProfile)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Subscriptions subscriptions = new Subscriptions();
                int? result = subscriptions.EditProfile(FrontEndSessions.CurrentSubscription.Email,
                                                        frontEndSubscriptionProfile.FirstName,
                                                        frontEndSubscriptionProfile.LastName,
                                                        frontEndSubscriptionProfile.Birthdate.ToDateTime(),
                                                        frontEndSubscriptionProfile.PhoneNumber,
                                                        frontEndSubscriptionProfile.Address,
                                                        frontEndSubscriptionProfile.City,
                                                        frontEndSubscriptionProfile.PostCode,
                                                        frontEndSubscriptionProfile.CountryCode,
                                                        frontEndSubscriptionProfile.WantsNewsletter);
                switch (result)
                {
                    case 0:
                        FrontEndSessions.CurrentSubscription = subscriptions.GetSubscriptionByEmail(FrontEndSessions.CurrentSubscription.Email);

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings_Subscription.ProfileEditSuccess);
                        ViewData.IsFormVisible(false);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                        break;

                    case 3:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.EmailAlreadyExists);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(frontEndSubscriptionProfile);
        }
    }
}