using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.Email;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Domain.EmailNewsletter;
using CustomerPortalExtensions.Interfaces;
using CustomerPortalExtensions.Domain;
using Omu.ValueInjecter;

namespace CustomerPortalExtensions.Application.Email
{
    public class EmailSubscriptionsService : IEmailSubscriptionsService
    {
        private readonly IEmailSubscriptionConnector _emailSubscriptionConnector;
        private readonly IContactService _contactService;
        private readonly IConfigurationService _config;

        public EmailSubscriptionsService(IEmailSubscriptionConnector emailSubscriptionConnector, IContactService contactService, IConfigurationService config)
        {
            if (emailSubscriptionConnector == null)
            {
                throw new ArgumentNullException("emailNewsletterConnector");
            }
            emailSubscriptionConnector.SetApiKey(config.GetConfiguration().EmailNewsletterAPIKey);
            emailSubscriptionConnector.SetListId(config.GetConfiguration().EmailNewsletterListID);
            this._emailSubscriptionConnector = emailSubscriptionConnector;
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this._config = config;
            if (contactService == null)
            {
                throw new ArgumentNullException("contactService");
            }
            this._contactService = contactService;
        }


        public EmailSubscriptionsOperationStatus SynchroniseSubscriptions(EmailSubscriptions subscriptions)
        {
            var operationStatus=new EmailSubscriptionsOperationStatus();
            try
            {
                operationStatus = _emailSubscriptionConnector.SynchroniseSubscriptions(subscriptions);
                if (operationStatus.Status)
                    operationStatus.Message = "Your subscriptions have been updated successfully.";
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<EmailSubscriptionsOperationStatus>
                    .CreateFromException("An error has occurred while retrieving the email subscriptions", e);

            }
            return operationStatus;  
        }



        public EmailSubscriptionsOperationStatus GetEmailSubscriptions(string email) 
        {
            var operationStatus=new EmailSubscriptionsOperationStatus();
            try
            {
                operationStatus = _emailSubscriptionConnector.GetSubscriptions(email);
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<EmailSubscriptionsOperationStatus>
                    .CreateFromException("An error has occurred while retrieving the email subscriptions", e);

            }
            return operationStatus;  
        }


        public EmailSubscriptionsOperationStatus GetEmailSubscriptions()
        {
            var operationStatus=new EmailSubscriptionsOperationStatus();
            try
            {
                ContactOperationStatus contactOperationStatus = _contactService.GetContact();
                if (!contactOperationStatus.Status)
                {
                    //TODO: what is required here?
                    /**
                    returnContact = new EmailNewsletterContact();
                    returnContact.Status = false;
                    returnContact.Message = "No user currently logged in";
                    **/
                    return
                        (EmailSubscriptionsOperationStatus)
                        new EmailSubscriptionsOperationStatus().InjectFrom(contactOperationStatus);
                }
                Contact contact = contactOperationStatus.Contact;
                operationStatus = _emailSubscriptionConnector.GetSubscriptions(contact.Email);


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