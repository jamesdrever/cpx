using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.Interfaces.Contacts
{
    public interface IContactSecondaryRepository
    {
        bool RepositoryAvailable();
        ContactOperationStatus Get(string userName);
    }
}