using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces.ECommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.OrderQueue
{
    public class DefaultAdditionalQueueProcessingHandler : IAdditionalQueueProcessingHandler
    {
        public Order PerformAdditionalProcessing(Order order, Domain.Contacts.Contact contact)
        {
            return order;
        }
    }
}