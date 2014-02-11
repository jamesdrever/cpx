using System;
using CustomerPortalExtensions.Application.Ecommerce.Shipping;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Config;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Shipping
{
    public class ShippingHandlerFactory : IShippingHandlerFactory
    {
        //TODO: use injected dependency for configuration or just use config string

        #region IShippingHandlerFactory Members



        public IShippingHandler getShippingHandler(string config)
        {
            switch (config)
            {
                case "Default": return new DefaultShippingHandler();
                case "QtyAndLocation": return new QuantityAndLocationShippingHandler();
                default: return new DefaultShippingHandler();
            }
        }

        #endregion
    }
}