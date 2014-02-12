using System;
using System.Collections.Generic;
using CustomerPortalExtensions.Domain.ECommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class OrderUnitTests
    {

        [TestMethod]
        public void AddProductToOrder()
        {
            Order order = new Order();
            Product product = new Product {Title = "Test Product",ProductId=1,Price=50};
            var orderOperationStatus=order.Add(product, 2, "F", 50);
            Assert.IsTrue(orderOperationStatus.Status);
            Assert.IsTrue(order.NumberOfItems == 2);
            Assert.IsTrue(order.ContainsProduct(product));
            Assert.IsTrue(order.PaymentTotal==100);

        }


        [TestMethod]
        public void UpdateProductInOrder()
        {
            Order order = new Order();
            Product product = new Product { Title = "Test Product", ProductId = 1, Price = 50 };
            var orderOperationStatus = order.Add(product, 2, "F", 50);
            Assert.IsTrue(orderOperationStatus.Status);
            Assert.IsTrue(order.NumberOfItems == 2);
            Assert.IsTrue(order.ContainsProduct(product));
            Assert.IsTrue(order.PaymentTotal == 100);
            orderOperationStatus = order.Update(product, product, 6, "F", 50);
            Assert.IsTrue(orderOperationStatus.Status);
            Assert.IsTrue(order.NumberOfItems == 6);
            Assert.IsTrue(order.ContainsProduct(product));
            Assert.IsTrue(order.PaymentTotal == 300);

        }


        [TestMethod]
        public void UpdateProductInOrderToDifferentOption()
        {
            Order order = new Order();
            Product product = new Product { Title = "Test Product", ProductId = 1,OptionId = 1,OptionTitle = "Option1", Price=50};
            order.Add(product, 2, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 2);
            Assert.IsTrue(order.ContainsProduct(product));
            Assert.IsTrue(order.PaymentTotal == 100);
            Product updateProduct = new Product { Title = "Test Product", ProductId = 1, OptionId = 2, OptionTitle = "Option2",Price=50 };
            order.Update(product, updateProduct, 6, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 6);
            Assert.IsTrue(order.ContainsProduct(updateProduct));
            Assert.IsFalse(order.ContainsProduct(product));
            Assert.IsTrue(order.PaymentTotal == 300);

        }

        [TestMethod]
        public void ShouldNotUpdateWhereNotExistingProductInOrder()
        {
            Order order = new Order();
            Product product = new Product { Title = "Test Product", ProductId = 1, OptionId = 1, OptionTitle = "Option1",Price=50 };
            var operationStatus = order.Update(product, product, 6, "F", 50);
            Assert.IsTrue(operationStatus.Status == false);
            Assert.IsTrue(operationStatus.MessageCode == "CPX.ItemNotInOrder");
        }

        [TestMethod]
        public void ShouldNotUpdateItemToExistingProduct()
        {
            Order order = new Order();
            Product product = new Product {Title = "Test Product", ProductId = 1, OptionId = 1, OptionTitle = "Option1",Price=50};
            order.Add(product, 2, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 2);
            Assert.IsTrue(order.ContainsProduct(product));
            Assert.IsTrue(order.PaymentTotal == 100);
            Product product2 = new Product
                {
                    Title = "Test Product",
                    ProductId = 1,
                    OptionId = 2,
                    OptionTitle = "Option2",
                    Price=50
                };
            order.Add(product2, 2, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 4);
            Assert.IsTrue(order.ContainsProduct(product2));
            Assert.IsTrue(order.PaymentTotal == 200);
            Product updateProduct = new Product
                {
                    Title = "Test Product",
                    ProductId = 1,
                    OptionId = 2,
                    OptionTitle = "Option2",
                    Price=50
                };
            var operationStatus = order.Update(product, updateProduct, 6, "F", 50);
            Assert.IsTrue(operationStatus.Status == false);
            Assert.IsTrue(operationStatus.MessageCode == "CPX.ItemAlreadyInOrder");

        }



        [TestMethod]
        public void AddPercentageVoucher()
        {
            Order order=new Order{ VoucherPercentage = 10, VoucherId=1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine{ Quantity=1, PaymentAmount=10 });
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal()==1);

        }

        [TestMethod]
        public void AddPerItemVoucher()
        {
            Order order = new Order { VoucherPerItemAmount = 1, VoucherId = 1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine { Quantity = 1, PaymentAmount = 10 });
            orderLines.Add(new OrderLine { Quantity = 1, PaymentAmount = 10 });
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal() == 2);

        }

        [TestMethod]
        public void AddPerItemVoucherWithCategoryFilter()
        {
            Order order = new Order { VoucherPerItemAmount = 1, VoucherCategoryFilter = "TEST1CATEGORY", VoucherId = 1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine
                {
                    ProductVoucherCategory = "TEST1CATEGORY",
                    Quantity = 1,
                    PaymentAmount = 10
                });
            orderLines.Add(new OrderLine {ProductVoucherCategory = "TEST2CATEGORY", Quantity = 1, PaymentAmount = 10});
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal() == 1);

        }

        [TestMethod]
        public void AddPerItemVoucherWithProductCategoryFilter()
        {
            Order order = new Order { VoucherPerItemAmount = 1, VoucherProductCategoryFilter = "TEST1", VoucherId = 1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine { ProductCategory = "TEST1CATEGORY", Quantity = 1, PaymentAmount = 10 });
            orderLines.Add(new OrderLine { ProductCategory = "TEST2CATEGORY", Quantity = 1, PaymentAmount = 10 });
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal() == 1);

        }



        [TestMethod]
        public void AddPerItemVoucherWithMiniumPayment()
        {
            Order order = new Order { VoucherPerItemAmount = 1, VoucherMinimumPayment=25, VoucherId = 1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine {  Quantity = 1, PaymentAmount = 10 });
            orderLines.Add(new OrderLine {  Quantity = 1, PaymentAmount = 10 });
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal() == 0);

        }

        [TestMethod]
        public void ShouldHaveZeroVoucherTotalWithLessThanMiniumItems()
        {
            Order order = new Order { VoucherPerItemAmount = 1, VoucherMinimumItems = 5, VoucherId = 1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine { Quantity = 2, PaymentAmount = 10 });
            orderLines.Add(new OrderLine { Quantity = 2, PaymentAmount = 10 });
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal() == 0);

        }
        [TestMethod]
        public void ShouldHavePositiveVoucherTotalWithMoreThanMiniumItems()
        {
            Order order = new Order { VoucherPerItemAmount = 1, VoucherMinimumItems = 1, VoucherId = 1 };
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(new OrderLine { Quantity = 2, PaymentAmount = 10 });
            orderLines.Add(new OrderLine { Quantity = 2, PaymentAmount = 10 });
            order.OrderLines = orderLines;
            Assert.IsTrue(order.GetVoucherTotal() == 4);

        }

        [TestMethod]
        public void ShoulReturnLocationEmailsCorrectly()
        {
            Order order = new Order();
            Product product = new Product
                {
                    Title = "Test Product",
                    ProductId = 1,
                    OptionId = 1,
                    OptionTitle = "Option1",
                    LocationEmail = "test@test1.com",
                    Price=50
                };
            order.Add(product, 2, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 2);
            Assert.IsTrue(order.ContainsProduct(product));
            var expectedEmailList = new List<string> {"test@test1.com"};
            var actualEmailList = order.GetLocationEmails();

            Assert.AreEqual(expectedEmailList.Count,actualEmailList.Count);
            Assert.AreEqual(expectedEmailList[0],actualEmailList[0]);
            Product product2 = new Product
                {
                    Title = "Test Product 2",
                    ProductId = 2,
                    LocationEmail = "test@test2.com",
                    Price=50
                };
            order.Add(product2, 2, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 4);
            Assert.IsTrue(order.ContainsProduct(product2));

            expectedEmailList = new List<string> {"test@test1.com", "test@test2.com"};
            actualEmailList = order.GetLocationEmails();
            
            Assert.AreEqual(expectedEmailList.Count,actualEmailList.Count);
            Assert.AreEqual(expectedEmailList[0],actualEmailList[0]);
            Assert.AreEqual(expectedEmailList[1],actualEmailList[1]);
           
            //check that it only returns distinct email addresses
            Product product3 = new Product
            {
                Title = "Test Product 3",
                ProductId = 3,
                LocationEmail = "test@test1.com",
                Price=50
            };
            order.Add(product3,2, "F", 50);
            Assert.IsTrue(order.NumberOfItems == 6);
            Assert.IsTrue(order.ContainsProduct(product3));
               
            actualEmailList = order.GetLocationEmails();            
            Assert.AreEqual(expectedEmailList.Count,actualEmailList.Count);
            Assert.AreEqual(expectedEmailList[0],actualEmailList[0]);
            Assert.AreEqual(expectedEmailList[1],actualEmailList[1]);

        }
        
    }
}
