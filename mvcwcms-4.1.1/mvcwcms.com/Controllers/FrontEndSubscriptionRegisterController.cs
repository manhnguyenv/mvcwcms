using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndSubscriptionRegisterController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page, string k)
        {
            FrontEndSubscriptionRegister frontEndSubscriptionRegister = new FrontEndSubscriptionRegister()
            {
                LangugeCode = page.LanguageCode
            };

            ViewBag.IsActivationPage = false;
            if (k.IsNotEmptyOrWhiteSpace())
            {
                ViewBag.IsActivationPage = true;
                Subscriptions subscriptions = new Subscriptions();
                int? result = subscriptions.ActivativateSubscription(k);
                switch (result)
                {
                    case 0:
                        Subscription subscription = subscriptions.GetSubscriptionByActivationKey(k);

                        GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                        CultureInfoHelper.ForceBackEndLanguage();

                        string subject = Resources.Strings_Subscription.EmailSubscriptionAddedSubject.Replace("{$Url}", globalConfiguration.DomainName.ToUrl());
                        string body = Resources.Strings_Subscription.EmailSubscriptionAddedBody
                                      .Replace("{$Url}", globalConfiguration.DomainName.ToUrl())
                                      .Replace("{$subscriptionEmail}", subscription.Email)
                                      .Replace("{$ipAddress}", Request.UserHostAddress);

                        CultureInfoHelper.RestoreFrontEndLanguage();

                        EmailHelper email = new EmailHelper(globalConfiguration.NotificationEmail, globalConfiguration.NotificationEmail, subject, body);
                        email.Send();

                        ViewBag.ActivationResult = Resources.Strings_Subscription.ActivationKeySuccess;
                        break;

                    case 2:
                        ViewBag.ActivationResult = Resources.Strings_Subscription.ActivationKeyInvalid;
                        break;

                    case 3:
                        ViewBag.ActivationResult = Resources.Strings_Subscription.ActivationKeyAlreadyUsed;
                        break;

                    default:
                        ViewBag.ActivationResult = Resources.Strings.UnexpectedError;
                        break;
                }
            }

            return View(frontEndSubscriptionRegister);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndSubscriptionRegister frontEndSubscriptionRegister)
        {
            ViewBag.IsActivationPage = false;
            if (ModelState.IsValidOrRefresh())
            {
                if (frontEndSubscriptionRegister.Notes.IsEmpty()) //Honey pot to avoid spammers
                {
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                    //Add the subscription to the database and generate the {$VerificationUrl}
                    string ActivationKey = DateTime.Now.Ticks.ToBase36() + Session.SessionID;
                    Subscriptions subscriptions = new Subscriptions();
                    int? result = subscriptions.Add(frontEndSubscriptionRegister.Email,
                                                    frontEndSubscriptionRegister.Password,
                                                    frontEndSubscriptionRegister.FirstName,
                                                    frontEndSubscriptionRegister.LastName,
                                                    frontEndSubscriptionRegister.Birthdate.ToDateTime(),
                                                    frontEndSubscriptionRegister.PhoneNumber,
                                                    frontEndSubscriptionRegister.Address,
                                                    frontEndSubscriptionRegister.City,
                                                    frontEndSubscriptionRegister.PostCode,
                                                    frontEndSubscriptionRegister.CountryCode,
                                                    1, //Not verified
                                                    frontEndSubscriptionRegister.WantsNewsletter,
                                                    DateTime.Now,
                                                    Request.UserHostAddress,
                                                    ActivationKey,
                                                    null);
                    switch (result)
                    {
                        case 0:
                            //Send the email to the subscriber to confirm his identity
                            string subject = Resources.Strings_Subscription.EmailVerifyAccountSubject.Replace("{$SiteName}", globalConfiguration.SiteName);
                            string body = Resources.Strings_Subscription.EmailVerifyAccountBody
                                          .Replace("{$FirstName}", frontEndSubscriptionRegister.FirstName)
                                          .Replace("{$SiteName}", globalConfiguration.SiteName)
                                          .Replace("{$VerificationUrl}", globalConfiguration.DomainName.ToUrl() + page.LanguageCode + "/register?k=" + ActivationKey)
                                          .Replace("{$Url}", globalConfiguration.DomainName.ToUrl());

                            EmailHelper email = new EmailHelper("subscription-activation@" + globalConfiguration.DomainName, frontEndSubscriptionRegister.Email, subject, body);
                            if (email.Send())
                            {
                                ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings_Subscription.Thanks);
                                ViewData.IsFormVisible(false);
                            }
                            else
                            {
                                subscriptions.Delete(frontEndSubscriptionRegister.Email);
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + " (Email)");
                            }

                            break;

                        case 2:
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.EmailAlreadyExists);
                            break;

                        default:
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + " (Database)");
                            break;
                    }
                }
                else
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings_Subscription.Thanks);
                    ViewData.IsFormVisible(false);
                }
            }

            return View(frontEndSubscriptionRegister);
        }
    }
}