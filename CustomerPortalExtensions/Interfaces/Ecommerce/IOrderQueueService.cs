﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IOrderQueueService
    {
        OrderOperationStatus QueueOrder(int orderId);
        OrderOperationStatus UpdateOrderLine(int orderId, int orderLineId, string status);
        OrderQueueOperationStatus GetOrderQueue(string orderStatus, string location);
    }
}