using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Ecommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IBespokePricingHandler
    {
        Product CreateBespokePrice(Product product, int quantity);
    }
}