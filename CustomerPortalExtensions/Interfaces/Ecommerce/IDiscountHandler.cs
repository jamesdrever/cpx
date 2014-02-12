using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IDiscountHandler
    {
        Order UpdateDiscount(Order order,Contact contact);
    }
}