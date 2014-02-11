using System;
using System.Collections.Generic;
using CustomerPortal.Infrastructure.Services;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Synchronisation;
using Omu.ValueInjecter;

namespace CustomerPortalExtensions.Application.Contacts
{
    public class ContactService : IContactService
    {
        private readonly IContactAuthenticationHandler _authenticationHandler;
        private readonly IContactRepository _contactRepository;
        private readonly IContactSecondaryRepository _secondaryContactRepository;
        private readonly IContactSynchroniser _contactSynchroniser;
        private readonly IOrderRepository _orderRepository;

        public ContactService(IContactAuthenticationHandler authenticationHandler,IContactRepository contactRepository, 
            IContactSecondaryRepository secondaryContactRepository,
            IContactSynchroniser contactSynchroniser,
            IOrderRepository orderRepository
            )
        {
            if (contactRepository == null)
            {
                throw new ArgumentNullException("contactRepository");
            }
            _contactRepository = contactRepository;
            if (secondaryContactRepository == null)
            {
                throw new ArgumentNullException("secondaryContactRepository");
            }
            _secondaryContactRepository = secondaryContactRepository;
            
            
            if (authenticationHandler == null)
            {
                throw new ArgumentNullException("authenticationHandler");
            }
            _authenticationHandler = authenticationHandler;
            if (contactSynchroniser == null)
            {
                throw new ArgumentNullException("contactSynchroniser");
            }
            _contactSynchroniser = contactSynchroniser;
            if (orderRepository == null) throw new ArgumentNullException("orderRepository");
            _orderRepository = orderRepository;
        }

        #region Authentication

        public ContactAuthenticationOperationStatus Authenticate(string userName, string password)
        {
            var operationStatus = _authenticationHandler.Authenticate(userName, password);
            //if (operationStatus.Status)
            //{
            //    
            //    var orderOperationStatus = UpdateOrdersAfterAuthentication();
            //    if (!orderOperationStatus.Status)
            //        operationStatus= (ContactAuthenticationOperationStatus)operationStatus.InjectFrom(orderOperationStatus);
            //}
            return operationStatus;
        }


        public ContactAuthenticationOperationStatus AuthenticateWithEmailAddress(string email, string password)
        {
            var operationStatus = _authenticationHandler.AuthenticateWithEmailAddress(email, password);
            //if (operationStatus.Status)
            //{
            //    var orderOperationStatus = UpdateOrdersAfterAuthentication();
            //    if (!orderOperationStatus.Status)
            //        operationStatus = (ContactAuthenticationOperationStatus)operationStatus.InjectFrom(orderOperationStatus);
            //}
            return operationStatus;
        }
        /**
        private OrdersOperationStatus UpdateOrdersAfterAuthentication()
        {
            var operationStatus = new OrdersOperationStatus();
            var contactOperationStatus = GetContact();
            if (!contactOperationStatus.Status)
                return (OrdersOperationStatus) operationStatus.InjectFrom(contactOperationStatus);
            var contact = contactOperationStatus.Contact;
            //this will refresh the orders with the new contact id
            return _orderRepository.GetOrders(contact);
        }
        **/
        public void LogOff()
        {
            _authenticationHandler.LogOff();
        }

        public bool IsCurrentUserLoggedIn()
        {
            return _authenticationHandler.IsCurrentUserLoggedIn();
        }

        #endregion

        public ContactOperationStatus SaveContact(Contact contact)
        {
            if (String.IsNullOrEmpty(contact.UserName))
                contact.UserName = contact.Email;
            
            //if the user isn't already logged in
            if (contact.ContactId==0)
            {
                //create an authentication
                var authenticationOperationStatus=_authenticationHandler.CreateContact(contact);
                if (!authenticationOperationStatus.Status)
                {
                    return
                        (ContactOperationStatus) new ContactOperationStatus().InjectFrom(authenticationOperationStatus);
                }
                
            }

            //save the contact locally
            ContactOperationStatus operationStatus = _contactRepository.Save(contact);

            if (operationStatus.Status)
            {
                if (operationStatus.ContactCreated)
                    _authenticationHandler.SetContactId(contact.UserName,contact.ContactId);
       
                contact = operationStatus.Contact;

                //if the contact can be synced
                if (_contactSynchroniser.SynchronisationAvailable())
                {
                    //sync it
                    operationStatus = _contactSynchroniser.SaveContact(contact);
                }
                operationStatus.Message = "Your details have been successfully updated.";
            }
            return operationStatus;

        }
        

        public ContactOperationStatus GetContact()
        {
            var operationStatus=new ContactOperationStatus();
            int contactId = 0;
            try
            {
                //if the user logged in?
                if (_authenticationHandler.IsCurrentUserLoggedIn())
                {
                    //if yes, get the ContactId
                    contactId = _authenticationHandler.GetContactId();
                    //if there is one, get the contact
                    if (contactId>0)
                        operationStatus = _contactRepository.Get(contactId);
                    else
                    {
                        operationStatus.Status = false;
                    }
                    //if no luck with contact repository, try secondary repository, if it is active 
                    if (!operationStatus.Status&&_secondaryContactRepository.RepositoryAvailable())
                    {
                        string userName = _authenticationHandler.GetUserName();
                        operationStatus = _secondaryContactRepository.Get(userName);
                        if (operationStatus.Status)
                        {
                            operationStatus = _contactRepository.Save(operationStatus.Contact);
                            if (operationStatus.Status)
                                _authenticationHandler.SetContactId(operationStatus.Contact.UserName,operationStatus.Contact.ContactId);
                        }
                    }
                    if (operationStatus.Status)
                    {
                        Contact contact = operationStatus.Contact;
                        
                        //check if the UserId for the contact needs updating
                        string currentUserId = _authenticationHandler.GetUserId();
                        string currentReferrerdId = _authenticationHandler.GetReferrerId();
                        if (contact.UserId != currentUserId||contact.ReferrerId!=currentReferrerdId)
                        {
                            contact.UserId = currentUserId;
                            contact.ReferrerId = currentReferrerdId;
                            _contactRepository.Save(contact);

                        }
                        contact.ReferrerId = _authenticationHandler.GetReferrerId();


                        if (_contactSynchroniser.SynchronisationAvailable() &&
                            _contactSynchroniser.CanContactBeSynced(contact))
                        {
                            operationStatus = _contactSynchroniser.GetContact(contact.ExternalContactNumber);
                            if (operationStatus.Status)
                            {
                                //merge in changes from sync
                                contact.InjectFrom<IgnoreNulls>(operationStatus.Contact);
                                contact.ContactId = contactId;
                                //update the repository with the changes
                                operationStatus = _contactRepository.Save(contact);
                            }
                        }
                    }
                }
                else
                {
                    Contact contact = new Contact();
                    contact.UserId = _authenticationHandler.GetUserId();
                    contact.ReferrerId = _authenticationHandler.GetReferrerId();
                    contact.Status = false;
                    operationStatus.Contact = contact;
                    operationStatus.Status = true;
                }
                
                
            }
            catch (Exception e)
            {
                operationStatus.LogFailedOperation(e);
                //operationStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                //       .CreateFromException("An error has occurred getting the contact", e);
                
            }
            return operationStatus;
        }

        public List<CustomerPortal.Domain.Contacts.ContactTitle> GetContactTitles()
        {
            return _contactSynchroniser.GetContactTitles();
        }


        public List<CustomerPortal.Domain.Contacts.Country> GetCountries()
        {
            return _contactSynchroniser.GetCountries();
        }

        public ContactOperationStatus UpdateExternalContactNumber(int contactId, int externalContactNumber)
        {
            var operationStatus=new ContactOperationStatus();
            try
            {
                operationStatus = _contactRepository.Get(contactId);
                if (operationStatus.Status)
                {
                    var contact = operationStatus.Contact;
                    contact.ExternalContactNumber = externalContactNumber;
                    operationStatus = _contactRepository.Save(contact);
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("An error has occurred getting the contact", e);

            }
            return operationStatus;
        }



        public ContactOperationStatus CreateContactFromQueue(int contactId)
        {
            var operationStatus = new ContactOperationStatus();
            try
            {
                operationStatus = _contactRepository.Get(contactId);
                if (operationStatus.Status)
                {
                    var contact = operationStatus.Contact;
                    //sync the queued contact
                    operationStatus = _contactSynchroniser.SaveContact(operationStatus.Contact);
                    if (operationStatus.Status)
                    {
                        //then save locally (to update the external contact/address numbers)
                        operationStatus = _contactRepository.Save(operationStatus.Contact);
                    }
                    return operationStatus;
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("An error has occurred getting the contact", e);

            }
            return operationStatus;
        }

        public ContactOperationStatus UpdateContactFromQueue(int contactId, int externalContactNumber)
        {
            var operationStatus = new ContactOperationStatus();
            try
            {
                operationStatus = _contactRepository.Get(contactId);
                if (operationStatus.Status)
                {
                    var contact = operationStatus.Contact;
                    contact.ExternalContactNumber = externalContactNumber;
                    //sync the queued contact
                    operationStatus = _contactSynchroniser.SaveContact(operationStatus.Contact);
                    if (operationStatus.Status)
                    {
                        //then save locally (to update the external contact/address numbers)
                        operationStatus = _contactRepository.Save(operationStatus.Contact);
                    }
                    return operationStatus;
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("An error has occurred getting the contact", e);

            }
            return operationStatus;
        }

        //TODO: where does this sit??
        //needs access to ecommerce but also contact syncing!
        /**
        public MembershipOperationStatus CreateMembership(Membership membership)
        {
            MembershipOperationStatus operationStatus=new MembershipOperationStatus();
            if (membership.MembershipPaymentMethod == "CC")
            {
                int productId = Convert.ToInt32(membership.MembershipType);
                OrderOperationStatus orderOperationStatus = _ecommerceService.AddProductToOrder(productId, 1);
                if (orderOperationStatus.Status)
                {
                    orderOperationStatus.Message = "Your membership has been successfully added to your basket.";
                }
                return (MembershipOperationStatus) operationStatus.InjectFrom(orderOperationStatus);
            }
            if (membership.MembershipPaymentMethod == "DD")
            {
                if (_contactSynchroniser.SynchronisationAvailable())
                {
                    Contact contact = new Contact();
                    contact.InjectFrom(membership);
                    if (_contactSynchroniser.CanContactBeSynced(contact))
                    {
                        //either add direct to CARE
                        operationStatus= _contactSynchroniser.CreateMembership(membership);
                    }
                    else
                    {
                        //or add to the contact queue...
                    }
                }
                else
                {
                    throw new NotImplementedException("No synchronisation is available");
                }

            }
            return operationStatus;

        }
        **/

       

        public ContactQueueOperationStatus GetQueue()
        {
            var operationStatus = _contactRepository.GetQueue();

            if (operationStatus.Status)
            {
                //TODO: sort this out!!!


                var queuedContacts = operationStatus.QueuedContacts;

                for(int i=0;i<queuedContacts.Count;i++)
                {
                    var dupeOperationStatus = _contactSynchroniser.GetDuplicates(queuedContacts[i].Title, queuedContacts[i].LastName,
                                                            queuedContacts[i].Postcode);
                    if (dupeOperationStatus.Status)
                    {
                        queuedContacts[i].PossibleDuplicates = dupeOperationStatus.PossibleDuplicates;
                    }
                }
                operationStatus.QueuedContacts = queuedContacts;
            }
            return operationStatus;
        }
    }
}