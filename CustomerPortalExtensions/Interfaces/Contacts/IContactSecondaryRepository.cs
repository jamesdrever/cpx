using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.Interfaces.Contacts
{
    public interface IContactSecondaryRepository
    {
        bool RepositoryAvailable();
        ContactOperationStatus Get(string userName);
    }
}