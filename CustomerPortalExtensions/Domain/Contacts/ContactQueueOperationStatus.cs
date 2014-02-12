using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.Contacts
{
    public class ContactQueueOperationStatus : OperationStatus
    {
        public List<ContactQueueItem> QueuedContacts { get; set; }
    }
}