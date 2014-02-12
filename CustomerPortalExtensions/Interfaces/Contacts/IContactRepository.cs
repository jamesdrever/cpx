
using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.Interfaces.Contacts
{
    public interface IContactRepository
    {
        ContactOperationStatus Save(Contact contact);
        ContactOperationStatus Get(int contactId);
        ContactQueueOperationStatus GetQueue();
    }
}
