using System;
using System.Configuration;
using CustomerPortalExtensions.Application.Ecommerce;
using CustomerPortalExtensions.Config;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class EcommerceServiceUnitTests
    {
        private IEcommerceService _ecommerceService;
        //TODO: not sure how meaningful these unit tests are, beyond
        //establishing the functionality is in place.  Too much
        //dependence on mocking..

        [TestInitialize]
        public void Initialise()
        {
            var product = new Product
                {
                    ProductId = 29397,
                    ProductType = "P",
                    OrderIndex = 1,
                    Price = 3,
                    Title = "Test Product"
                };
            var product2 = new Product
                {
                    ProductId = 29398,
                    ProductType = "P",
                    OrderIndex = 1,
                    OptionId = 30001,
                    Price = 3,
                    Title = "Test Product with Option"
                };

            var productRepository = new Mock<IProductRepository>();
            productRepository.Setup(x => x.GetProduct(It.Is<int>(i => i == 29397),It.Is<int>(i => i==0)))
                             .Returns(new ProductOperationStatus {Product = product, Status = true});
            productRepository.Setup(x => x.GetProduct(It.Is<int>(i => i == 29398),It.Is<int>(i=>i==30001)))
                             .Returns(new ProductOperationStatus {Product = product2, Status = true});

            var voucherRepository = new Mock<IVoucherRepository>();

            var contactRepository = new Mock<IContactRepository>();
            Contact contact = new Contact {UserId = "1234", UserName = "james@jamesdrever.co.uk"};
            contactRepository.Setup((x => x.GetContact()))
                             .Returns(new ContactOperationStatus { Status = true, Contact = contact} ); //need to create UpdatedContact details new UpdatedContact 
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<string>(),It.IsAny<decimal>(),It.IsAny<Contact>()))
                           .Returns(new OrderOperationStatus {Status = true});
            orderRepository.Setup(x => x.Update(It.IsAny<Product>(),It.IsAny<Product>(), It.IsAny<int>(),It.IsAny<string>(), It.IsAny<decimal>(),It.IsAny<Contact>()))
                           .Returns(new OrderOperationStatus {Status = true});
            orderRepository.Setup(x => x.Remove(It.IsAny<Product>(), It.IsAny<Contact>()))
                           .Returns(new OrderOperationStatus {Status = true});
            orderRepository.Setup(x => x.UpdateSpecialRequirements(It.IsAny<string>(),It.IsAny<int>(),It.IsAny<Contact>()))
                           .Returns(new OrderOperationStatus { Status = true });
            var configService = new Mock<IConfigurationService>();
            configService.Setup(x => x.GetConfiguration()).Returns(new CustomerPortalSection());
            _ecommerceService = new EcommerceService(orderRepository.Object, productRepository.Object, voucherRepository.Object,
                                                     contactRepository.Object, configService.Object);
        }

        [TestMethod]
        public void AddProductToOrder()
        {
            var operationStatus = _ecommerceService.AddProductToOrder(29397, 2);
            Assert.IsTrue(operationStatus.Status);
        }

        [TestMethod]
        public void AddOptionToOrder()
        {
            var operationStatus = _ecommerceService.AddProductToOrder(29397, 2);
            Assert.IsTrue(operationStatus.Status);
        }


        [TestMethod]
        public void RemoveFromOrder()
        {
            var operationStatus = _ecommerceService.Remove(1,2);
            Assert.IsTrue(operationStatus.Status);
        }

        [TestMethod]
        public void UpdateOrder()
        {
            var operationStatus = _ecommerceService.Update(1,1,2);
            Assert.IsTrue(operationStatus.Status);
        }

        [TestMethod]
        public void UpdateSpecialRequirements()
        {
            var operationStatus = _ecommerceService.UpdateSpecialRequirements("This is a test");
            Assert.IsTrue(operationStatus.Status);
        }
    }
}
