using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerPortalExtensions.Interfaces.Email;
using CustomerPortalExtensions.Models;
using CustomerPortalExtensions.Models.Email;
using Omu.ValueInjecter;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.Controllers.Email
{
    public class CpxMaintainEmailSubscriptionsController : CpxBaseController
    {
        private readonly IEmailSubscriptionsService _emailNewsletterService;

        public CpxMaintainEmailSubscriptionsController(IEmailSubscriptionsService emailNewsletterService)
        {
            if (emailNewsletterService == null) throw new ArgumentNullException("emailNewsletterService");
            _emailNewsletterService = emailNewsletterService;
        }
        public override ActionResult Index(RenderModel model)
        {
            var operationStatus = _emailNewsletterService.GetEmailSubscriptions();

            if (operationStatus.Status)
            {
                var subscriptionsViewModel = new EmailSubscriptionsViewModel(model);
                subscriptionsViewModel.InjectFrom(operationStatus.EmailSubscriptions);
                return CurrentTemplate(subscriptionsViewModel);
            }
            else
            {
                return ReturnErrorView(operationStatus, model);
            }

        }

    }
}
