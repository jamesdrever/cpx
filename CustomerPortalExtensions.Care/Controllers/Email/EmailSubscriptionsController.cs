using CustomerPortalExtensions.Domain.EmailNewsletter;
using CustomerPortalExtensions.Interfaces.Email;
using CustomerPortalExtensions.Models;
using System;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Umbraco.Web.Mvc;

namespace CustomerPortalExtensions.Controllers.Email
{
    [PluginController("CustomerPortal")]
    public class EmailSubscriptionsSurfaceController : SurfaceController
    {
        private readonly IEmailSubscriptionsService _emailSubscriptionsService;

        public EmailSubscriptionsSurfaceController(IEmailSubscriptionsService emailSubscriptionService)
        {
            if (emailSubscriptionService == null) throw new ArgumentNullException("emailNewsletterService");
            _emailSubscriptionsService = emailSubscriptionService;
        }
 
        [HttpPost]
        public ActionResult SynchroniseSubscriptions(EmailSubscriptions subscriber)
        {
            if (ModelState.IsValid)
            {
                EmailSubscriptions subscriptions = (EmailSubscriptions) new EmailSubscriptions().InjectFrom(subscriber);
                var operationStatus = _emailSubscriptionsService.SynchroniseSubscriptions(subscriptions);
                TempData["Status"] = operationStatus.SubscriptionStatus;
                TempData["StatusMessage"] = operationStatus.Message;
                return RedirectToCurrentUmbracoPage();
            }
            else
            {
                return CurrentUmbracoPage();
            }
        }
    }
}
