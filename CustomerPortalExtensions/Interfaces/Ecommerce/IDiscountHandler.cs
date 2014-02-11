using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IDiscountHandler
    {
        Order UpdateDiscount(Order order,Contact contact);
    }
}