using System;
using System.Web.Mvc;
using CustomerPortalExtensions.Interfaces.Contacts;
using Umbraco.Web.Mvc;

namespace CustomerPortalExtensions.MVC.Controllers.Admin
{
    [PluginController("CustomerPortal")]
    public class ContactQueueSurfaceController : CpxBaseSurfaceController
    {
        private readonly IContactService _contactService;

        public ContactQueueSurfaceController(IContactService contactService)
        {
            if (contactService == null)
            {
                throw new ArgumentNullException("contactService");
            }
            _contactService = contactService;
        }

   

        [HttpPost]
        [Authorize(Roles = "cpxAdmin")]
        public ActionResult CreateContact(int contactId)
        {
            var opStatus=_contactService.CreateContactFromQueue(contactId);
            return ReturnBasedOnStatus(opStatus, null);
        }

        [HttpPost]
        [Authorize(Roles = "cpxAdmin")]
        public ActionResult UpdateContact(int contactId,int contactDupeId)
        {
            //TODO: error handling..
            _contactService.UpdateContactFromQueue(contactId, contactDupeId);
            return RedirectToCurrentUmbracoPage();
        }

    }
}
