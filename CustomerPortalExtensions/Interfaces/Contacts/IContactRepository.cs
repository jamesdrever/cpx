using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.Interfaces.Contacts
{
    public interface IContactRepository
    {
        ContactOperationStatus Save(Contact contact);
        ContactOperationStatus Get(int contactId);
        ContactQueueOperationStatus GetQueue();
    

/**
        ContactCredentials GetContactCredentials();
        Contact GetContact(int UserId);
        bool IsUniqueUserName(string userName);
        bool IsUniqueEmail(string email);
        string GetDeliveryCountry();
        bool CurrentUserLogggedIn();
        bool Authenticate(string userName,string password);
        bool AuthenticateWithEmailAddress(string email, string password);
        void LogOff();
 **/
    }
}
