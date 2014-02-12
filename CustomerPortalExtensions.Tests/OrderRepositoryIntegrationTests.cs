using System;
using System.Configuration;
using CustomerPortalExtensions.Application.Ecommerce.Discounts;
using CustomerPortalExtensions.Application.Ecommerce.Shipping;
using CustomerPortalExtensions.Config;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Infrastructure.ECommerce;
using CustomerPortalExtensions.Infrastructure.ECommerce.Orders;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class OrderRepositoryIntegrationTests
    {
        private static IOrderRepository _orderRepository;
        private static Contact _existingUserCredentials;
        private static Contact _newUserCredentials;

        [ClassInitialize]
        public static void Initialise(TestContext context)
        {

            _existingUserCredentials = new Contact {UserId = "1234", UserName = "james@jamesdrever.co.uk"};
            _newUserCredentials = new Contact
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString()
                };

            var database = new Database(ConfigurationManager.AppSettings["umbracoDbDSN"],
                                                 "System.Data.SqlClient");
            _orderRepository = new OrderRepository(database);
        }

        [TestInitialize]
        public void TestInitialise()
        {
            _orderRepository.RemoveAll(_existingUserCredentials);
            _orderRepository.RemoveAll(_newUserCredentials);
        }

        [TestMethod]
        public void UpdateOrder()
        {
            OrderOperationStatus operationStatus = _orderRepository.GetOrder(_existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;
            order.VoucherInfo = "test addition";
            operationStatus = _orderRepository.SaveOrder(order);
            Assert.IsTrue(operationStatus.Status);
            order=operationStatus.Order;
            Assert.IsTrue(order.VoucherInfo=="test addition");
        }


        [TestMethod]
        public void AddToOrderRepository()
        {
            var product = new Product
                {
                    ProductId = 29397,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Product"
                };

            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            //TODO: check that the product has been added sucessfully
        }

        [TestMethod]
        public void CreateAndAddToOrderRepository()
        {
            var product = new Product
                {
                    ProductId = 29397,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Product"
                };

            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _newUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            operationStatus = _orderRepository.GetOrder(_newUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            var updatedOrder = operationStatus.Order;
            Assert.IsTrue(updatedOrder.ContainsProduct(product));
        }

        [TestMethod]
        public void AddThenRemoveFromOrderRepository()
        {
            var product = new Product
                {
                    ProductId = 24249,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Remove Product"
                };
            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            operationStatus = _orderRepository.Remove(product, _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsFalse(operationStatus.Order.ContainsProduct(product));
        }

        [TestMethod]
        public void AddThenUpdateOrderRepository()
        {
            var product = new Product
                {
                    ProductId = 24249,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Update Product"
                };
            var newProduct = new Product
                {
                    ProductId = 24249,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Update Product"
                };
            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            operationStatus = _orderRepository.Update(product, newProduct, 2, "F", product.Price,
                                                      _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).Quantity == 2);


        }


        [TestMethod]
        public void CheckAddingSameProduct()
        {
            var product = new Product
                {
                    ProductId = 24126,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Checking Same Twice Product"
                };
            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            operationStatus = _orderRepository.Add(product, 2, "F", product.Price, _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).Quantity == 3);


        }

        [TestMethod]
        public void CheckAddingSameProductWithDifferentOptions()
        {
            var product = new Product
                {
                    ProductId = 24359,
                    OptionId = 24904,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Checking Same Twice Product"
                };
            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            var secondProduct = new Product()
                {
                    ProductId = 24359,
                    OptionId = 25308,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Checking Same Twice Product 2"
                };

            operationStatus = _orderRepository.Add(secondProduct, 1, "F", product.Price, _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            Assert.IsTrue(operationStatus.Order.ContainsProduct(secondProduct));
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).Quantity == 1);
            Assert.IsTrue(operationStatus.Order.GetOrderLine(secondProduct).Quantity == 1);


        }

        [TestMethod]
        public void AddProductWithPartialPayment()
        {
            var product = new Product
                {
                    ProductId = 24314,
                    OptionId = 24904,
                    DepositAmount = 50,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 300,
                    Title = "Test Partial Payment Product"
                };
            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "D", 1, _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).Quantity == 1);
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).PaymentAmount.Equals(50));
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).ProductPrice.Equals(3));

        }

        [TestMethod]
        public void UpdateSpecialRequirements()
        {

            OrderOperationStatus operationStatus = _orderRepository.UpdateSpecialRequirements("This is a test", 1,
                                                                                              _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.SpecialRequirements == "This is a test");

            operationStatus = _orderRepository.GetOrder(_existingUserCredentials, 1);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.SpecialRequirements == "This is a test");
        }

        [TestMethod]
        public void DeletingProductWithDifferentOptions()
        {
            var product = new Product
                {
                    ProductId = 24359,
                    OptionId = 24904,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Removing Same Poduct with Different Options 1"
                };
            OrderOperationStatus operationStatus = _orderRepository.Add(product, 1, "F", product.Price,
                                                                        _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            var secondProduct = new Product()
            {
                ProductId = 24359,
                OptionId = 25308,
                ProductType = "P",
                OrderIndex = 1,
                Price = 3,
                Title = "Test Removing Same Poduct with Different Options 2"
            };

            operationStatus = _orderRepository.Add(secondProduct, 1, "F", product.Price, _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            Assert.IsTrue(operationStatus.Order.ContainsProduct(secondProduct));
            Assert.IsTrue(operationStatus.Order.GetOrderLine(product).Quantity == 1);
            Assert.IsTrue(operationStatus.Order.GetOrderLine(secondProduct).Quantity == 1);

            operationStatus = _orderRepository.Remove(secondProduct, _existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Order.ContainsProduct(product));
            Assert.IsFalse(operationStatus.Order.ContainsProduct(secondProduct));
        }

        [TestMethod]
        public void ApplyVoucher()
        {
            Voucher voucher = new Voucher() {Amount = 20, Title="£20 off"};
            OrderOperationStatus operationStatus = _orderRepository.AddVoucher(voucher,_existingUserCredentials);
            Assert.IsTrue(operationStatus.Status);
        }
    }

}
