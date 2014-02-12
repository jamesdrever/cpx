using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CustomerPortalExtensions.Models.Admin;
using Omu.ValueInjecter;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Interfaces.Contacts;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.Controllers.Admin
{
    public class CpxDisplayContactQueueController : CpxBaseController
    {
        private readonly IContactService _contactService;


        public CpxDisplayContactQueueController(IContactService contactService)
        {
            if (contactService == null)
            {
                throw new ArgumentNullException("contactService");
            }
            _contactService = contactService;
           
        }

        [Authorize(Roles = "cpxAdmin")]
        public override ActionResult Index(RenderModel model)
        {
            var operationStatus = _contactService.GetQueue();
            if (operationStatus.Status)
            {
                ContactQueueViewModel queueViewModel=new ContactQueueViewModel(model);
                queueViewModel.QueuedContacts = operationStatus.QueuedContacts;
                return CurrentTemplate(queueViewModel);
            }
            return ReturnErrorView(operationStatus,model);
        }


    }
}
