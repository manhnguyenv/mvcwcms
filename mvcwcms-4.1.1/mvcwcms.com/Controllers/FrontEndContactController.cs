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
    public class FrontEndContactController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndContact frontEndContact)
        {
            if (ModelState.IsValidOrRefresh())
            {
                if (frontEndContact.Notes.IsEmpty()) //Honey pot to avoid spammers
                {
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                    CultureInfoHelper.ForceBackEndLanguage();

                    string subject = Resources.Strings_Contact.EmailSubject.Replace("{$Url}", globalConfiguration.DomainName.ToUrl());
                    string body = Resources.Strings_Contact.EmailBody
                                  .Replace("{$fullName}", frontEndContact.FullName)
                                  .Replace("{$companyName}", frontEndContact.CompanyName)
                                  .Replace("{$phoneNumber}", frontEndContact.PhoneNumber)
                                  .Replace("{$email}", frontEndContact.Email)
                                  .Replace("{$message}", frontEndContact.Message)
                                  .Replace("{$ipAddress}", Request.UserHostAddress)
                                  .Replace("{$Url}", globalConfiguration.DomainName.ToUrl());

                    CultureInfoHelper.RestoreFrontEndLanguage();

                    EmailHelper email = new EmailHelper(globalConfiguration.NotificationEmail, globalConfiguration.NotificationEmail, subject, body);
                    email.ReplyToList.Add(frontEndContact.Email);
                    if (email.Send())
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings_Contact.Thanks);
                        ViewData.IsFormVisible(false);
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    }
                }
                else
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings_Contact.Thanks);
                    ViewData.IsFormVisible(false);
                }
            }

            return View(frontEndContact);
        }
    }
}
