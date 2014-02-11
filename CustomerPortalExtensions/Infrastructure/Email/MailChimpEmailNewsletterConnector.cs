using System;
using System.Collections.Generic;
using System.Linq;
using MailChimp;
using MailChimp.Types;
using CustomerPortalExtensions.Interfaces.Email;
using CustomerPortalExtensions.Domain.EmailNewsletter;
using CustomerPortalExtensions.Domain.Operations;
using System.ComponentModel.Composition;

namespace CustomerPortalExtensions.Infrastructure.Email
{
    [Export(typeof(IEmailSubscriptionConnector))]
    public class MailChimpEmailSubscriptionConnector : IEmailSubscriptionConnector
    {
        private string _apiKey;
        private string _listId;
        public void SetApiKey(string APIKey) { _apiKey = APIKey; }
        public void SetListId(string ListID) { _listId=ListID; }
        public EmailSubscriptionsOperationStatus SynchroniseSubscriptions(EmailSubscriptions subscriptions)
        {
            var operationStatus=new EmailSubscriptionsOperationStatus();
            try
            {
                MCApi api = new MCApi(_apiKey, false);
                var subscribeOptions =
                 new Opt<List.SubscribeOptions>(
                     new List.SubscribeOptions
                     {
                         SendWelcome = true,
                         UpdateExisting = true,
                         DoubleOptIn = (subscriptions.Subscriptions.Count() != 0), //retain double optin for new subscribers
                         ReplaceInterests = true
                     });

                var groupings = new List<List.Grouping>() 
                        {
                            new List.Grouping("Signed Up As", subscriptions.Subscriptions.ToArray()),
                        };

                var merges =
                    new Opt<List.Merges>(
                    new List.Merges
					{
						{"FNAME", subscriptions.ForeName},
                        {"LNAME",subscriptions.LastName},
                        {"GROUPINGS",groupings}
					});

                operationStatus.Status = api.ListSubscribe(_listId, subscriptions.Email, merges, subscribeOptions);
                operationStatus.SubscriptionStatus = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<EmailSubscriptionsOperationStatus>
                       .CreateFromException("An error has occurred while syncing the email subscriptions", e);
                operationStatus.SubscriptionStatus = false;

            }
            return operationStatus;
        }

        public EmailSubscriptionsOperationStatus GetSubscriptions(string email)
        {
            var operationStatus=new EmailSubscriptionsOperationStatus();
            try
            {


                MCApi api = new MCApi(_apiKey, false);
                var subscriptions = new EmailSubscriptions();
                subscriptions.Email = email;
                var testResults = api.ListMemberInfo(_listId, email);
                if (testResults.Success.Value == 1)
                {
                    var merges = testResults.Data[0].Merges;

                    foreach (var merge in merges)
                    {
                        if (merge.Key == "GROUPINGS")
                        {
                            var grouping = (List.Grouping[]) merge.Value;
                            if (grouping.Count()>0)
                                subscriptions.Subscriptions = grouping[0].Groups.ToList();
                            operationStatus.Status = true;
                            operationStatus.SubscriptionStatus = true;
                        }
                        if (merge.Key == "FNAME")
                        {
                            subscriptions.ForeName = merge.Value.ToString();
                        }
                        if (merge.Key == "LNAME")
                        {
                            subscriptions.LastName = merge.Value.ToString();
                        }
                    }
                    operationStatus.SubscriptionStatus = true;
                }
                else
                {                   
                    operationStatus.SubscriptionStatus = false;
                    operationStatus.Message = testResults.ErrorsData[0].ToString();
                }
                operationStatus.Status = true;
                operationStatus.EmailSubscriptions = subscriptions;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<EmailSubscriptionsOperationStatus>
                       .CreateFromException("An error has occurred while retrieving the email subscriptions", e);

            }
            return operationStatus;          
        }
    }
}