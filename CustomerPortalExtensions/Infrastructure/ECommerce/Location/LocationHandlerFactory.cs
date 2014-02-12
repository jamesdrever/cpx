using CustomerPortalExtensions.Application.Ecommerce.Locations;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Location
{
    public class LocationHandlerFactory : ILocationHandlerFactory
    {
        //private readonly IConfigurationService _configuration;

        #region IDiscountHandlerFactory Members

        public ILocationHandler GetLocationHandler(string config)
        {
            switch (config)
            {
                case "umbraco": return new LocationHandler();
                case "family": return new FamilyLocationHandler();
            }
            return new DefaultLocationHandler();
        }

        #endregion
    }
}