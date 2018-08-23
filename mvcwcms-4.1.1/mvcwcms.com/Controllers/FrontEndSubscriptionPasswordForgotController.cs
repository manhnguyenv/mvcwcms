using MVCwCMS.Models;
using MVCwCMS.Helpers;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndSubscriptionPasswordForgotController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page)
        {
            FrontEndSubscriptionPasswordForgot frontEndSubscriptionPasswordForgot = new FrontEndSubscriptionPasswordForgot()
            {
                LanguageCode = page.LanguageCode
            };
            return View(frontEndSubscriptionPasswordForgot);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndSubscriptionPasswordForgot frontEndSubscriptionPasswordForgot)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Subscriptions subscriptions =  new Subscriptions();
                Subscription subscription = subscriptions.GetSubscriptionByEmail(frontEndSubscriptionPasswordForgot.Email);
                if (subscription.IsNotNull())
                {
                    string PasswordResetKey = DateTime.Now.Ticks.ToBase36() + frontEndSubscriptionPasswordForgot.Email.EncodeBase64() + Session.SessionID;

                    int? result = subscriptions.EditPasswordResetKey(frontEndSubscriptionPasswordForgot.Email, PasswordResetKey);
                    switch (result)
                    {
                        case 0:
                            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                            string subject = Resources.Strings_Subscription.EmailPwForgotSubject.Replace("{$SiteName}", globalConfiguration.SiteName);
                            string body = Resources.Strings_Subscription.EmailPwForgotBody
                                          .Replace("{$FirstName}", subscription.FirstName)
                                          .Replace("{$SiteName}", globalConfiguration.SiteName)
                                          .Replace("{$PasswordResetUrl}", globalConfiguration.DomainName.ToUrl() + page.LanguageCode + "/reset-password?PasswordResetKey=" + PasswordResetKey)
                                          .Replace("{$Url}", globalConfiguration.DomainName.ToUrl());

                            EmailHelper email = new EmailHelper("subscription-pw-reset@" + globalConfiguration.DomainName, frontEndSubscriptionPasswordForgot.Email, subject, body);
                            if (email.Send())
                            {
                                ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.FormSuccessfullySubmitted);
                                ViewData.IsFormVisible(false);
                            }
                            else
                            {
                                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + " (Email)");
                            }
                            break;
                        case 2:
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_Subscription.SubscriptionEmailNotValid);
                            break;
                        default:
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + " (Database)");
                            break;
                    }
                }
                else
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings_Subscription.SubscriptionEmailNotValid);
                }
            }

            return View(frontEndSubscriptionPasswordForgot);
        }
    }
}