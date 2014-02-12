using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IShippingHandler
    {
        Order UpdateShipping(Order order, Contact contact);
    }
}