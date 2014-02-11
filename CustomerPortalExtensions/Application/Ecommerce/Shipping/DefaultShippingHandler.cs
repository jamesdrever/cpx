using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Shipping
{
    public class DefaultShippingHandler : IShippingHandler
    {

        #region IShippingHandler Members

        public Order UpdateShipping(Order order, Contact contact)
        {
            return order;
        }

        #endregion
    }
}