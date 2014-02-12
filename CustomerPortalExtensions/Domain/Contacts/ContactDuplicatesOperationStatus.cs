using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.Contacts
{
    public class ContactDuplicatesOperationStatus : OperationStatus
    {
        public List<Contact> PossibleDuplicates { get; set; }
    }
}