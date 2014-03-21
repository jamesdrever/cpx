using System.Collections.Generic;
using System.Configuration;
using Autofac;
using CustomerPortalExtensions.Application.Contacts;
using CustomerPortalExtensions.Application.Ecommerce;
using CustomerPortalExtensions.Application.Ecommerce.Locations;
using CustomerPortalExtensions.Application.Ecommerce.OrderQueue;
using CustomerPortalExtensions.Application.Email;
using CustomerPortalExtensions.Config;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Infrastructure.Contacts;
using CustomerPortalExtensions.Infrastructure.ECommerce.Discounts;
using CustomerPortalExtensions.Infrastructure.ECommerce.Location;
using CustomerPortalExtensions.Infrastructure.ECommerce.Orders;
using CustomerPortalExtensions.Infrastructure.ECommerce.Products;
using CustomerPortalExtensions.Infrastructure.ECommerce.Shipping;
using CustomerPortalExtensions.Infrastructure.Email;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Infrastructure.Synchronisation;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using CustomerPortalExtensions.Interfaces.Email;
using CustomerPortalExtensions.Interfaces.Synchronisation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class EcommerceServiceIntegrationTests
    {


        private IEcommerceService _sut;

        [TestInitialize]
        public void Initialise()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ContactRepository>().As<IContactRepository>();
            builder.RegisterType<ContactSecondaryRepository>().As<IContactSecondaryRepository>();
            builder.RegisterType<DefaultContactSynchroniser>().As<IContactSynchroniser>();
            builder.RegisterType<ContactService>().As<IContactService>();
            builder.RegisterType<MailChimpEmailSubscriptionConnector>().As<IEmailSubscriptionConnector>();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>();
            builder.RegisterType<DiscountHandlerFactory>().As<IDiscountHandlerFactory>();
            builder.RegisterType<EcommerceService>().As<IEcommerceService>();
            builder.RegisterType<ShippingHandlerFactory>().As<IShippingHandlerFactory>();
            builder.RegisterType<LocationHandlerFactory>().As<ILocationHandlerFactory>();
            builder.RegisterType<LocationHandler>().As<ILocationHandler>();
            builder.RegisterType<OrderQueueService>().As<IOrderQueueService>();
            builder.RegisterType<EmailSubscriptionsService>().As<IEmailSubscriptionsService>();
            builder.RegisterType<BespokePricingHandlerFactory>().As<IBespokePricingHandlerFactory>();
            builder.RegisterType<OrderQueue>().As<IOrderQueue>();
            builder.RegisterType<OrderCoordinator>().As<IOrderCoordinator>();
            builder.RegisterType<Database>()
                .WithParameter("connectionString",
                    ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString)
                .WithParameter("providerName", "System.Data.SqlClient");

            //mocking config service
            var configService = new Mock<IConfigurationService>();
            configService.Setup(x => x.GetConfiguration()).Returns(new CustomerPortalSection());

            var product1 = new Product
            {
                Title = "Test Publication",
                Code = "OP1",
                ProductId = 100,
                OptionId = 0,
                Price = 5,
                ProductType = "P",
                OrderIndex = 2
            };
            var product1Status = new ProductOperationStatus {Status = true, Product = product1};

            var course1 = new Product
            {
                Title = "Test Course 1",
                Location = "Flatford Mill",
                LocationCode = "FM",
                DepositAmount = 50,
                ProductId = 1000,
                OptionId = 2000,
                Price = 300,
                ProductType = "C",
                OptionTitle = "Sole Residency",
                OrderIndex = 1,
                Category = "F/1",
                VoucherCategory = ""
            };
            var course1Status = new ProductOperationStatus {Status = true, Product = course1};

            var course1WithoutOptions = new Product
            {
                Title = "Test Course 1",
                Location = "Flatford Mill",
                LocationCode = "FM",
                DepositAmount = 50,
                ProductId = 1000,
                ProductType = "C",
                OrderIndex = 1,
                Category = "F/1",
                VoucherCategory = ""
            };
            var course1WithoutOptionsStatus = new ProductOperationStatus {Status = true, Product = course1WithoutOptions};

            var course1Options = new List<ProductOption>();
              
            course1Options.Add(new ProductOption {OptionId = 2000,Name="Sole Residency",OptionPrice=300});
            course1Options.Add(new ProductOption { OptionId = 2001, Name = "Shared Room", OptionPrice = 250 });
            course1Options.Add(new ProductOption { OptionId = 2002, Name = "Non Resident", OptionPrice = 200 });
            var course1OptionsStatus = new ProductOptionOperationStatus {Status = true, ProductOptions = course1Options};

            var course2 = new Product
                {
                    Title = "Test Course 2",
                    Location= "Juniper Hall",
                    LocationCode = "JH",
                    DepositAmount = 50,
                    ProductId = 1001,
                    OptionId = 2001,
                    Price = 250,
                    ProductType = "C",
                    OptionTitle = "Shared room",
                    OrderIndex = 1,
                    Category = "H/1",
                    VoucherCategory = "TESTFILT1"
                };
            var course2Status = new ProductOperationStatus {Status = true, Product = course2};


            var courseBespoke1 = new Product
            {
                Title = "Test Bespoke Course 1",
                Location = "Juniper Hall",
                LocationCode = "JH",
                DepositAmount = 50,
                ProductId = 2000,
                OptionId = 0,
                Price = (decimal)16.50,
                ProductType = "C",
                OptionTitle = "Sole occupancy",
                OrderIndex = 3,
                Category = "F/10",
                VoucherCategory = ""
            };
            var courseBespoke1Status = new ProductOperationStatus { Status = true, Product = courseBespoke1 };

            var courseBespoke2 = new Product
            {
                Title = "Test Bespoke Course 1",
                Location = "Juniper Hall",
                StartDateRange1 = new DateTime(2014,08,01),
                FinishDateRange1 = new DateTime(2014, 08, 10),
                LocationCode = "JH",
                DepositAmount = 50,
                ProductId = 2001,
                OptionId = 0,
                Price = (decimal)16.50,
                ProductType = "C",
                OptionTitle = "Sole occupancy",
                OrderIndex = 3,
                Category = "F/10",
                VoucherCategory = ""
            };
            var courseBespoke2Status = new ProductOperationStatus { Status = true, Product = courseBespoke2 };


            var Voucher20PercentOff = new Voucher
            {
                Title = "Ten per cent off",
                VoucherId = 1,
                Percentage = 10,
                OrderIndex = 1
            };
            var Voucher20PercentOffStatus = new VoucherOperationStatus
            {
                Status = true,
                Voucher = Voucher20PercentOff
            };
            
            var Voucher20PercentOffFamilyCourses = new Voucher
                {
                    Title = "Twenty per cent off family courses",                    
                    VoucherId = 2,
                    Percentage = 20,
                    OrderIndex = 1,
                    ProductCategoryFilter = "F",
                };
            var Voucher20PercentOffFamilyCoursesStatus = new VoucherOperationStatus
                {
                    Status = true,
                    Voucher = Voucher20PercentOffFamilyCourses
                };

            var Voucher20PoundPerItemWhenSpent100Pounds = new Voucher
                {
                    Title = "Twenty pounds per item off when hundred pounds spent",
                    VoucherId = 3,
                    PerItemAmount = 20,
                    OrderIndex = 1,
                    MinimumPayment = 100
                };

            var Voucher20PoundPerItemWhenSpent100PoundsStatus = new VoucherOperationStatus
                {
                    Status = true,
                    Voucher = Voucher20PoundPerItemWhenSpent100Pounds
                };


            var Voucher20PerCentWhenSpent100Pounds = new Voucher
                {
                    Title = "Twenty pounds per cent off when hundred pounds spent",
                    VoucherId = 4,
                    PerItemAmount = 10,
                    OrderIndex = 2,
                    MinimumPayment = 100
                };

            var Voucher20PerCentWhenSpent100PoundsStatus = new VoucherOperationStatus
                {
                    Status = true,
                    Voucher = Voucher20PerCentWhenSpent100Pounds
                };

            var Voucher10PerCentOnSelectedCourses = new Voucher
            {
                Title = "Ten per cent off selected courses",
                VoucherId = 5,
                Percentage = 10,
                VoucherCategoryFilter = "TESTFILT1",
                OrderIndex = 1,
            };

            var Voucher10PerCentOnSelectedCoursesStatus = new VoucherOperationStatus
            {
                Status = true,
                Voucher = Voucher10PerCentOnSelectedCourses
            };


            var VoucherNotFoundStatus = new VoucherOperationStatus
            {
                Status = false,
                Message="Voucher not found"
            };


            //need to mock productRepository as tied to Umbraco nodes
            var productRepository = new Mock<IProductRepository>();
            productRepository.Setup(x => x.GetProduct(100, 0)).Returns(product1Status);
            productRepository.Setup(x => x.GetProduct(1000,0)).Returns(course1WithoutOptionsStatus);
            productRepository.Setup(x => x.GetProduct(1000, 2000)).Returns(course1Status);
            productRepository.Setup(x => x.GetProductOptions(1000)).Returns(course1OptionsStatus);
            productRepository.Setup(x => x.GetProduct(1001, 2001)).Returns(course2Status);
            productRepository.Setup(x => x.GetProduct(2000, 0)).Returns(courseBespoke1Status);
            productRepository.Setup(x => x.GetProduct(2001, 0)).Returns(courseBespoke2Status);

            //need to mock VoucherRepository as tied to Umbraco nodes
            var VoucherRepository = new Mock<IVoucherRepository>();
            VoucherRepository.Setup(x => x.GetVoucher("TEST1")).Returns(Voucher20PercentOffStatus);
            VoucherRepository.Setup(x => x.GetVoucher("TEST2")).Returns(Voucher20PercentOffFamilyCoursesStatus);
            VoucherRepository.Setup(x => x.GetVoucher("TEST3")).Returns(Voucher20PoundPerItemWhenSpent100PoundsStatus);
            VoucherRepository.Setup(x => x.GetVoucher("TEST4")).Returns(Voucher20PerCentWhenSpent100PoundsStatus);
            VoucherRepository.Setup(x => x.GetVoucher("TEST5")).Returns(Voucher10PerCentOnSelectedCoursesStatus);

            VoucherRepository.Setup(x => x.GetVoucher("TESTNOTFOUND")).Returns(VoucherNotFoundStatus);

            //need to mock IContactAuthenticationHandler as tied to HttpContext
            var contactAuthenticationHandler = new Mock<IContactAuthenticationHandler>();
            contactAuthenticationHandler.Setup(x => x.IsCurrentUserLoggedIn()).Returns(false);
            contactAuthenticationHandler.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());

            builder.RegisterInstance(configService.Object).As<IConfigurationService>();
            builder.RegisterInstance(contactAuthenticationHandler.Object).As<IContactAuthenticationHandler>();
            builder.RegisterInstance(productRepository.Object).As<IProductRepository>();
            builder.RegisterInstance(VoucherRepository.Object).As<IVoucherRepository>();

            var container = builder.Build();

            _sut = container.Resolve<IEcommerceService>();
        }

        [TestMethod]
        public void ShouldCreateNewOrderIfNoneExists()
        {
            var operationStatus = _sut.GetOrder();
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(0,order.NumberOfItems);
            Assert.AreEqual(0,order.ProductSubTotal);
            Assert.AreEqual(0,order.PaymentTotal);
            Assert.IsFalse(order.ContainsProductType("P"));
            Assert.IsFalse(order.ContainsProducts());

        }




        [TestMethod]
        public void ShouldAddProductToBasket()
        {
            var operationStatus = _sut.AddProductToOrder(100, 0, 1,"F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(5,order.ProductSubTotal);
            Assert.AreEqual(5,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("P"));
            Assert.IsTrue(order.ContainsProducts());

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(2);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(5,order.ProductSubTotal);
            Assert.AreEqual(5,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("P"));
            Assert.IsTrue(order.ContainsProducts());

        }
        [TestMethod]
        public void ShouldReturnCorrectOrderSummary()
        {
            var operationStatus = _sut.AddProductToOrder(100, 0, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1, order.NumberOfItems);
            Assert.AreEqual(5, order.ProductSubTotal);
            Assert.AreEqual(5, order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("P"));
            Assert.IsTrue(order.ContainsProducts());

            int orderId = order.OrderId;

            //check that when go back and get the order summary
            //we get the same results
            var operationSummaryStatus = _sut.GetOrderSummaryById(orderId);
            Assert.IsTrue(operationSummaryStatus.Status);
            var orderSummary = operationSummaryStatus.OrderSummary;

            Assert.AreEqual(1, orderSummary.NumberOfItems);
            Assert.AreEqual("£5.00", orderSummary.ProductSubTotal);
            Assert.AreEqual("£5.00", orderSummary.PaymentTotal);

        }

        [TestMethod]
        public void ShouldAddCourseAndFullPaymentToBasket()
        {
            var operationStatus = _sut.AddProductToOrder(1000, 2000, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(300,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(300,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());


        }

        [TestMethod]
        public void ShouldAddCourseAndDepositPaymentToBasket()
        {
            var operationStatus = _sut.AddProductToOrder(1000, 2000, 1, "D");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(50,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(50,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());


        }

        [TestMethod]
        public void ShouldReturnCorrectBespokePricing()
        {
            DateTime startDate = new DateTime(2013, 08, 05);
            DateTime finishDate = new DateTime(2013, 08, 08);
            var operationStatus = _sut.GetProduct(2000, 0,0, startDate, finishDate);
            Assert.IsTrue(operationStatus.Status);
            Assert.AreEqual((decimal)16.50,operationStatus.Product.ChargedPrice);
        }


        [TestMethod]
        public void ShouldAddCourseWithBespokePricingToOrder()
        {
            DateTime startDate=new DateTime(2013,08,05);
            DateTime finishDate=new DateTime(2013,08,08);
            var operationStatus = _sut.AddCourseWithFlexibleDatesToOrder(2000,0, startDate, finishDate, 4,"F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(4, order.NumberOfItems);
            Assert.AreEqual((decimal)(16.50*4), order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(3);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(4, order.NumberOfItems);
            Assert.AreEqual((decimal)(16.50 * 4), order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

        }

        [TestMethod]
        public void ShouldNotAddCourseWithProductDatesOutOfRange()
        {
            DateTime startDate = new DateTime(2013, 07, 05);
            DateTime finishDate = new DateTime(2013, 07, 08);
            var operationStatus = _sut.AddCourseWithFlexibleDatesToOrder(2001,0,startDate, finishDate, 4,"F");
            Assert.IsFalse(operationStatus.Status);
            Assert.AreEqual("CPX.DatesOutOfRange",operationStatus.MessageCode);
        }


        [TestMethod]
        public void ShouldAddProductsToCorrectOrderIndex()
        {
            //first publication into order index 2
            var operationStatus = _sut.AddProductToOrder(100, 0, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(5,order.ProductSubTotal);
            Assert.AreEqual(5,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("P"));
            Assert.IsTrue(order.ContainsProducts());

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(2);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(5,order.ProductSubTotal);
            Assert.AreEqual(5,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("P"));
            Assert.IsTrue(order.ContainsProducts());

            operationStatus = _sut.AddProductToOrder(1000, 2000, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(300,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(300,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

        }

        [TestMethod]
        public void ShouldReturnCorrectProductInfoWithOption()
        {
            var operationStatus = _sut.GetProduct(100,0);
            Assert.IsTrue(operationStatus.Status);
            var product = operationStatus.Product;
            Assert.AreEqual("Test Publication",product.Title);
            Assert.AreEqual("OP1",product.Code);
            Assert.AreEqual(5,product.Price);
            Assert.AreEqual("P",product.ProductType);
            Assert.AreEqual(2, product.OrderIndex);
        }


        [TestMethod]
        public void ShouldReturnCorrectProductInfoWithoutOption()
        {
            var operationStatus = _sut.GetProduct(1000);
            Assert.IsTrue(operationStatus.Status);
            var product = operationStatus.Product;
            Assert.AreEqual("Test Course 1", product.Title);
            Assert.AreEqual(null, product.Price);
            Assert.AreEqual("C", product.ProductType);
            Assert.AreEqual(1, product.OrderIndex);
        }

        [TestMethod]
        public void ShouldReturnCorrectProductOptions()
        {
            var operationStatus = _sut.GetProductOptions(1000);
            Assert.IsTrue(operationStatus.Status);
            Assert.AreEqual(3, operationStatus.ProductOptions.Count);
        }

        [TestMethod]
        public void ShouldAddVoucherToOrder()
        {
            var operationStatus = _sut.AddVoucherToOrder("TEST1");

            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1,order.VoucherId);
            Assert.AreEqual(10,order.VoucherPercentage);

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1, order.VoucherId);
            Assert.AreEqual(10, order.VoucherPercentage);

        }

        [TestMethod]
        public void ShouldHandleVoucherNotFound()
        {
            var operationStatus = _sut.AddVoucherToOrder("TESTNOTFOUND");

            Assert.IsFalse(operationStatus.Status);
            var order = operationStatus.Order;

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(0, order.VoucherId);
            Assert.AreEqual(0, order.VoucherPercentage);

        }

        [TestMethod]
        public void ShouldApplyVoucher()
        {

            var operationStatus = _sut.AddProductToOrder(1000, 2000, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.AreEqual(300,order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            operationStatus = _sut.AddVoucherToOrder("TEST1");

            Assert.IsTrue(operationStatus.Status);
            
            order = operationStatus.Order;

            Assert.AreEqual(1, order.VoucherId);
            Assert.AreEqual(10, order.VoucherPercentage);

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(1, order.VoucherId);
            Assert.AreEqual(10, order.VoucherPercentage);
            Assert.AreEqual(1,order.NumberOfItems);
            Assert.AreEqual(300,order.ProductSubTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());
            Assert.AreEqual(30,order.GetVoucherTotal());
            Assert.AreEqual(270, order.PaymentTotal);

        }


        [TestMethod]
        public void ShouldApplyVoucherWithProductFilter()
        {

            var operationStatus = _sut.AddProductToOrder(1000, 2000, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1, order.NumberOfItems);
            Assert.AreEqual(300, order.ProductSubTotal);
            Assert.AreEqual(300, order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            operationStatus = _sut.AddProductToOrder(1001, 2001, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(2, order.NumberOfItems);
            Assert.AreEqual(550, order.ProductSubTotal);
            Assert.AreEqual(550, order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());


            operationStatus = _sut.AddVoucherToOrder("TEST2");

            Assert.IsTrue(operationStatus.Status);

            order = operationStatus.Order;

            Assert.AreEqual(2, order.VoucherId);
            Assert.AreEqual(20, order.VoucherPercentage);

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(2, order.VoucherId);
            Assert.AreEqual(20, order.VoucherPercentage);
            Assert.AreEqual(2, order.NumberOfItems);
            Assert.AreEqual(550, order.ProductSubTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());
            Assert.AreEqual(60, order.GetVoucherTotal());
            Assert.AreEqual(490, order.PaymentTotal);

        }

        [TestMethod]
        public void ShouldApplyVoucherWithCategoryFilter()
        {

            var operationStatus = _sut.AddProductToOrder(1000, 2000, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            var order = operationStatus.Order;

            Assert.AreEqual(1, order.NumberOfItems);
            Assert.AreEqual(300, order.ProductSubTotal);
            Assert.AreEqual(300, order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());

            operationStatus = _sut.AddProductToOrder(1001, 2001, 1, "F");
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(2, order.NumberOfItems);
            Assert.AreEqual(550, order.ProductSubTotal);
            Assert.AreEqual(550, order.PaymentTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());


            operationStatus = _sut.AddVoucherToOrder("TEST5");

            Assert.IsTrue(operationStatus.Status);

            order = operationStatus.Order;

            Assert.AreEqual(5, order.VoucherId);
            Assert.AreEqual(10, order.VoucherPercentage);

            //check that when go back and get the order
            //we get the same results
            operationStatus = _sut.GetOrder(1);
            Assert.IsTrue(operationStatus.Status);
            order = operationStatus.Order;

            Assert.AreEqual(5, order.VoucherId);
            Assert.AreEqual(10, order.VoucherPercentage);
            Assert.AreEqual(2, order.NumberOfItems);
            Assert.AreEqual(550, order.ProductSubTotal);
            Assert.IsTrue(order.ContainsProductType("C"));
            Assert.IsTrue(order.ContainsCourses());
            Assert.AreEqual(25, order.GetVoucherTotal());
            Assert.AreEqual(525, order.PaymentTotal);

        }



    }
}
