using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Discounts
{
    public class DefaultDiscountHandler : IDiscountHandler
    {

        #region IDiscountHandler Members

        public Order UpdateDiscount(Order order, Contact contact)
        {
            return order;
        }

        #endregion
    }
}