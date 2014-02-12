using System;
using CustomerPortalExtensions.Domain.ECommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class ProductUnitTests
    {

        [TestMethod]
        public void ShouldRecogniseProductStartAndFinishDateWithinSingleRange()
        {
            Product _sut = new Product
            {
                StartDate = new DateTime(2014, 08, 15),
                FinishDate = new DateTime(2014, 08, 18),
                StartDateRange1 = new DateTime(2014, 08, 10),
                FinishDateRange1 = new DateTime(2014, 08, 20)
            };
            Assert.AreEqual(true, _sut.HasRestrictedDateRange());
            Assert.AreEqual(true, _sut.HasStartAndFinishDatesWithinRestrictedRange());
        }

        [TestMethod]
        public void ShouldRecogniseProductStartAndFinishDateOutOfSingleRange()
        {
            Product _sut = new Product
                {
                    StartDate = new DateTime(2014, 08, 05),
                    FinishDate = new DateTime(2014,08,10),
                    StartDateRange1 = new DateTime(2014, 08, 10),
                    FinishDateRange1 = new DateTime(2014, 08, 20)
                };
            Assert.AreEqual(true,_sut.HasRestrictedDateRange());
            Assert.AreEqual(false, _sut.HasStartAndFinishDatesWithinRestrictedRange());
        }
        [TestMethod]
        public void ShouldRecogniseProductStartAndFinishDateWithinDoubleRange()
        {
            Product _sut = new Product
            {
                StartDate = new DateTime(2014, 08, 15),
                FinishDate = new DateTime(2014, 08, 18),
                StartDateRange1 = new DateTime(2014, 08, 1),
                FinishDateRange1 = new DateTime(2014, 08, 8),
                StartDateRange2 = new DateTime(2014, 08, 10),
                FinishDateRange2 = new DateTime(2014, 08, 20)
            };
            Assert.AreEqual(true, _sut.HasRestrictedDateRange());
            Assert.AreEqual(true, _sut.HasStartAndFinishDatesWithinRestrictedRange());
        }

        [TestMethod]
        public void ShouldRecogniseProductStartAndFinishDateOutOfDoubleRange()
        {
            Product _sut = new Product
            {
                StartDate = new DateTime(2014, 08, 25),
                FinishDate = new DateTime(2014, 08, 28),
                StartDateRange1 = new DateTime(2014, 08, 1),
                FinishDateRange1 = new DateTime(2014, 08, 8),
                StartDateRange2 = new DateTime(2014, 08, 10),
                FinishDateRange2 = new DateTime(2014, 08, 20)
            };
            Assert.AreEqual(true, _sut.HasRestrictedDateRange());
            Assert.AreEqual(false, _sut.HasStartAndFinishDatesWithinRestrictedRange());
        }

        [TestMethod]
        public void ShouldRecogniseWhenCanUseEarlyBirdPricing()
        {
            Product _sut = new Product
            {
                Price=10,
                EarlyBirdPrice=5,
                EarlyBirdPriceCutOffDate = DateTime.Now.AddDays(5)
            };
            Assert.AreEqual(true, _sut.CanUseEarlyBirdPrice());
        }

        [TestMethod]
        public void ShouldRecogniseWhenCanUseEarlyBirdPricingOnCutOffDay()
        {
            Product _sut = new Product
            {
                Price = 10,
                EarlyBirdPrice = 5,
                EarlyBirdPriceCutOffDate = DateTime.Now
            };
            Assert.AreEqual(true, _sut.CanUseEarlyBirdPrice());
        }

        [TestMethod]
        public void ShouldRecogniseWhenCannotUseEarlyBirdPricing()
        {
            Product _sut = new Product
            {
                Price = 10,
                EarlyBirdPrice = 5,
                EarlyBirdPriceCutOffDate = DateTime.Now.AddDays(-5)
            };
            Assert.AreEqual(false, _sut.CanUseEarlyBirdPrice());
        }

    }
}
