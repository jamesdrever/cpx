using CustomerPortalExtensions.Config;

namespace CustomerPortalExtensions.Interfaces.Config
{
    public interface ICustomerPortalConfiguration
    {
        string ProductExternalIdProperty { get; set; }
        string ProductTitleProperty { get; set; }
        string ProductOptionProperty { get; set; }
        string ProductOptionExternalIdProperty { get; set; }
        string ProductCategoryProperty { get; set; }
        string ProductVoucherCategoryProperty { get; set; }
        string ProductCodeProperty { get; set; }
        string ProductLocationProperty { get; set; }
        string ProductStartDateProperty { get; set; }
        string ProductFinishDateProperty { get; set; }
        string ProductStartDateRange1Property { get; set; }
        string ProductFinishDateRange1Property { get; set; }
        string ProductStartDateRange2Property { get; set; }
        string ProductFinishDateRange2Property { get; set; }
        string ProductPriceProperty { get; set; }
        string ProductEarlyBirdPriceProperty { get; set; }
        string ProductEarlyBirdPriceCutOffDateProperty { get; set; }
        string ProductStatusProperty { get; set; }
        string ProductEcommerceDisabledProperty { get; set; }
        string ProductDepositOptionDisabledProperty { get; set; }
        decimal ProductDepositAmount { get; set; }
        decimal ProductMinAmountForDepositOption { get; set; }
        string ProductType { get; set; }
        string VoucherIdProperty { get; set; }
        string VoucherTitleProperty { get; set; }
        string VoucherCodeProperty { get; set; }
        string VoucherAmountProperty { get; set; }
        string VoucherAmountPerItemProperty { get; set; }
        string VoucherPercentageProperty { get; set; }
        string VoucherCategoryFilterProperty { get; set; }
        string VoucherProductCategoryFilterProperty { get; set; }
        string VoucherMinimumPaymentProperty { get; set; }
        string VoucherMinimumItemsProperty { get; set; }
        string VoucherSearchProvider { get; set; }
        string DefaultNumberFormat { get; set; }
        string ShippingHandler { get; set; }
        string DiscountHandler { get; set; }
        string EmailNewsletterAPIKey { get; set; }
        string EmailNewsletterListID { get; set; }
        string LocationHandler { get; set; }
        string CheckoutPage { get; set; }
        string ContactDetailsPage { get; set; }
        string OrderPage { get; set; }
        string SpecialRequirementsText { get; set; }
        string PaymentGatewayForm { get; set; }
        string PaymentGatewayAccount { get; set; }
        //TODO: these return types should be interfaces
        ProductTypeSection ProductTypeSettings { get; set; }
        VoucherTypeSection VoucherTypeSettings { get; set; }
        OrderTypeSection OrderTypeSettings { get; set; }
    }

    //TODO: this doesn't properly enforce creation of this interface to 
    //return IProductTypeSetting
    public interface IProductTypeSetting
    {
        string Name { get; }
        string DocType { get;  }
        string ProductType { get; set; }
        string StatusProperty { get; set; }
        string TitleProperty { get; set; }
        string CodeProperty { get; set; }
        string ExternalIdProperty { get; set; }
        string LocationProperty { get; set; }
        string OptionDocType { get; }
        string OptionExternalIdProperty { get; set; }
        string PriceProperty { get; set; }
        string EarlyBirdPriceProperty { get; set; }
        string EarlyBirdPriceCutOffDateProperty { get; set; }
        string CategoryProperty { get; set; }
        string VoucherCategoryProperty { get; set; }
        string OptionProperty { get; set; }
        string StartDateProperty { get; set; }
        string FinishDateProperty { get; set; }
        string StartDateRange1Property { get; set; }
        string FinishDateRange1Property { get; set; }
        string StartDateRange2Property { get; set; }
        string FinishDateRange2Property { get; set; }
        string EcommerceDisabledProperty { get; set; }
        string DepositOptionDisabledProperty { get; set; }
        int OrderIndex { get; set; }
        string LocationHandler { get; set; }
        decimal DepositAmount { get; set; }
        decimal MinAmountForDepositOption { get; set; }
        bool WaitingList { get; set; }
        bool AllowProvisionalBookings { get; set; }
        /// <summary>
        /// the number of days from which a deposit can no longer be taken for a course
        /// </summary>
        int DaysDepositLimit { get; set; }
        int MaximumQuantity { get; set; }
        string BespokePricingHandler { get; set; }
    }



    public interface IVoucherTypeSetting
    {
        string Name{ get; }
        string DocType { get; }
        string IdProperty { get; set; }
        string TitleProperty { get; set; }
        string CodeProperty { get; set; }
        string AmountProperty { get; set; }
        string AmountPerItemProperty { get; set; }
        string PercentageProperty { get; set; }
        string CategoryFilterProperty { get; set; }
        string ProductCategoryFilterProperty { get; set; }
        string MinimumPaymentProperty { get; set; }
        string MinimumItemsProperty { get; set; }
        int OrderIndex { get; set; }
    }


    public interface IOrderTypeSetting
    {
        string Index { get; }
        string Description { get; set; }
        string ShippingHandler { get; set; }
        string DiscountHandler { get; set; }
        string CheckoutPage { get; set; }
        string OrderPage { get; set; }
        string ContactDetailsPage { get; set; }
        string SpecialRequirementsText { get; set; }
        string PaymentGatewayForm { get; set; }
        string PaymentGatewayAccount { get; set; }
        string PaymentGatewayCheckCode { get; set; }
        string PaymentGatewayCallbackUrl { get; set; }
        string PaymentGatewayCompletionPage { get; set; }
        string AdditionalQueueProcessingHandler { get; set; }
    }
}