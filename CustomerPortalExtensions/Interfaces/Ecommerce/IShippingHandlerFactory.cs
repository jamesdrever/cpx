using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Interfaces.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IShippingHandlerFactory
    {
        IShippingHandler getShippingHandler(string config);
    }
}