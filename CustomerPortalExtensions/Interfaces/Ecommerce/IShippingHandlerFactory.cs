namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IShippingHandlerFactory
    {
        IShippingHandler getShippingHandler(string config);
    }
}