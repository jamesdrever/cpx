using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.ECommerce
{
    public class OrderOperationStatus : OperationStatus
    {
        public Order Order { get; set; }
        public OrderLine OrderLine { get; set; }
        public bool OrderLineDeleted { get; set; }
    }

    public class OrdersOperationStatus : OperationStatus
    {
        public List<Order> Orders { get; set; }
    }

    public class OrderQueueOperationStatus : OperationStatus
    {
        public List<OrderQueueItem> OrderQueueItems { get; set; }
    }

    public class ProductOperationStatus : OperationStatus
    {
        public Product Product { get; set; }
    }

    public class ProductSummaryOperationStatus : OperationStatus
    {
        public Product Product { get; set; }
        public string OrderPage { get; set; }
    }

    public class ProductOptionOperationStatus : OperationStatus
    {
        public List<ProductOption> ProductOptions { get; set; }
    }
    public class PaymentOptionOperationStatus : OperationStatus
    {
        public List<PaymentOption> PaymentOptions { get; set; }
    }
}