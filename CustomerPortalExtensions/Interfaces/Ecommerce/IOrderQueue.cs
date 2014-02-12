using System;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IOrderQueue
    {
        OrderOperationStatus QueueOrder(Order order, Contact contact);
        OrderQueueOperationStatus GetOrderQueue(string orderStatus, DateTime rangeStart, DateTime rangeFinish, string location);
    }
}