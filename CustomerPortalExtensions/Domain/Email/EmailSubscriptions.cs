using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.EmailNewsletter
{
    public class EmailSubscriptions
    {
        public EmailSubscriptions()
        {
            Subscriptions = new List<string>();
        }
        public string Email { get; set; }
        public string ForeName { get; set; }
        public string LastName { get; set; }
        public List<string> Subscriptions { get; set; }
    }



    public class EmailSubscriptionsOperationStatus : OperationStatus
    {
        public EmailSubscriptions EmailSubscriptions { get; set; }
        public bool SubscriptionStatus { get; set; }
    }
}