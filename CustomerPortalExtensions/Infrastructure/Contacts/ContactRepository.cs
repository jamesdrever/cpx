using System;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces;
using CustomerPortalExtensions.Interfaces.Contacts;

namespace CustomerPortalExtensions.Infrastructure.Contacts
{
    public class ContactRepository : IContactRepository
    {
        private readonly Database _database;

        public ContactRepository(Database database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            _database = database;
        }


        public ContactOperationStatus Save(Contact contact)
        {
            var operationStatus = new ContactOperationStatus();
            try
            {
                if (_database.IsNew(contact))
                {
                    contact.ContactId=(int)_database.Insert(contact);
                    operationStatus.ContactCreated = true;
                }
                else
                {
                    _database.Update(contact);
                }
                operationStatus.Contact = contact;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus.LogFailedOperation(e);
                //operationStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                //    .CreateFromException("An error has occurred retrieving the contact from the queue", e);

            }
            return operationStatus;

        }

        public ContactOperationStatus Get(int contactId)
        {
            var operationStatus = new ContactOperationStatus();
            try
            {
                operationStatus.Contact = _database.SingleOrDefault<Contact>("SELECT * FROM cpxContact WHERE ContactId=@0",contactId);
                operationStatus.Status =(operationStatus.Contact!=null);
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("An error has occurred retrieving the contact from the queue", e);

            }
            return operationStatus;
        }


        public ContactQueueOperationStatus GetQueue()
        {
            var operationStatus = new ContactQueueOperationStatus();
            try
            {
                operationStatus.QueuedContacts = _database.Fetch<ContactQueueItem>("SELECT * FROM cpxContact WHERE ExternalContactNumber=0");
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactQueueOperationStatus>
                    .CreateFromException("An error has occurred retrieving the contact from the queue", e);

            }
            return operationStatus;
        }
    }
}