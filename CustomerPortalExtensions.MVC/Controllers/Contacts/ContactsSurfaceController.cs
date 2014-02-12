using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Membership;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.Synchronisation;
using CustomerPortalExtensions.Models.Contacts;
using CustomerPortalExtensions.MVC.Models;
using CustomerPortalExtensions.MVC.Models.Contacts;
using CustomerPortalExtensions.MVC.Models.Membership;
using Omu.ValueInjecter;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace CustomerPortalExtensions.MVC.Controllers.Contacts
{
    [PluginController("CustomerPortal")]
    public class ContactsSurfaceController : CpxBaseSurfaceController
    {
        private readonly IContactService _contactService;
        private readonly IContactSynchroniser _contactSynchroniser;

        //
        // GET: /Contacts/

        public ContactsSurfaceController(IContactSynchroniser contactSynchroniser, IContactService contactService)
        {
            if (contactSynchroniser == null)
            {
                throw new ArgumentNullException("contactSynchroniser");
            }
            _contactSynchroniser = contactSynchroniser;
            if (contactService == null)
            {
                throw new ArgumentNullException("contactService");
            }
            _contactService = contactService;
        }

        #region Authentication 

        public ActionResult DisplayLogin()
        {
            ContactOperationStatus operationStatus = _contactService.GetContact();
            var viewContact = new ContactViewModel();
            if (operationStatus.Status)
            {
                viewContact.InjectFrom(operationStatus.Contact);
                viewContact.ExistingUserName = operationStatus.Contact.UserName;
                viewContact.ExistingEmail = operationStatus.Contact.Email;
                viewContact.Status = operationStatus.Status;
                viewContact.Message = operationStatus.Message;
                return PartialView("Login", viewContact);
            }
            return PartialView("DisplayError", operationStatus);
        }


        [HttpPost]
        public ActionResult Login([Bind(Prefix = "Login")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                ContactAuthenticationOperationStatus operationStatus = _contactService.Authenticate(login.Email,
                    login.Password);
                if (operationStatus.Status)
                {
                    return ReturnToCurrentPage();
                }
                    //otherwise try with email address (this check is optional, and reflects the fact
                    //that on the current system, some users have separate usernames and email addresses)
                if (login.Email.Contains("@"))
                {
                    operationStatus = _contactService.AuthenticateWithEmailAddress(login.Email, login.Password);
                    if (operationStatus.Status)
                    {
                        return ReturnToCurrentPage();
                    }
                }

                ModelState.AddModelError("Login.Email", "Your email address and password have not been recognised.");
                return CurrentUmbracoPage();
            }
            return CurrentUmbracoPage();
        }


        public ActionResult LogOff()
        {
            _contactService.LogOff();
            return ReturnToCurrentPage();
        }

        #endregion

        #region ContactDetails

        public ActionResult DisplayContact()
        {
            ContactOperationStatus operationStatus = _contactService.GetContact();
            var viewContact = new ContactViewModel();
            if (operationStatus.Status)
            {
                viewContact.InjectFrom(operationStatus.Contact);
                viewContact.ExistingUserName = operationStatus.Contact.UserName;
                viewContact.ExistingEmail = operationStatus.Contact.Email;
                if (!String.IsNullOrEmpty(Request["url"]))
                {
                    viewContact.Url = Request["url"];
                }
                if (!String.IsNullOrEmpty(Umbraco.Field("checkoutPage").ToString()))
                    viewContact.Url = Umbraco.Field("checkoutPage").ToString();

                viewContact.Titles = GetContactTitles();
                viewContact.Countries = GetCountries();
                viewContact.Status = operationStatus.Status;
                viewContact.Message = operationStatus.Message;
                return PartialView("MaintainContact", viewContact);
            }
            return PartialView("DisplayError", operationStatus);
        }


        [HttpPost]
        public ActionResult SaveContact([Bind(Prefix = "Contact")] ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                string url = Request["Contact.url"] ?? "";
                var updateContact = (Contact) new Contact().InjectFrom(contact);
                ContactOperationStatus saveStatus = _contactService.SaveContact(updateContact);
                return ReturnBasedOnStatus(saveStatus, url);
            }
            return CurrentUmbracoPage();
        }

        private List<ContactTitle> GetContactTitles()
        {
            return _contactService.GetContactTitles();
        }

        private List<Country> GetCountries()
        {
            return _contactService.GetCountries();
        }

        #endregion

        #region ContactPreferences

        [ChildActionOnly]
        public ActionResult CreatePreferencesForm()
        {
            ContactOperationStatus operationStatus = _contactService.GetContact();
            if (operationStatus.Status)
            {
                Contact contact = operationStatus.Contact;
                if (_contactSynchroniser.CanContactBeSynced(contact))
                {
                    ContactPreferencesOperationStatus preferenceStatus =
                        _contactSynchroniser.GetCurrentContactPreferences(contact.ExternalContactNumber);
                    if (preferenceStatus.Status)
                    {
                        ContactPreferences preferences = preferenceStatus.UpdatedPreferences;
                        return PartialView("MaintainPreferences", Mapper.Map<ContactPreferencesViewModel>(preferences));
                    }
                    //TODO:what to do if getting details fails - NOT this..
                    return CurrentUmbracoPage();
                }
                throw new NotImplementedException("No synchronisation is available");
            }
            return PartialView("DisplayErrorMessage", operationStatus);
        }

        [HttpPost]
        public ActionResult SavePreferences(ContactPreferencesViewModel preferences, string[] selectedPreferences,
            string[] allPreferences)
        {
            if (ModelState.IsValid)
            {
                //check if synchronisation is possible
                //TODO: need to do this properly outside of the controller..
                //if (ContactSynchroniser.CanContactBeSynced(contact))
                //{
                if (preferences.ExternalContactNumber > 0 & preferences.ExternalAddressNumber > 0)
                {
                    ContactPreferences preferencesToUpdate = getContactPreferencesToUpdate(preferences,
                        selectedPreferences, allPreferences);
                    ContactPreferencesOperationStatus savePreferenceStatus =
                        _contactSynchroniser.SaveContactPreferences(preferencesToUpdate);
                    if (savePreferenceStatus.Status)
                    {
                        return RedirectToCurrentUmbracoPage();
                    }
                    //TODO:what to do if getting details fails - NOT this..
                    return CurrentUmbracoPage();
                    //}
                }
            }
            return CurrentUmbracoPage();
        }

        private ContactPreferences getContactPreferencesToUpdate(ContactPreferencesViewModel preferences,
            string[] selectedPreferences, string[] allPreferences)
        {
            //first get list of selected prefences
            var listofSelectedPreferences = new List<Preference>();
            foreach (string preference in selectedPreferences)
            {
                string[] splitPreferences = preference.Split('/');
                listofSelectedPreferences.Add(new Preference
                {
                    Category = splitPreferences[0],
                    Value = splitPreferences[1]
                });
            }


            //manual mapping from ContactPreferencesViewModel to ContactPreferences
            var preferencesToUpdate = new ContactPreferences();
            preferencesToUpdate.Preferences = new List<Preference>();
            preferencesToUpdate.ExternalContactNumber = preferences.ExternalContactNumber;
            preferencesToUpdate.ExternalAddressNumber = preferences.ExternalAddressNumber;

            //loop through each preference
            foreach (string preference in allPreferences)
            {
                string[] splitPreferences = preference.Split('/');
                string preferenceCategory = splitPreferences[0];
                string preferenceValue = splitPreferences[1];
                //check if was actually selected..
                bool selected =
                    listofSelectedPreferences.Any(
                        s => s.Category.Equals(preferenceCategory) && s.Value.Equals(preferenceValue));
                preferencesToUpdate.Preferences.Add(new Preference
                {
                    Category = preferenceCategory,
                    Value = preferenceValue,
                    Selected = selected
                });
            }
            return preferencesToUpdate;
        }

        #endregion

        #region Membership

        [ChildActionOnly]
        public ActionResult DisplayMaintainMembershipForm()
        {
            //TODO: need to rewrite this completely!!
            ContactOperationStatus operationStatus = _contactService.GetContact();
            if (operationStatus.Status)
            {
                var membership = Mapper.Map<MembershipViewModel>(operationStatus.Contact);
                membership.AvailableMembershipTypes = GetMembershipTypes();
                return PartialView("MaintainMembership", membership);
            }
            return PartialView("DisplayErrorMessage", operationStatus);
        }

        [HttpPost]
        public ActionResult SaveMembership(MembershipViewModel membership)
        {
            if (ModelState.IsValid)
            {
                var createMembership = new Membership();
                createMembership.InjectFrom(membership);

                //TOD: moving this!
                //MembershipOperationStatus operationStatus =
                //    _contactService.CreateMembership(createMembership);
                //return ReturnBasedOnStatus(operationStatus,null);
                return null;
            }
            return CurrentUmbracoPage();
        }

        private List<MembershipType> GetMembershipTypes()
        {
            var listOfMembershipTypes = new List<MembershipType>();
            foreach (IPublishedContent membershipType in (CurrentPage.Children))
            {
                //TODO: this - doc type alias- shouldn't be hard-coded
                if (membershipType.DocumentTypeAlias == "FSCMembership")
                {
                    listOfMembershipTypes.Add(new MembershipType
                    {
                        Description = membershipType.Name,
                        Code = membershipType.Id.ToNullSafeString(),
                        Price = Convert.ToDecimal(membershipType.GetProperty("Price").Value)
                    });
                }
            }
            return listOfMembershipTypes;
        }

        #endregion
    }
}