using System;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Helper.Config;
using CustomerPortalExtensions.Infrastructure.ECommerce.Location;
using CustomerPortalExtensions.Infrastructure.ECommerce.Products;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.ECommerce;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class ProductRepositoryIntegrationTest
    {
        /** based on following config section
   <CustomerPortalExtensions defaultNumberFormat="£#,##0.00" numberOfOrders="2" discountHandlerOrder1="Default" discountHandlerOrder2="Qty" shippingHandlerOrder1="Default" shippingHandlerOrder2="QtyAndLocation" emailNewsletterAPIKey="4e948d1c8fec9e1ce06bff93151e9cb2-us4" emailNewsletterListID="aa412643ce" >
  	<productTypes>
		<add name="Membership" docType="FSCMembership" externalIdProperty="price" productType="M" />
		<add name="Course" docType="FSCIndividualsAndFamiliesCourse" optionDocType="FSCIndividualsAndFamiliesCourseBookingOption" productType="C" />
		<add name="Publication" docType="FSCPublicationsPage" productType="P" />	
	</productTypes>
  </CustomerPortalExtensions>
         * 
         */
        private IProductRepository _sut;
        private IConfigurationService _configurationService;
        private ILocationHandlerFactory _locationHandlerFactory;

        [TestInitialize]
        public void Initialise()
        {
            _configurationService = new Configuration();
            _locationHandlerFactory=new LocationHandlerFactory()  ;
            _sut = new ProductRepository(_configurationService,_locationHandlerFactory);
        }


        [TestMethod]
        public void GetProduct()
        {
            ProductOperationStatus productOperation = _sut.GetProduct(24126,0);
            Assert.IsTrue(productOperation.Status);
        }

        [TestMethod]
        public void GetOption()
        {
            //TODO: proper values for option!
            ProductOperationStatus productOperation = _sut.GetProduct(24763,55555);
            Assert.IsTrue(productOperation.Status);

        }
    }
}
