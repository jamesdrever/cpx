using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Pricing
{
    public class DefaultBespokePricingHandler : IBespokePricingHandler
    {
        public Product CreateBespokePrice(Product product,int quantity)
        {
            return product;
        }

    }
}