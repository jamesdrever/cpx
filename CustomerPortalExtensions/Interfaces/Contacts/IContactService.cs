using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Contacts;

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
        List<ContactTitle> GetContactTitles();
        List<Country> GetCountries();

        //TODO: where does this sit??
        /**
        MembershipOperationStatus CreateMembership(Membership membership);
        **/

    }
}