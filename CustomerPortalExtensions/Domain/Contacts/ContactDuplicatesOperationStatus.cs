using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain
{
    public class ContactDuplicatesOperationStatus : OperationStatus
    {
        public List<Contact> PossibleDuplicates { get; set; }
    }
}