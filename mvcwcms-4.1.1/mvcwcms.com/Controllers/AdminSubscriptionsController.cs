using MVCwCMS.Filters;
using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/Subscriptions/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult Subscriptions(BackEndSubscriptionsList backEndSubscriptionsList)
        {
            Subscriptions subscriptions = new Subscriptions();
            backEndSubscriptionsList.SubscriptionsList = subscriptions.GetAllSubscriptions(backEndSubscriptionsList.Email, backEndSubscriptionsList.SubscriptionStatusId, backEndSubscriptionsList.FirstName, backEndSubscriptionsList.LastName, backEndSubscriptionsList.JoinDateFrom, backEndSubscriptionsList.JoinDateTo, backEndSubscriptionsList.WantsNewsletter);
            if (backEndSubscriptionsList.SubscriptionsList.IsNull() || backEndSubscriptionsList.SubscriptionsList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }

            return View(backEndSubscriptionsList);
        }

        //  /Admin/SubscriptionsAdd/
        [HttpGet]
        [IsRestricted]
        public ActionResult SubscriptionsAdd()
        {
            return View();
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult SubscriptionsAdd(BackEndSubscriptionsAdd backEndSubscriptionsAdd)
        {
            if (ModelState.IsValidOrRefresh())
            {
                Subscriptions subscriptions = new Subscriptions();
                int? result = subscriptions.Add(backEndSubscriptionsAdd.Email,
                                                       backEndSubscriptionsAdd.Password,
                                                       backEndSubscriptionsAdd.FirstName,
                                                       backEndSubscriptionsAdd.LastName,
                                                       backEndSubscriptionsAdd.Birthdate.ToDateTime(),
                                                       backEndSubscriptionsAdd.PhoneNumber,
                                                       backEndSubscriptionsAdd.Address,
                                                       backEndSubscriptionsAdd.City,
                                                       backEndSubscriptionsAdd.PostCode,
                                                       backEndSubscriptionsAdd.CountryCode,
                                                       backEndSubscriptionsAdd.SubscriptionStatusId,
                                                       backEndSubscriptionsAdd.WantsNewsletter,
                                                       DateTime.Now,
                                                       Request.UserHostAddress,
                                                       DateTime.Now.Ticks.ToBase36() + Session.SessionID,
                                                       backEndSubscriptionsAdd.Notes);
                switch (result)
                {
                    case 0:
                        ModelState.Clear();
                        backEndSubscriptionsAdd = new BackEndSubscriptionsAdd();

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.EmailAlreadyExists);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndSubscriptionsAdd);
        }

        //  /Admin/SubscriptionsEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult SubscriptionsEdit(string id)
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            BackEndSubscriptionsEdit backEndSubscriptionsEdit = new BackEndSubscriptionsEdit();

            Subscriptions subscriptions = new Subscriptions();
            Subscription subscription = subscriptions.GetSubscriptionByEmail(id);
            if (subscription.IsNotNull())
            {
                backEndSubscriptionsEdit.Email = subscription.Email;
                backEndSubscriptionsEdit.FirstName = subscription.FirstName;
                backEndSubscriptionsEdit.LastName = subscription.LastName;
                backEndSubscriptionsEdit.Birthdate = subscription.BirthDate.ToDateTimeString();
                backEndSubscriptionsEdit.PhoneNumber = subscription.PhoneNumber;
                backEndSubscriptionsEdit.Address = subscription.Address;
                backEndSubscriptionsEdit.City = subscription.City;
                backEndSubscriptionsEdit.PostCode = subscription.PostCode;
                backEndSubscriptionsEdit.CountryCode = subscription.CountryCode;
                backEndSubscriptionsEdit.SubscriptionStatusId = subscription.SubscriptionStatusId;
                backEndSubscriptionsEdit.WantsNewsletter = subscription.WantsNewsletter;
                backEndSubscriptionsEdit.JoinDate = subscription.JoinDate.ToDateTimeString();
                backEndSubscriptionsEdit.Notes = subscription.Notes;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndSubscriptionsEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult SubscriptionsEdit(BackEndSubscriptionsEdit backEndSubscriptionsEdit, string id)
        {
            Subscriptions subscriptions = new Subscriptions();
            int? result = subscriptions.Edit(id,
                                             backEndSubscriptionsEdit.Password,
                                             backEndSubscriptionsEdit.FirstName,
                                             backEndSubscriptionsEdit.LastName,
                                             backEndSubscriptionsEdit.Birthdate.ToDateTime(),
                                             backEndSubscriptionsEdit.PhoneNumber,
                                             backEndSubscriptionsEdit.Address,
                                             backEndSubscriptionsEdit.City,
                                             backEndSubscriptionsEdit.PostCode,
                                             backEndSubscriptionsEdit.CountryCode,
                                             backEndSubscriptionsEdit.SubscriptionStatusId,
                                             backEndSubscriptionsEdit.WantsNewsletter,
                                             backEndSubscriptionsEdit.Notes);
            switch (result)
            {
                case 0:
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
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

            return View(backEndSubscriptionsEdit);
        }

        //  /Admin/SubscriptionsDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult SubscriptionsDelete(string deleteId)
        {
            Subscriptions subscriptions = new Subscriptions();
            switch (subscriptions.Delete(deleteId))
            {
                case 0:
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;

                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemUsedSomewhereElse);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("Subscriptions");
        }

        //  /Admin/SubsriptionsConfiguration/
        [HttpGet]
        [IsRestricted]
        public ActionResult SubscriptionsConfiguration()
        {
            BackEndSubscriptionsConfigurationEdit backEndSubscriptionsConfigurationEdit = new BackEndSubscriptionsConfigurationEdit();

            SubscriptionConfiguration subscriptionConfiguration = new SubscriptionConfigurations().GetSubscriptionConfiguration();
            if (subscriptionConfiguration.IsNotNull())
            {
                backEndSubscriptionsConfigurationEdit.IsSubscriptionActive = subscriptionConfiguration.IsSubscriptionActive;
                backEndSubscriptionsConfigurationEdit.RegisterPageId = subscriptionConfiguration.RegisterPageId;
                backEndSubscriptionsConfigurationEdit.SignInPageId = subscriptionConfiguration.SignInPageId;
                backEndSubscriptionsConfigurationEdit.ForgotPasswordPageId = subscriptionConfiguration.ForgotPasswordPageId;
                backEndSubscriptionsConfigurationEdit.ChangePasswordPageId = subscriptionConfiguration.ChangePasswordPageId;
                backEndSubscriptionsConfigurationEdit.ProfilePageId = subscriptionConfiguration.ProfilePageId;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndSubscriptionsConfigurationEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult SubscriptionsConfiguration(BackEndSubscriptionsConfigurationEdit backEndSubscriptionsConfigurationEdit)
        {
            SubscriptionConfigurations subscriptionConfigurations = new SubscriptionConfigurations();
            int? result = subscriptionConfigurations.Edit(backEndSubscriptionsConfigurationEdit.IsSubscriptionActive,
                                                          backEndSubscriptionsConfigurationEdit.RegisterPageId,
                                                          backEndSubscriptionsConfigurationEdit.SignInPageId,
                                                          backEndSubscriptionsConfigurationEdit.ForgotPasswordPageId,
                                                          backEndSubscriptionsConfigurationEdit.ChangePasswordPageId,
                                                          backEndSubscriptionsConfigurationEdit.ProfilePageId);
            switch (result)
            {
                case 0:

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    ViewData.IsFormVisible(false);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return View(backEndSubscriptionsConfigurationEdit);
        }
    }
}