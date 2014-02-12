using System;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Orders
{

    public class OrderRepository : IOrderRepository
    {
        private readonly Database _database;

        public OrderRepository(Database database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            _database = database;
        }

        public OrderOperationStatus GetOrder(Contact contact)
        {
            return GetOrder(contact, 1);
        }

        public OrderOperationStatus GetOrder(Contact contact, int orderIndex)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                var sql = Sql.Builder
                    .Select("*")
                    .From("cpxOrder");

                sql.Append("WHERE status='PROV' AND OrderIndex=@0",orderIndex);

                if (contact.ContactId > 0)
                {
                    sql.Append("AND ContactId=@0", contact.ContactId);
                }
                else
                    sql.Append("AND UserId=@0", contact.UserId);

                var order =
                    _database.SingleOrDefault<Order>(sql);

                //if no order found with contact id, try user id
                if (contact.ContactId > 0 & order == null)
                {
                    sql = Sql.Builder
                             .Select("*")
                             .From("cpxOrder")
                             .Where("status='PROV' AND OrderIndex=@0 AND UserId=@1",orderIndex,contact.UserId);
                    order = _database.SingleOrDefault<Order>(sql);
                }

                if (order == null)
                {
                    order = CreateOrder(contact, orderIndex);
                }
                if (contact.ContactId != order.ContactId)
                {
                    order.ContactId = contact.ContactId;
                    _database.Update(order);
                }

                order.OrderLines = _database.Fetch<OrderLine>("SELECT * FROM cpxOrderLines WHERE OrderId=@0",order.OrderId);
                operationStatus.Order = order;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred retrieving the order", e);
            }
            return operationStatus;
        }

        public OrdersOperationStatus GetOrders(Contact contact)
        {
            var operationStatus = new OrdersOperationStatus();
            try
            {
                var sql = Sql.Builder
                    .Select("*")
                    .From("cpxOrder")
                    .Where("status='PROV'");

                if (contact.ContactId > 0)
                    sql.Append("AND ContactId=@0", contact.ContactId);
                else
                    sql.Append("AND UserId=@0", contact.UserId);

                var orders =
                    _database.Fetch<Order>(sql);
                
                for (int i=0;i<orders.Count;i++)
                {
                    if (contact.ContactId != orders[i].ContactId)
                    {
                        orders[i].ContactId = contact.ContactId;
                        _database.Update(orders[i]);
                    }
                    orders[i].OrderLines = _database.Fetch<OrderLine>("SELECT * FROM cpxOrderLines WHERE OrderId=@0", orders[i].OrderId);
                }
                operationStatus.Orders = orders;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus.LogFailedOperation(e,"An error has occurred updating the order");
            }
            return operationStatus;
        }

        private Order CreateOrder(Contact contact, int orderIndex)
        {
            var order = new Order {UserId = contact.UserId, UserName = contact.UserName, ContactId=contact.ContactId,OrderIndex = orderIndex,GiftAidAgreement = contact.GiftAidAgreement};
            order.OrderId = (int) _database.Insert("cpxOrder", "OrderId", true, order);
            return order;
        }

        public OrderOperationStatus SaveOrder(Order order)
        {
            return SaveOrder(order, true);
        }


        public OrderOperationStatus SaveOrder(Order order, bool updateOrderLines)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                if (!_database.IsNew(order))
                {
                    _database.Update(order);
                    if (updateOrderLines)
                    {
                        for (int i = 0; i < order.OrderLines.Count; i++)
                        {
                            if (_database.IsNew(order.OrderLines[i]))
                            {
                                order.OrderLines[i].OrderLineId =
                                    (int)_database.Insert(order.OrderLines[i]);
                                
                            }
                            else
                            {
                                _database.Update(order.OrderLines[i]);
                            }
                        }
                    }
                    orderOperationStatus.Status = true;
                    orderOperationStatus.Message = "The order has been saved successfully.";
                }
                else
                {
                    orderOperationStatus.Status = false;
                    orderOperationStatus.Message = "The order could not be found.";

                }
                orderOperationStatus.Order = order;

            }
            catch (Exception e)
            {
                orderOperationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred saving the order"+e.Message+e.StackTrace, e);

            }
            return orderOperationStatus;

        }

       
        public OrderOperationStatus GetOrder(int orderId)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                var order =
                    _database.SingleOrDefault<Order>(
                        "SELECT * FROM cpxOrder WHERE OrderId=@0",
                        orderId);
                order.OrderLines = _database.Fetch<OrderLine>("SELECT * FROM cpxOrderLines WHERE OrderId=@0",
                                                              orderId);
                operationStatus.Order = order;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred getting the order", e);
            }
            return operationStatus;


        }

        public OrderOperationStatus ProcessOrder(Order order, Contact contact)
        {
            throw new NotImplementedException();
        }



 

    }
}