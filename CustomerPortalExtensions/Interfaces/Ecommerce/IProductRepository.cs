using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{

    public interface IProductRepository
    {
        ProductOperationStatus GetProduct(int productId,int optionId);
        ProductOptionOperationStatus GetProductOptions(int productId);
        PaymentOptionOperationStatus GetPaymentOptions(int productId);
        PaymentOptionOperationStatus GetPaymentOptions(int productId, int optionId, decimal productLineTotal, int quantity);
    }
}