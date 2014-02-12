namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IAdditionalQueueProcessingHandlerFactory
    {
        IAdditionalQueueProcessingHandler GetHandler(string config);
    }
}