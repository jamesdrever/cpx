using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IOrderCoordinator
    {
        OrderOperationStatus GetOrder(Contact contact, int orderIndex);
        OrderOperationStatus GetOrder(int orderId);
        OrderOperationStatus SaveOrder(Order order, bool updateOrderLines);
        Order UpdateOrderWithShippingAndDiscounts(Order order, Contact contact);
        string GetOrderPage(int orderIndex);
    }
}