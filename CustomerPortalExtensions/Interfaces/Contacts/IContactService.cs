using System.Collections.Generic;
using CustomerPortal.Domain.Contacts;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Membership;

namespace CustomerPortalExtensions.Interfaces.Contacts
{
    public interface IContactService
    {
       
        ContactOperationStatus SaveContact(Contact contact);
        ContactOperationStatus UpdateExternalContactNumber(int contactId,int externalcontactNumber);
        ContactOperationStatus CreateContactFromQueue(int contactId);
        ContactQueueOperationStatus GetQueue();
        ContactOperationStatus UpdateContactFromQueue(int contactId, int externalContactNumber);
        ContactOperationStatus GetContact();
        ContactAuthenticationOperationStatus Authenticate(string userName,string password);
        ContactAuthenticationOperationStatus AuthenticateWithEmailAddress(string email, string password);
        bool IsCurrentUserLoggedIn();
        void LogOff();
        
        //TODO: where does this sit??
        /**
        MembershipOperationStatus CreateMembership(Membership membership);
        **/
        List<ContactTitle> GetContactTitles();
        List<Country> GetCountries();


        /**
        string GetDeliveryCountry();
        bool IsUniqueUserName(string UserName);
        bool IsUniqueEmail(string Email);
        
        Contact GetContact(string UserName);
        Contact GetContact(int UserID);
         **/
    }
}