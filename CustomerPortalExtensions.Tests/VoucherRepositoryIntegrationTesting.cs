using System;
using CustomerPortalExtensions.Config;
using CustomerPortalExtensions.Infrastructure.ECommerce.Vouchers;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class VoucherRepositoryIntegrationTesting
    {
        //TODO: can't test because of dependencies on Umbraco xml cache
        private static IVoucherRepository _sut;

        [ClassInitialize]
        public static void Initialise(TestContext context)
        {
            var configService = new Mock<IConfigurationService>();
            configService.Setup(x => x.GetConfiguration()).Returns(new CustomerPortalSection());
            _sut=new VoucherRepository(configService.Object);
        }


        [TestMethod]
        public void GetVoucher()
        {
            var operationStatus = _sut.GetVoucher("TEST");
            Assert.IsTrue(operationStatus.Status);
            Assert.IsTrue(operationStatus.Voucher.Title == "50% off");
            Assert.IsTrue(operationStatus.Voucher.Percentage == 50);
            Assert.IsTrue(operationStatus.Voucher.Amount == 0);
            

        }
    }
}
