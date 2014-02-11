using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Interfaces.Contacts;

namespace CustomerPortalExtensions.Infrastructure.Contacts
{
    public class DefaultContactSecondaryRepository : IContactSecondaryRepository
    {
        public bool RepositoryAvailable()
        {
            return false;
        }

        public Domain.ContactOperationStatus Get(string userName)
        {
            throw new NotImplementedException();
        }
    }
}