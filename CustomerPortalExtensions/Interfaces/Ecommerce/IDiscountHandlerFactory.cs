namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IDiscountHandlerFactory
    {
        IDiscountHandler getDiscountHandler(string config);
    }
}