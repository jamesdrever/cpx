using CustomerPortalExtensions.Domain.EmailNewsletter;

namespace CustomerPortalExtensions.Interfaces.Email
{
    public interface IEmailSubscriptionConnector
    {
       void SetApiKey(string APIKey);
       void SetListId(string ListID);
       EmailSubscriptionsOperationStatus SynchroniseSubscriptions(EmailSubscriptions subscriptions);
       EmailSubscriptionsOperationStatus GetSubscriptions(string email);       
    }
}