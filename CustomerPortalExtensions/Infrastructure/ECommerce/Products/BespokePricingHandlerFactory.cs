using CustomerPortalExtensions.Application.Ecommerce.Pricing;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Products
{
    public class BespokePricingHandlerFactory : IBespokePricingHandlerFactory
    {
        //TODO: use injected dependency for configuration or just use config string

        #region IShippingHandlerFactory Members



        public IBespokePricingHandler GetBespokePricingHandler(string config)
        {
            switch (config)
            {
                case "Default": return new DefaultBespokePricingHandler();
                case "Family": return new FamilyBespokePricingHandler();
                default: return new DefaultBespokePricingHandler();
            }
        }

        #endregion
    }
}