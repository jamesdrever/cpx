using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.MVC.Models.Ecommerce
{
    public class OrderViewModel    
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public int OrderIndex { get; set; }
        public string Description { get; set; }
        public IList<OrderLine> CurrentOrderLines { get; set; }
        public string SpecialRequirements { get; set; }
        public bool Status { get; set; }
        public string Message { get; set;  }
        public int NumberOfItems { get; set; }
        public decimal ProductSubTotal { get; set; }
        public decimal PaymentSubTotal { get; set; }
        public string ShippingInfo { get; set; }
        public decimal ShippingTotal { get; set; }
        public string DiscountInfo { get; set; }
        public decimal DiscountTotal { get; set; }
        public int VoucherId { get; set; }
        public string VoucherInfo { get; set; }
        public decimal VoucherTotal { get; set; }
        public decimal PaymentSubTotalIncludingDiscountAndVoucher { get; set; }
        public decimal PaymentTotal { get; set; }
        public bool ContainsGiftAidableProducts { get; set; }
        public bool GiftAidAgreement { get; set; }
        public string CheckoutPage { get; set;  }
        public string OrderPage { get; set; }
        public string ContactDetailsPage { get; set; }
        public string PaymentGatewayForm { get; set; }
        public string PaymentGatewayAccount { get; set; }
        public string PaymentGatewayCheckCode { get; set; }
        public string PaymentGatewayCallbackUrl { get; set; }
        public string PaymentGatewayCompletionPage { get; set; }
        public string SpecialRequirementsText { get; set; }
        public Contact ContactDetails { get; set; }
        public string FullErrorDetails { set; get; }
    }

    public class OrderSummaryViewModel    
    {
        public int OrderId { get; set; }
        public int NumberOfItems { get; set; }
        public string ProductSubTotal { get; set; }
        public string PaymentTotal { get; set; }
        public bool Status { get; set; }
        public string FullErrorDetails { set; get; }
    }
}