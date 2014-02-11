using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IBespokePricingHandlerFactory
    {
        IBespokePricingHandler GetBespokePricingHandler(string config);
    }
}