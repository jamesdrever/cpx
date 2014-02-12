using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Membership;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces.Synchronisation;
using System;
using System.Collections.Generic;

namespace CustomerPortalExtensions.Infrastructure.Synchronisation
{
    public class DefaultContactSynchroniser : IContactSynchroniser
    {
        private readonly Database _database;

        public DefaultContactSynchroniser(Database database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            _database = database;
        }

        bool IContactSynchroniser.CanContactBeSynced(Contact contact)
        {
            return false;
        }

        ContactOperationStatus IContactSynchroniser.SaveContact(Contact contact)
        {
            throw new NotImplementedException();
        }

        ContactOperationStatus IContactSynchroniser.GetContact(int externalContactNumber)
        {
            throw new NotImplementedException();
        }


        public ContactPreferencesOperationStatus SaveContactPreferences(ContactPreferences preferences)
        {
            throw new NotImplementedException();
        }


        ContactPreferencesOperationStatus IContactSynchroniser.GetContactPreferences(int externalContactNumber)
        {
            throw new NotImplementedException();
        }


        public ContactPreferencesOperationStatus GetCurrentContactPreferences(int externalContactNumber)
        {
            throw new NotImplementedException();
        }


        public CustomerPortalExtensions.Domain.MembershipOperationStatus CreateMembership(Membership membership)
        {
            throw new NotImplementedException();
        }


        public ContactDuplicatesOperationStatus GetDuplicates(string title, string surname, string postcode)
        {
            throw new NotImplementedException();
        }


        public ContactOperationStatus CreateContactFromQueue(int contactId)
        {
            throw new NotImplementedException();
        }

        public ContactOperationStatus UpdateContactFromQueue(int contactId, int externalContactNumber)
        {
            throw new NotImplementedException();
        }


        public bool SynchronisationAvailable()
        {
           return false;
        }


        public List<ContactTitle> GetContactTitles()
        {
            return _database.Fetch<ContactTitle>("SELECT title AS code,title AS name FROM cpxTitles");
        }


        public List<Country> GetCountries()
        {
            return _database.Fetch<Country>("SELECT country AS code,country_desc AS name FROM cpxCountries ORDER BY country_desc");
        }
    }
}