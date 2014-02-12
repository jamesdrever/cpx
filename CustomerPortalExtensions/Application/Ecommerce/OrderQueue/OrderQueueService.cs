using System;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Omu.ValueInjecter;

namespace CustomerPortalExtensions.Application.Ecommerce.OrderQueue
{
    public class OrderQueueService : IOrderQueueService
    {
        private readonly IOrderQueue _orderQueue;
        private readonly IOrderCoordinator _orderCoordinator;
        private readonly IContactRepository _contactRepository;

        public OrderQueueService(IOrderQueue orderQueue, IOrderCoordinator orderCoordinator, IContactRepository contactRepository)
        {
            if (orderQueue == null)
            {
                throw new ArgumentNullException("orderQueue");
            }
            _orderQueue = orderQueue;
            if (orderCoordinator == null)
            {
                throw new ArgumentNullException("orderCoordinator");
            }
            _orderCoordinator = orderCoordinator;
            if (contactRepository == null)
            {
                throw new ArgumentNullException("contactRepository");
            }
            _contactRepository = contactRepository;
        }

        public OrderOperationStatus QueueOrder(int orderId)
        {
            OrderOperationStatus orderOperationStatus;
            try
            {
                orderOperationStatus = _orderCoordinator.GetOrder(orderId);
                if (orderOperationStatus.Status)
                {
                    var order = orderOperationStatus.Order;
                    var contactOperationStatus = _contactRepository.Get(order.ContactId);
                    if (!contactOperationStatus.Status)
                        return (OrderOperationStatus) new OrderOperationStatus().InjectFrom(contactOperationStatus);
                    orderOperationStatus = _orderQueue.QueueOrder(order, contactOperationStatus.Contact);
                }
                return orderOperationStatus;
            }
            catch (Exception e)
            {
                orderOperationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred updating the order line", e);
            }
            return orderOperationStatus;
        }

        public OrderQueueOperationStatus GetOrderQueue(string orderStatus, string location="")
        {
            DateTime rangeStart;
            DateTime rangeFinish;
            if (orderStatus != "QUE")
            {
                rangeStart = DateTime.Now.AddMonths(-2);
                rangeFinish = DateTime.Now.AddMonths(2);
            }
            else
            {
                rangeStart = DateTime.Now.AddYears(-100);
                rangeFinish = DateTime.Now.AddYears(100);
            }
            return _orderQueue.GetOrderQueue(orderStatus, rangeStart, rangeFinish,location);
        }



        public OrderOperationStatus UpdateOrderLine(int orderId, int orderLineId, string status)
        {

            OrderOperationStatus orderOperationStatus;
            try
            {
                orderOperationStatus = _orderCoordinator.GetOrder(orderId);
                if (orderOperationStatus.Status)
                {
                    var order = orderOperationStatus.Order;
                    //if there is a payment associated with a confirmed order line
                    //make the status CONP (confirmed, awaiting payment).
                    string paymentType = order.GetOrderLine(orderLineId).PaymentType;
                    if (status == "CONF" &&  (paymentType== "D"||paymentType=="F"))
                        status = "CONP";
                    order.GetOrderLine(orderLineId).OrderStatus = status;
                    orderOperationStatus = _orderCoordinator.SaveOrder(order,true);
                    if (orderOperationStatus.Status)
                        orderOperationStatus.Message = "Updated successfully!";
                }
            }
            catch (Exception e)
            {
                orderOperationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred updating the order line", e);
            }
            return orderOperationStatus;
    
        }
    }
}