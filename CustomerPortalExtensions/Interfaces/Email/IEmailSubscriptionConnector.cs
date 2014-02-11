using System.Web.Services.Description;
using CustomerPortalExtensions.Domain.EmailNewsletter;
using System.ComponentModel.Composition;

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