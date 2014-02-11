using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.Interfaces.Contacts
{
    public interface IContactAuthenticationHandler
    {
        ContactAuthenticationOperationStatus Authenticate(string userName, string password);
        ContactAuthenticationOperationStatus AuthenticateWithEmailAddress(string email, string password);
        ContactAuthenticationOperationStatus CreateContact(Contact contact);
        void LogOff();
        bool IsCurrentUserLoggedIn();
        string GetUserId();
        string GetReferrerId();
        string GetUserName();
        int GetContactId();
        void SetContactId(string userName, int contactId);
        string GetPropertyValue(string propertyName);
        

    }
}