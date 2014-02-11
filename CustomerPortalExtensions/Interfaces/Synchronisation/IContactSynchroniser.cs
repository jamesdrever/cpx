using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortal.Domain.Contacts;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Membership;

namespace CustomerPortalExtensions.Interfaces.Synchronisation
{
    public interface IContactSynchroniser
    {
       bool CanContactBeSynced(Contact contact);
       bool SynchronisationAvailable();
       ContactOperationStatus SaveContact(Contact contact);
       ContactOperationStatus GetContact(int externalContactNumber);
       List<ContactTitle> GetContactTitles();
       List<Country> GetCountries();
       MembershipOperationStatus CreateMembership(Membership membership);
       ContactPreferencesOperationStatus GetContactPreferences(int externalContactNumber);
       ContactPreferencesOperationStatus GetCurrentContactPreferences(int externalContactNumber);
       ContactPreferencesOperationStatus SaveContactPreferences(ContactPreferences preferences);
       ContactDuplicatesOperationStatus GetDuplicates(string title, string surname, string postcode);
       
       /** 
       ContactOperationStatus CreateContactFromQueue(int contactId);
       ContactOperationStatus UpdateContactFromQueue(int contactId, int externalContactNumber);
        **/
    }
}