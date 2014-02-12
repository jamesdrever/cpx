using System;
using CustomerPortalExtensions.Application.Ecommerce.Pricing;
using CustomerPortalExtensions.Domain.ECommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class FamilyBespokePricingHandlerUnitTests
    {
        private FamilyBespokePricingHandler _sut = new FamilyBespokePricingHandler();
        private Product _courseBespoke3Night;
        private Product _courseBespoke4Night;
        private Product _courseBespoke7Night;
        private Product _courseBespoke8Night;
        [TestInitialize]
        public void Initialise()
        {
            _courseBespoke3Night = new Product
            {
                Title = "Test Bespoke Course 3 Day",
                Location = "Juniper Hall",
                StartDate = new DateTime(2013, 08, 05),
                FinishDate = new DateTime(2013, 08, 08),
                LocationCode = "JH",
                DepositAmount = 50,
                ProductId = 2000,
                OptionId = 3001,
                Price = (decimal)16.50,
                ProductType = "C",
                OptionTitle = "Sole occupancy",
                OrderIndex = 3,
                Category = "F/10",
                VoucherCategory = ""
            };

            _courseBespoke4Night = new Product
            {
                Title = "Test Bespoke Course 4 Day",
                Location = "Juniper Hall",
                StartDate = new DateTime(2013, 08, 05),
                FinishDate = new DateTime(2013, 08, 09),
                LocationCode = "JH",
                DepositAmount = 50,
                ProductId = 2000,
                OptionId = 3001,
                Price = (decimal)16.50,
                ProductType = "C",
                OptionTitle = "Sole occupancy",
                OrderIndex = 3,
                Category = "F/10",
                VoucherCategory = ""
            };

            _courseBespoke7Night = new Product
            {
                Title = "Test Bespoke Course 4 Day",
                Location = "Juniper Hall",
                StartDate = new DateTime(2013, 08, 05),
                FinishDate = new DateTime(2013, 08, 12),
                LocationCode = "JH",
                DepositAmount = 50,
                ProductId = 2000,
                OptionId = 3001,
                Price = (decimal)16.50,
                ProductType = "C",
                OptionTitle = "Sole occupancy",
                OrderIndex = 3,
                Category = "F/10",
                VoucherCategory = ""
            };

            _courseBespoke8Night = new Product
            {
                Title = "Test Bespoke Course 4 Day",
                Location = "Juniper Hall",
                StartDate = new DateTime(2013, 08, 05),
                FinishDate = new DateTime(2013, 08, 13),
                LocationCode = "JH",
                DepositAmount = 50,
                ProductId = 2000,
                OptionId = 3001,
                Price = (decimal)16.50,
                ProductType = "C",
                OptionTitle = "Sole occupancy",
                OrderIndex = 3,
                Category = "F/10",
                VoucherCategory = ""
            };


        }


        [TestMethod]
        public void ShouldCalculateThreeNightBespokeFamilyCourse()
        {
            decimal expectedPrice = (decimal)_courseBespoke3Night.Price*3;
            decimal actualPrice = (decimal)_sut.CreateBespokePrice(_courseBespoke3Night, 1).Price;
            Assert.AreEqual(expectedPrice,actualPrice);
        }

        [TestMethod]
        public void ShouldCalculateFourNightBespokeFamilyCourse()
        {
            decimal expectedPrice = (decimal)_courseBespoke4Night.Price * 3;
            decimal actualPrice = (decimal)_sut.CreateBespokePrice(_courseBespoke4Night, 3).Price;
            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void ShouldCalculateSevenDayBespokeFamilyCourse()
        {
            decimal expectedPrice = (decimal)_courseBespoke7Night.Price * 6;
            decimal actualPrice = (decimal)_sut.CreateBespokePrice(_courseBespoke7Night, 3).Price;
            Assert.AreEqual(expectedPrice, actualPrice); ;
        }

        [TestMethod]
        public void ShouldCalculateEightDayBespokeFamilyCourse()
        {
            decimal expectedPrice = (decimal)_courseBespoke8Night.Price * 6;
            decimal actualPrice = (decimal)_sut.CreateBespokePrice(_courseBespoke8Night, 3).Price;
            Assert.AreEqual(expectedPrice, actualPrice); 
        }

    }
}
