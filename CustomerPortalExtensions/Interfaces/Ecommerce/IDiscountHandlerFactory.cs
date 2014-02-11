using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IDiscountHandlerFactory
    {
        IDiscountHandler getDiscountHandler(string config);
    }
}