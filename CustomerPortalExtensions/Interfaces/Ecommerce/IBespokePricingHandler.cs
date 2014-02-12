using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IBespokePricingHandler
    {
        Product CreateBespokePrice(Product product, int quantity);
    }
}