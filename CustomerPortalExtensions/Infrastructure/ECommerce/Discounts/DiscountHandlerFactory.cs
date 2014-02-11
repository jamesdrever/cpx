using System;
using CustomerPortalExtensions.Application.Ecommerce.Discounts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Config;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Discounts
{
    public class DiscountHandlerFactory : IDiscountHandlerFactory
    {

        #region IDiscountHandlerFactory Members

        public IDiscountHandler getDiscountHandler(string config)
        {
            switch (config)
            {
                case "Default": return new DefaultDiscountHandler();
                case "Qty":
 
                    return new QuantityDiscountHandler(20, 10);
                default: return new DefaultDiscountHandler();
            }
        }

        #endregion
    }
}