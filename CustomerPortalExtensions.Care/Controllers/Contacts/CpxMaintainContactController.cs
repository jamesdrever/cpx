using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CustomerPortal.Domain.Contacts;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Models;
using CustomerPortalExtensions.Models.Contacts;
using Omu.ValueInjecter;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.Controllers.Contacts
{
    public class CpxMaintainContactController : CpxBaseController
    {
        private readonly IContactService _contactService;


        public CpxMaintainContactController(IContactService contactService)
        {

            if (contactService == null)
            {
                throw new ArgumentNullException("contactService");
            }
            _contactService = contactService;
        }


        public override ActionResult Index(RenderModel model)
        {
            ContactOperationStatus operationStatus = _contactService.GetContact();
            ContactViewModel viewContact = new ContactViewModel(model);
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
                return CurrentTemplate(viewContact);
            }
            return ReturnErrorView(operationStatus, model);

        }

        private List<ContactTitle> GetContactTitles()
        {
            return _contactService.GetContactTitles();
        }
        private List<Country> GetCountries()
        {
            return _contactService.GetCountries();
        }

    }
}
