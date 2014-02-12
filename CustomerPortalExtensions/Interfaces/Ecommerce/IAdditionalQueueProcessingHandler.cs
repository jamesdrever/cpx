using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IAdditionalQueueProcessingHandler
    {
        Order PerformAdditionalProcessing(Order order, Contact contact);
    }
}