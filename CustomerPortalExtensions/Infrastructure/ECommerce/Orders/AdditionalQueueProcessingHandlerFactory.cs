using CustomerPortalExtensions.Application.Ecommerce.OrderQueue;
using CustomerPortalExtensions.Interfaces.ECommerce;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Orders
{
    public class AdditionalQueueProcessingHandlerFactory : IAdditionalQueueProcessingHandlerFactory
    {
        //private readonly IConfigurationService _configuration;

        #region IDiscountHandlerFactory Members

        public IAdditionalQueueProcessingHandler GetHandler(string config)
        {
            switch (config)
            {
                case "family": return new FamilyAdditionalQueueProcessingHandler();
            }
            return new DefaultAdditionalQueueProcessingHandler();
        }

        #endregion
    }
}