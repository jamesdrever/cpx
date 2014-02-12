using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.Contacts
{
    public class ContactOperationStatus : OperationStatus
    {
        public bool ContactCreated { get; set; }
        public bool ContactQueued { get; set; }
        public Contact Contact { get; set; }
    }
}