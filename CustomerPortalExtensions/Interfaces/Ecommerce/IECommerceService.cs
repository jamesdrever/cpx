using System;
using System.Collections.Generic;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IEcommerceService
    {
        OrderOperationStatus GetOrder();
        OrderOperationStatus GetOrder(int orderIndex);
        OrderOperationStatus AddProductToOrder(int productId, int quantity);
        OrderOperationStatus AddProductToOrder(int productId, int optionId, int quantity, string paymentType);
        OrderOperationStatus AddCourseWithFlexibleDatesToOrder(int productId, int optionId, DateTime startDate, DateTime finishDate, int quantity, string paymentType);        
        OrderOperationStatus AddDonationToOrder(int productId);
        OrderOperationStatus AddFlexibleDonationToOrder(int productId,decimal paymentAmount);
        OrderOperationStatus AddVoucherToOrder(string voucherCode);

        OrderOperationStatus Update(int productId,int optionId,int quantity);
        OrderOperationStatus Update(int productId, int optionId, int newOptionId, int quantity, string paymentType);
        OrderOperationStatus Remove(int productId,int optionId);

        OrderOperationStatus UpdateSpecialRequirements(string specialRequirements);
        OrderOperationStatus UpdateSpecialRequirements(string specialRequirements, int orderIndex);
        OrderOperationStatus UpdateGiftAidAgreement(bool agreementStatus, int orderIndex);
        OrderOperationStatus ConfirmOrder(int orderIndex);
        ProductOperationStatus GetProduct(int productId);
        ProductOperationStatus GetProduct(int productId, int optionId);
        ProductOperationStatus GetProduct(int productId, int optionId, int quantity, DateTime? startDate,
    DateTime? finishDate);
        ProductOptionOperationStatus GetProductOptions(int productId);
        PaymentOptionOperationStatus GetPaymentOptions(int productId);
        ProductSummaryOperationStatus GetProductSummary(int productId);

    }
}