using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{

    public interface IOrderRepository
    {
        OrderOperationStatus GetOrder(Contact contact);
        OrderOperationStatus GetOrder(Contact contact,int cartIndex);
        OrdersOperationStatus GetOrders(Contact contact);
        OrderOperationStatus SaveOrder(Order order);
        OrderOperationStatus SaveOrder(Order order, bool updateOrderLines);
        OrderOperationStatus GetOrder(int orderId);

    }
}