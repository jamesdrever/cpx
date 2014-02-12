using CustomerPortalExtensions.Domain.EmailNewsletter;

namespace CustomerPortalExtensions.Interfaces.Email
{
    public interface IEmailSubscriptionsService
    {
        EmailSubscriptionsOperationStatus SynchroniseSubscriptions(EmailSubscriptions subscriptions);
        EmailSubscriptionsOperationStatus GetEmailSubscriptions();
    }
}
