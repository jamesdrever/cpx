using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Orders
{
    public class OrderCoordinator : IOrderCoordinator
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IConfigurationService _config;
        private readonly IDiscountHandlerFactory _discountHandlerFactory;
        private readonly IShippingHandlerFactory _shippingHandlerFactory;

        public OrderCoordinator(IOrderRepository orderRepository, IConfigurationService config,IDiscountHandlerFactory discountHandlerFactory,
                                IShippingHandlerFactory shippingHandlerFactory)
        {
            if (orderRepository == null)
            {
                throw new ArgumentNullException("orderRepository");
            }
            _orderRepository = orderRepository;
             if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            _config = config;
            if (discountHandlerFactory == null)
            {
                throw new ArgumentNullException("discountHandlerFactory");
            }
            _discountHandlerFactory = discountHandlerFactory;
            if (shippingHandlerFactory == null)
            {
                throw new ArgumentNullException("shippingHandlerFactory");
            }
            _shippingHandlerFactory = shippingHandlerFactory;



        }

        public OrderOperationStatus GetOrder(Contact contact, int orderIndex)
        {
            var orderOperationStatus = _orderRepository.GetOrder(contact, orderIndex);
            if (orderOperationStatus.Status)
            {
                orderOperationStatus.Order = GetOrderConfiguration(orderOperationStatus.Order);
            }
            return orderOperationStatus;

        }


        public OrderOperationStatus GetOrder(int orderId)
        {
            var orderOperationStatus = _orderRepository.GetOrder(orderId);
            if (orderOperationStatus.Status)
            {
                orderOperationStatus.Order = GetOrderConfiguration(orderOperationStatus.Order);
            }
            return orderOperationStatus;
        }

        public OrderSummaryOperationStatus GetOrderSummary(int orderId)
        {
            var orderSummaryOperationStatus = new OrderSummaryOperationStatus();
            var orderOperationStatus = _orderRepository.GetOrder(orderId);

            if (orderOperationStatus.Status)
            {
                orderOperationStatus.Order = GetOrderConfiguration(orderOperationStatus.Order);
                var orderSummary = new OrderSummary();
                orderSummary.NumberOfItems = orderOperationStatus.Order.NumberOfItems;
                orderSummary.OrderId = orderOperationStatus.Order.OrderId;
                orderSummary.PaymentTotal = string.Format("{0:C}",orderOperationStatus.Order.PaymentTotal);
                orderSummary.ProductSubTotal = string.Format("{0:C}",orderOperationStatus.Order.ProductSubTotal);
                orderSummary.Status = orderOperationStatus.Order.Status;
                orderSummaryOperationStatus.OrderSummary = orderSummary;
                orderSummaryOperationStatus.Status = true;
            }
            else
            {
                orderSummaryOperationStatus.Message = orderOperationStatus.Message;
                orderSummaryOperationStatus.Status = false;
            }
            return orderSummaryOperationStatus;
        }

        public Order UpdateOrderWithShippingAndDiscounts(Order order, Contact contact)
        {
            IOrderTypeSetting orderTypeSetting =
    _config.GetConfiguration().OrderTypeSettings[order.OrderIndex.ToString()];

            string shippingConfig = (orderTypeSetting != null)
                                        ? orderTypeSetting.ShippingHandler
                                        : _config.GetConfiguration().ShippingHandler;
            order = _shippingHandlerFactory.getShippingHandler(shippingConfig).UpdateShipping(order, contact);

            string discountConfig = (orderTypeSetting != null)
                                        ? orderTypeSetting.DiscountHandler
                                        : _config.GetConfiguration().DiscountHandler;
            order = _discountHandlerFactory.getDiscountHandler(discountConfig).UpdateDiscount(order, contact);
            return order;
        }

        private Order GetOrderConfiguration(Order order)
        {
            IOrderTypeSetting orderTypeSetting =
                _config.GetConfiguration().OrderTypeSettings[order.OrderIndex.ToString()];

            order.Description = (orderTypeSetting != null)
                                    ? orderTypeSetting.Description
                                    : "";

            order.CheckoutPage = (orderTypeSetting != null)
                                        ? orderTypeSetting.CheckoutPage
                                        : _config.GetConfiguration().CheckoutPage;

            order.OrderPage = (orderTypeSetting != null)
                                        ? orderTypeSetting.OrderPage
                                        : _config.GetConfiguration().OrderPage;

            order.ContactDetailsPage = (orderTypeSetting != null)
                                        ? orderTypeSetting.ContactDetailsPage
                                        : _config.GetConfiguration().ContactDetailsPage;

            order.SpecialRequirementsText = (orderTypeSetting != null)
                            ? orderTypeSetting.SpecialRequirementsText
                            : _config.GetConfiguration().SpecialRequirementsText;

            order.PaymentGatewayForm = (orderTypeSetting != null)
                                        ? orderTypeSetting.PaymentGatewayForm
                                        : _config.GetConfiguration().PaymentGatewayForm;

            order.PaymentGatewayAccount = (orderTypeSetting != null)
                                        ? orderTypeSetting.PaymentGatewayAccount
                                        : _config.GetConfiguration().PaymentGatewayAccount;

            order.PaymentGatewayCallbackUrl = (orderTypeSetting != null)
                                                  ? orderTypeSetting.PaymentGatewayCallbackUrl
                                                  : "";

            order.PaymentGatewayCompletionPage = (orderTypeSetting != null)
                                      ? orderTypeSetting.PaymentGatewayCompletionPage
                                      : "";


            order.PaymentGatewayCheckCode = (orderTypeSetting != null)
                                                  ? orderTypeSetting.PaymentGatewayCheckCode
                                                  : "";

            order.AdditionalQueueProcessingHandler = (orderTypeSetting != null)
                                      ? orderTypeSetting.AdditionalQueueProcessingHandler
                                      : "";

            return order;
        }


        public string GetOrderPage(int orderIndex)
        {
            IOrderTypeSetting orderTypeSetting =
                        _config.GetConfiguration().OrderTypeSettings[orderIndex.ToString(CultureInfo.InvariantCulture)];
            return orderTypeSetting.OrderPage;
        }

        public OrderOperationStatus SaveOrder(Order order,bool updateOrderLines)
        {
            return _orderRepository.SaveOrder(order,updateOrderLines);
        }

    }
}