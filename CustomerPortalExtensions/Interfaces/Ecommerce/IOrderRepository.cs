using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

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