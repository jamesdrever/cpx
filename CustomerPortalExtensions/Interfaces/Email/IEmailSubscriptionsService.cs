
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomerPortalExtensions.Domain.EmailNewsletter;

namespace CustomerPortalExtensions.Interfaces.Email
{
    public interface IEmailSubscriptionsService
    {
        EmailSubscriptionsOperationStatus SynchroniseSubscriptions(EmailSubscriptions subscriptions);
        EmailSubscriptionsOperationStatus GetEmailSubscriptions();
        //EmailNewsletterContact GetContact(string email);
    }
}
