using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IBespokePricingHandlerFactory
    {
        IBespokePricingHandler GetBespokePricingHandler(string config);
    }
}