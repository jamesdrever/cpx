using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface ILocationHandlerFactory
    {
        ILocationHandler GetLocationHandler(string config);
    }
}