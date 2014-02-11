using System;
using System.Configuration;
using CustomerPortalExtensions.Interfaces.Config;

namespace CustomerPortalExtensions.Config
{
    public class CustomerPortalSection : ConfigurationSection, ICustomerPortalConfiguration
    {

        [ConfigurationProperty("productTitleProperty", DefaultValue = "pageName", IsRequired = false)]
        public string ProductTitleProperty
        {
            get { return (String) this["productTitleProperty"]; }
            set { this["productTitleProperty"] = value; }
        }


        [ConfigurationProperty("productOptionProperty", DefaultValue = "pageName", IsRequired = false)]
        public string ProductOptionProperty
        {
            get { return (String) this["productOptionProperty"]; }
            set { this["productOptionProperty"] = value; }
        }


        [ConfigurationProperty("productExternalIdProperty", DefaultValue = "externalId", IsRequired = false)]
        public string ProductExternalIdProperty
        {
            get { return (String) this["productExternalIdProperty"]; }
            set { this["productExternalIdProperty"] = value; }
        }


        [ConfigurationProperty("productOptionExternalIdProperty", DefaultValue = "externalOptionId",
            IsRequired = false)]
        public string ProductOptionExternalIdProperty
        {
            get { return (String) this["productOptionExternalIdProperty"]; }
            set { this["productOptionExternalIdProperty"] = value; }
        }

        [ConfigurationProperty("productCategoryProperty", DefaultValue = "category", IsRequired = false)]
        public string ProductCategoryProperty
        {
            get { return (String) this["productCategoryProperty"]; }
            set { this["productCategoryProperty"] = value; }
        }

        [ConfigurationProperty("productVoucherCategoryProperty", DefaultValue = "voucherCategory", IsRequired = false)]
        public string ProductVoucherCategoryProperty
        {
            get { return (String)this["productVoucherCategoryProperty"]; }
            set { this["productVoucherCategoryProperty"] = value; }
        }

        [ConfigurationProperty("productCodeProperty", DefaultValue = "code", IsRequired = false)]
        public string ProductCodeProperty
        {
            get { return (String)this["productCodeProperty"]; }
            set { this["productCodeProperty"] = value; }
        }


        [ConfigurationProperty("productStatusProperty", DefaultValue = "status", IsRequired = false)]
        public string ProductStatusProperty
        {
            get { return (String) this["productStatusProperty"]; }
            set { this["productStatusProperty"] = value; }
        }


        [ConfigurationProperty("productEcommerceDisabledProperty", DefaultValue = "status", IsRequired = false)]
        public string ProductEcommerceDisabledProperty
        {
            get { return (String) this["productEcommerceDisabledProperty"]; }
            set { this["productEcommerceDisabledProperty"] = value; }
        }

        [ConfigurationProperty("productDepositOptionDisabledProperty", DefaultValue = "disableDepositOption", IsRequired = false)]
        public string ProductDepositOptionDisabledProperty
        {
            get { return (String)this["depositOptionDisabledProperty"]; }
            set { this["depositOptionDisabledProperty"] = value; }
        }


        [ConfigurationProperty("locationProperty", DefaultValue = "location", IsRequired = false)]
        public string ProductLocationProperty
        {
            get { return (String) this["locationProperty"]; }
            set { this["locationProperty"] = value; }
        }

        [ConfigurationProperty("startDateProperty", DefaultValue = "startDate", IsRequired = false)]
        public string ProductStartDateProperty
        {
            get { return (String) this["startDateProperty"]; }
            set { this["startDate"] = value; }
        }


        [ConfigurationProperty("startDateRange1Property", DefaultValue = "startDateRange1", IsRequired = false)]
        public string ProductStartDateRange1Property
        {
            get { return (String)this["startDateRange1Property"]; }
            set { this["startDateRange1Property"] = value; }
        }

        [ConfigurationProperty("finishDateRange1Property", DefaultValue = "finishDateRange1", IsRequired = false)]
        public string ProductFinishDateRange1Property
        {
            get { return (String)this["finishDateRange1Property"]; }
            set { this["finishDateRange1Property"] = value; }
        }

        [ConfigurationProperty("startDateRange2Property", DefaultValue = "startDateRange2", IsRequired = false)]
        public string ProductStartDateRange2Property
        {
            get { return (String)this["startDateRange2Property"]; }
            set { this["startDateRange2Property"] = value; }
        }

        [ConfigurationProperty("finishDateRange2Property", DefaultValue = "finishDateRange2", IsRequired = false)]
        public string ProductFinishDateRange2Property
        {
            get { return (String)this["finishDateRange2Property"]; }
            set { this["finishDateRange2Property"] = value; }
        }


        [ConfigurationProperty("finishDateProperty", DefaultValue = "finishDate", IsRequired = false)]
        public string ProductFinishDateProperty
        {
            get { return (String) this["finishDateProperty"]; }
            set { this["finishDateProperty"] = value; }
        }


        [ConfigurationProperty("productPriceProperty", DefaultValue = "price", IsRequired = false)]
        public string ProductPriceProperty
        {
            get { return (String) this["productPriceProperty"]; }
            set { this["productPriceProperty"] = value; }
        }

        [ConfigurationProperty("earlyBirdPriceProperty", DefaultValue = "earlyBirdPrice", IsRequired = false)]
        public string ProductEarlyBirdPriceProperty
        {
            get { return (String)this["earlyBirdPriceProperty"]; }
            set { this["earlyBirdPriceProperty"] = value; }
        }

        [ConfigurationProperty("earlyBirdPriceCutOffDateProperty", DefaultValue = "earlyBirdPriceCutOffDate", IsRequired = false)]
        public string ProductEarlyBirdPriceCutOffDateProperty
        {
            get { return (String)this["earlyBirdPriceCutOffDateProperty"]; }
            set { this["earlyBirdPriceCutOffDateProperty"] = value; }
        }

        [ConfigurationProperty("productType", DefaultValue = "P", IsRequired = false)]
        public string ProductType
        {
            get { return (String) this["productType"]; }
            set { this["productType"] = value; }
        }

        [ConfigurationProperty("productDepositAmount", DefaultValue = "0", IsRequired = false)]
        public decimal ProductDepositAmount
        {
            get { return (decimal) this["productDepositAmount"]; }
            set { this["productDepositAmount"] = value; }
        }

        [ConfigurationProperty("productMinAmountForDepositOption", DefaultValue = "0", IsRequired = false)]
        public decimal ProductMinAmountForDepositOption
        {
            get { return (decimal)this["productMinAmountForDepositOption"]; }
            set { this["productMinAmountForDepositOption"] = value; }
        }

        [ConfigurationProperty("voucherTitleProperty", DefaultValue = "pageName", IsRequired = false)]
        public string VoucherTitleProperty
        {
            get { return (String)this["voucherTitleProperty"]; }
            set { this["voucherTitleProperty"] = value; }
        }

        [ConfigurationProperty("voucherIdProperty", DefaultValue = "id", IsRequired = false)]
        public string VoucherIdProperty
        {
            get { return (String)this["voucherIdProperty"]; }
            set { this["voucherIdProperty"] = value; }
        }

        [ConfigurationProperty("voucherCodeProperty", DefaultValue = "voucherCode", IsRequired = false)]
        public string VoucherCodeProperty
        {
            get { return (String)this["voucherCodeProperty"]; }
            set { this["voucherCodeProperty"] = value; }
        }


        [ConfigurationProperty("voucherAmountProperty", DefaultValue = "voucherAmount", IsRequired = false)]
        public string VoucherAmountProperty
        {
            get { return (String) this["voucherAmountProperty"]; }
            set { this["voucherAmountProperty"] = value; }
        }

        [ConfigurationProperty("voucherAmountPerItemProperty", DefaultValue = "voucherAmountPerItem", IsRequired = false)]
        public string VoucherAmountPerItemProperty
        {
            get { return (String)this["voucherAmountPerItemProperty"]; }
            set { this["voucherAmountPerItemProperty"] = value; }
        }


        [ConfigurationProperty("voucherPerCentProperty", DefaultValue = "voucherPerCent", IsRequired = false)]
        public string VoucherPercentageProperty
        {
            get { return (String) this["voucherPerCentProperty"]; }
            set { this["voucherPerCentProperty"] = value; }
        }

        [ConfigurationProperty("voucherProductCategoryFilterProperty", DefaultValue = "productCategoryFilter", IsRequired = false)]
        public string VoucherProductCategoryFilterProperty
        {
            get { return (String) this["voucherProductCategoryFilterProperty"]; }
            set { this["voucherProductCategoryFilterProperty"] = value; }
        }

        [ConfigurationProperty("voucherCategoryFilterProperty", DefaultValue = "voucherCategoryFilter", IsRequired = false)]
        public string VoucherCategoryFilterProperty
        {
            get { return (String)this["voucherCategoryFilterProperty"]; }
            set { this["voucherCategoryFilterProperty"] = value; }
        }


        [ConfigurationProperty("voucherMinimumPaymentProperty", DefaultValue = "voucherMinimumPayment", IsRequired = false)]
        public string VoucherMinimumPaymentProperty
        {
            get { return (String)this["voucherMinimumPaymentProperty"]; }
            set { this["voucherMinimumPaymentProperty"] = value; }
        }

        [ConfigurationProperty("voucherMinimumItemsProperty", DefaultValue = "voucherMinimumItems", IsRequired = false)]
        public string VoucherMinimumItemsProperty
        {
            get { return (String)this["voucherMinimumItemsProperty"]; }
            set { this["voucherMinimumItemsProperty"] = value; }
        }


        [ConfigurationProperty("voucherSearchProvider", DefaultValue = "cpxVouchersSearcher", IsRequired = false)]
        public string VoucherSearchProvider
        {
            get { return (String)this["voucherSearchProvider"]; }
            set { this["voucherSearchProvider"] = value; }
        }

        [ConfigurationProperty("defaultNumberFormat", DefaultValue = "£#,##0.00", IsRequired = false)]
        public string DefaultNumberFormat
        {
            get { return (String) this["defaultNumberFormat"]; }
            set { this["defaultNumberFormat"] = value; }
        }

        [ConfigurationProperty("shippingHandler", DefaultValue = "default", IsRequired = false)]
        public string ShippingHandler
        {
            get { return (String) this["shippingHandler"]; }
            set { this["shippingHandler"] = value; }
        }

        [ConfigurationProperty("discountHandler", DefaultValue = "default", IsRequired = false)]
        public string DiscountHandler
        {
            get { return (String) this["discountHandler"]; }
            set { this["discountHandler"] = value; }
        }

        [ConfigurationProperty("locationHandler", DefaultValue = "default", IsRequired = false)]
        public string LocationHandler
        {
            get { return (String) this["locationHandler"]; }
            set { this["locationHandler"] = value; }
        }

        [ConfigurationProperty("emailNewsletterAPIKey", DefaultValue = "default", IsRequired = false)]
        public string EmailNewsletterAPIKey
        {
            get { return (String) this["emailNewsletterAPIKey"]; }
            set { this["emailNewsletterAPIKey"] = value; }
        }

        [ConfigurationProperty("emailNewsletterListID", DefaultValue = "default", IsRequired = false)]
        public string EmailNewsletterListID
        {
            get { return (String) this["emailNewsletterListID"]; }
            set { this["emailNewsletterListID"] = value; }
        }

        [ConfigurationProperty("checkoutPage", DefaultValue = "/checkout.aspx", IsRequired = false)]
        public string CheckoutPage
        {
            get { return (String)this["checkoutPage"]; }
            set { this["checkoutPage"] = value; }
        }

        [ConfigurationProperty("contactDetailsPage", DefaultValue = "/contact-details.aspx", IsRequired = false)]
        public string ContactDetailsPage
        {
            get { return (String)this["contactDetailsPage"]; }
            set { this["contactDetailsPage"] = value; }
        }

        [ConfigurationProperty("orderPage", DefaultValue = "/order.aspx", IsRequired = false)]
        public string OrderPage
        {
            get { return (String)this["orderPage"]; }
            set { this["orderPage"] = value; }
        }

        [ConfigurationProperty("specialRequirementsText", DefaultValue = "", IsRequired = false)]
        public string SpecialRequirementsText
        {
            get { return (String)this["specialRequirementsText"]; }
            set { this["specialRequirementsText"] = value; }
        }

        [ConfigurationProperty("paymentGatewayForm", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayForm
        {
            get { return (String)this["paymentGatewayForm"]; }
            set { this["paymentGatewayForm"] = value; }
        }

        [ConfigurationProperty("paymentGatewayAccount", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayAccount
        {
            get { return (String)this["paymentGatewayAccount"]; }
            set { this["paymentGatewayAccount"] = value; }
        }

        [ConfigurationProperty("productTypes", IsDefaultCollection = true)]
        public ProductTypeSection ProductTypeSettings
        {
            get { return (ProductTypeSection) this["productTypes"]; }
            set { this["productTypes"] = value; }
        }

        [ConfigurationProperty("voucherTypes", IsDefaultCollection = true)]
        public VoucherTypeSection VoucherTypeSettings
        {
            get { return (VoucherTypeSection)this["voucherTypes"]; }
            set { this["voucherTypes"] = value; }
        }

        [ConfigurationProperty("orderTypes", IsDefaultCollection = true)]
        public OrderTypeSection OrderTypeSettings
        {
            get { return (OrderTypeSection) this["orderTypes"]; }
            set { this["orderTypes"] = value; }
        }



    }

    public class ProductTypeSetting : ConfigurationElement, IProductTypeSetting
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string) this["name"]; }
        }

        [ConfigurationProperty("docType")]
        public string DocType
        {
            get { return (string) this["docType"]; }
        }

        [ConfigurationProperty("productType", DefaultValue = "P", IsRequired = false)]
        public string ProductType
        {
            get { return (String) this["productType"]; }
            set { this["productType"] = value; }
        }

        [ConfigurationProperty("titleProperty", DefaultValue = "pageName", IsRequired = false)]
        public string TitleProperty
        {
            get { return (String) this["titleProperty"]; }
            set { this["titleProperty"] = value; }
        }

        [ConfigurationProperty("priceProperty", DefaultValue = "price", IsRequired = false)]
        public string PriceProperty
        {
            get { return (String) this["priceProperty"]; }
            set { this["priceProperty"] = value; }
        }

        [ConfigurationProperty("earlyBirdPriceProperty", DefaultValue = "earlyBirdPrice", IsRequired = false)]
        public string EarlyBirdPriceProperty
        {
            get { return (String)this["earlyBirdPriceProperty"]; }
            set { this["earlyBirdPriceProperty"] = value; }
        }

        [ConfigurationProperty("earlyBirdPriceCutOffDateProperty", DefaultValue = "earlyBirdPriceCutOffDate", IsRequired = false)]
        public string EarlyBirdPriceCutOffDateProperty
        {
            get { return (String)this["earlyBirdPriceCutOffDateProperty"]; }
            set { this["earlyBirdPriceCutOffDateProperty"] = value; }
        }

        [ConfigurationProperty("optionProperty", DefaultValue = "pageName", IsRequired = false)]
        public string OptionProperty
        {
            get { return (String) this["optionProperty"]; }
            set { this["optionProperty"] = value; }
        }


        [ConfigurationProperty("locationProperty", DefaultValue = "location", IsRequired = false)]
        public string LocationProperty
        {
            get { return (String) this["locationProperty"]; }
            set { this["locationProperty"] = value; }
        }

        [ConfigurationProperty("categoryProperty", DefaultValue = "category", IsRequired = false)]
        public string CategoryProperty
        {
            get { return (String) this["categoryProperty"]; }
            set { this["categoryProperty"] = value; }
        }

        [ConfigurationProperty("voucherCategoryProperty", DefaultValue = "voucherCategory", IsRequired = false)]
        public string VoucherCategoryProperty
        {
            get { return (String)this["voucherCategoryProperty"]; }
            set { this["voucherCategoryProperty"] = value; }
        }

        [ConfigurationProperty("codeProperty", DefaultValue = "code", IsRequired = false)]
        public string CodeProperty
        {
            get { return (String)this["codeProperty"]; }
            set { this["codeProperty"] = value; }
        }

        [ConfigurationProperty("statusProperty", DefaultValue = "status", IsRequired = false)]
        public string StatusProperty
        {
            get { return (String) this["statusProperty"]; }
            set { this["statusProperty"] = value; }
        }

        [ConfigurationProperty("ecommerceDisabledProperty", DefaultValue = "ecommerceDisabled", IsRequired = false)]
        public string EcommerceDisabledProperty
        {
            get { return (String) this["ecommerceDisabledProperty"]; }
            set { this["ecommerceDisabledProperty"] = value; }
        }

        [ConfigurationProperty("depositOptionDisabledProperty", DefaultValue = "disableDepositOption", IsRequired = false)]
        public string DepositOptionDisabledProperty
        {
            get { return (String)this["depositOptionDisabledProperty"]; }
            set { this["depositOptionDisabledProperty"] = value; }
        }

        [ConfigurationProperty("startDateProperty", DefaultValue = "startDate", IsRequired = false)]
        public string StartDateProperty
        {
            get { return (String) this["startDateProperty"]; }
            set { this["startDateProperty"] = value; }
        }

        [ConfigurationProperty("finishDateProperty", DefaultValue = "finishDate", IsRequired = false)]
        public string FinishDateProperty
        {
            get { return (String) this["finishDateProperty"]; }
            set { this["finishDateProperty"] = value; }
        }

        [ConfigurationProperty("startDateRange1Property", DefaultValue = "startDateRange1", IsRequired = false)]
        public string StartDateRange1Property
        {
            get { return (String)this["startDateRange1Property"]; }
            set { this["startDateRange1Property"] = value; }
        }

        [ConfigurationProperty("finishDateRange1Property", DefaultValue = "finishDateRange1", IsRequired = false)]
        public string FinishDateRange1Property
        {
            get { return (String)this["finishDateRange1Property"]; }
            set { this["finishDateRange1Property"] = value; }
        }

        [ConfigurationProperty("startDateRange2Property", DefaultValue = "startDateRange2", IsRequired = false)]
        public string StartDateRange2Property
        {
            get { return (String)this["startDateRange2Property"]; }
            set { this["startDateRange2Property"] = value; }
        }

        [ConfigurationProperty("finishDateRange2Property", DefaultValue = "finishDateRange2", IsRequired = false)]
        public string FinishDateRange2Property
        {
            get { return (String)this["finishDateRange2Property"]; }
            set { this["finishDateRange2Property"] = value; }
        }


        [ConfigurationProperty("optionDocType")]
        public string OptionDocType
        {
            get { return (string) this["optionDocType"]; }
        }

        [ConfigurationProperty("optionProperty", DefaultValue = "pageName", IsRequired = false)]
        public string ProductOptionProperty
        {
            get { return (String) this["optionProperty"]; }
            set { this["optionProperty"] = value; }
        }


        [ConfigurationProperty("externalIdProperty", DefaultValue = "externalId", IsRequired = false)]
        public string ExternalIdProperty
        {
            get { return (String) this["externalIdProperty"]; }
            set { this["externalIdProperty"] = value; }
        }


        [ConfigurationProperty("optionExternalIdProperty", DefaultValue = "externalOptionId",
            IsRequired = false)]
        public string OptionExternalIdProperty
        {
            get { return (String) this["optionExternalIdProperty"]; }
            set { this["optionExternalIdProperty"] = value; }
        }

        [ConfigurationProperty("orderIndex", DefaultValue = "1",
            IsRequired = false)]
        public int OrderIndex
        {
            get { return (int)this["orderIndex"]; }
            set { this["orderIndex"] = value; }
        }

        [ConfigurationProperty("depositAmount", DefaultValue = "1",
            IsRequired = false)]
        public decimal DepositAmount
        {
            get { return (decimal) this["depositAmount"]; }
            set { this["depositAmount"] = value; }
        }


        [ConfigurationProperty("minAmountForDepositOption", DefaultValue = "0",
            IsRequired = false)]
        public decimal MinAmountForDepositOption
        {
            get { return (decimal)this["minAmountForDepositOption"]; }
            set { this["minAmountForDepositOption"] = value; }
        }

        [ConfigurationProperty("daysDepositLimit", DefaultValue = "1",
            IsRequired = false)]
        public int DaysDepositLimit
        {
            get { return (int) this["daysDepositLimit"]; }
            set { this["daysDepositAmount"] = value; }
        }

        [ConfigurationProperty("waitingList", DefaultValue = "false",
            IsRequired = false)]
        public bool WaitingList
        {
            get { return (bool) this["waitingList"]; }
            set { this["waitingList"] = value; }
        }

        [ConfigurationProperty("allowProvisionalBookings", DefaultValue = "false",
            IsRequired = false)]
        public bool AllowProvisionalBookings
        {
            get { return (bool)this["allowProvisionalBookings"]; }
            set { this["allowProvisionalBookings"] = value; }
        }



        [ConfigurationProperty("maximumQuantity", DefaultValue = "100", IsRequired = false)]
        public int MaximumQuantity
        {
            get { return (int) this["maximumQuantity"]; }
            set { this["maximumQuantity"] = value; }
        }

        [ConfigurationProperty("bespokePricingHandler", DefaultValue = "", IsRequired = false)]
        public string BespokePricingHandler {
            get { return (String) this["bespokePricingHandler"]; }
            set { this["bespokePricingHandler"] = value; }
        }

        [ConfigurationProperty("locationHandler", DefaultValue = "default", IsRequired = false)]
        public string LocationHandler
        {
            get { return (String) this["locationHandler"]; }
            set { this["locationHandler"] = value; }
        }


    }

    public class VoucherTypeSetting : ConfigurationElement, IVoucherTypeSetting
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("docType")]
        public string DocType
        {
            get { return (string)this["docType"]; }
        }

        [ConfigurationProperty("idProperty", DefaultValue = "id", IsRequired = false)]
        public string IdProperty
        {
            get { return (String)this["idProperty"]; }
            set { this["idProperty"] = value; }
        }

        [ConfigurationProperty("titleProperty", DefaultValue = "pageName", IsRequired = false)]
        public string TitleProperty
        {
            get { return (String)this["titleProperty"]; }
            set { this["titleProperty"] = value; }
        }

        [ConfigurationProperty("codeProperty", DefaultValue = "voucherCode", IsRequired = false)]
        public string CodeProperty
        {
            get { return (String)this["codeProperty"]; }
            set { this["codeProperty"] = value; }
        }

        [ConfigurationProperty("amountProperty", DefaultValue = "voucherAmount", IsRequired = false)]
        public string AmountProperty
        {
            get { return (String)this["amountProperty"]; }
            set { this["amountProperty"] = value; }
        }

        [ConfigurationProperty("amountPerItemProperty", DefaultValue = "voucherAmountPerItem", IsRequired = false)]
        public string AmountPerItemProperty
        {
            get { return (String)this["amountPerItemProperty"]; }
            set { this["amountPerItemProperty"] = value; }
        }


        [ConfigurationProperty("perCentProperty", DefaultValue = "voucherPerCent", IsRequired = false)]
        public string PercentageProperty
        {
            get { return (String)this["perCentProperty"]; }
            set { this["perCentProperty"] = value; }
        }

        [ConfigurationProperty("categoryFilterProperty", DefaultValue = "categoryFilter", IsRequired = false)]
        public string CategoryFilterProperty
        {
            get { return (String)this["categoryFilterProperty"]; }
            set { this["categoryFilterProperty"] = value; }
        }

        [ConfigurationProperty("productCategoryFilterProperty", DefaultValue = "productCategoryFilter", IsRequired = false)]
        public string ProductCategoryFilterProperty
        {
            get { return (String)this["productCategoryFilterProperty"]; }
            set { this["productCategoryFilterProperty"] = value; }
        }


        [ConfigurationProperty("minimumPaymentProperty", DefaultValue = "voucherMinimumPayment", IsRequired = false)]
        public string MinimumPaymentProperty
        {
            get { return (String)this["minimumPaymentProperty"]; }
            set { this["minimumPaymentProperty"] = value; }
        }

        [ConfigurationProperty("minimumItemsProperty", DefaultValue = "voucherMinimumItems", IsRequired = false)]
        public string MinimumItemsProperty
        {
            get { return (String)this["minimumItemsProperty"]; }
            set { this["minimumItemsProperty"] = value; }
        }

        [ConfigurationProperty("orderIndex", DefaultValue = "1",IsRequired = false)]
        public int OrderIndex
        {
            get { return (int)this["orderIndex"]; }
            set { this["orderIndex"] = value; }
        }
    }

    public class OrderTypeSetting : ConfigurationElement, IOrderTypeSetting
    {
        [ConfigurationProperty("index")]
        public string Index
        {
            get { return (string) this["index"]; }
        }

        [ConfigurationProperty("description")]
        public string Description
        {
            get { return (string)this["description"]; }
            set { this["description"] = value; }
        }

        [ConfigurationProperty("shippingHandler")]
        public string ShippingHandler
        {
            get { return (string) this["shippingHandler"]; }
            set { this["shippingHandler"] = value; }
        }
        [ConfigurationProperty("discountHandler")]
        public string DiscountHandler
        {
            get { return (string)this["discountHandler"]; }
            set { this["discountHandler"] = value; }
        }
        [ConfigurationProperty("checkoutPage", DefaultValue = "/checkout.aspx", IsRequired = false)]
        public string CheckoutPage
        {
            get { return (String)this["checkoutPage"]; }
            set { this["checkoutPage"] = value; }
        }

        [ConfigurationProperty("contactDetailsPage", DefaultValue = "/contact-details.aspx", IsRequired = false)]
        public string ContactDetailsPage
        {
            get { return (String)this["contactDetailsPage"]; }
            set { this["contactDetailsPage"] = value; }
        }

        [ConfigurationProperty("orderPage", DefaultValue = "/order.aspx", IsRequired = false)]
        public string OrderPage
        {
            get { return (String)this["orderPage"]; }
            set { this["orderPage"] = value; }
        }

        [ConfigurationProperty("specialRequirementsText", DefaultValue = "", IsRequired = false)]
        public string SpecialRequirementsText
        {
            get { return (String)this["specialRequirementsText"]; }
            set { this["specialRequirementsText"] = value; }
        }

        [ConfigurationProperty("paymentGatewayForm", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayForm
        {
            get { return (String)this["paymentGatewayForm"]; }
            set { this["paymentGatewayForm"] = value; }
        }

        [ConfigurationProperty("paymentGatewayAccount", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayAccount
        {
            get { return (String)this["paymentGatewayAccount"]; }
            set { this["paymentGatewayAccount"] = value; }
        }

        [ConfigurationProperty("paymentGatewayCheckCode", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayCheckCode
        {
            get { return (String)this["paymentGatewayCheckCode"]; }
            set { this["paymentGatewayCheckCode"] = value; }
        }

        [ConfigurationProperty("paymentGatewayCallbackUrl", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayCallbackUrl
        {
            get { return (String)this["paymentGatewayCallbackUrl"]; }
            set { this["paymentGatewayCallbackUrl"] = value; }
        }

        [ConfigurationProperty("paymentGatewayCompletionPage", DefaultValue = "", IsRequired = false)]
        public string PaymentGatewayCompletionPage
        {
            get { return (String)this["paymentGatewayCompletionPage"]; }
            set { this["paymentGatewayCompletionPage"] = value; }
        }

        [ConfigurationProperty("additionalQueueProcessingHandler")]
        public string AdditionalQueueProcessingHandler
        {
            get { return (string)this["additionalQueueProcessingHandler"]; }
            set { this["additionalQueueProcessingHandler"] = value; }
        }


    }

    [ConfigurationCollection(typeof(ProductTypeSetting))]  
    public class ProductTypeSection : ConfigurationElementCollection
    {
        //Constructor
        public ProductTypeSection()
        {
            AddElementName = "ProductTypeSetting";
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProductTypeSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProductTypeSetting) element).DocType;
        }

        public IProductTypeSetting this[string key]
        {
            get { return BaseGet(key) as ProductTypeSetting; }
        }

    }

    [ConfigurationCollection(typeof(VoucherTypeSetting))]
    public class VoucherTypeSection : ConfigurationElementCollection
    {
        //Constructor
        public VoucherTypeSection()
        {
            AddElementName = "VoucherTypeSetting";
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new VoucherTypeSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((VoucherTypeSetting)element).DocType;
        }

        public IVoucherTypeSetting this[string key]
        {
            get { return BaseGet(key) as VoucherTypeSetting; }
        }

    }

    [ConfigurationCollection(typeof(OrderTypeSetting))]
    public class OrderTypeSection : ConfigurationElementCollection
    {
        //Constructor
        public OrderTypeSection()
        {
            AddElementName = "OrderTypeSetting";
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new OrderTypeSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OrderTypeSetting)element).Index;
        }

        public IOrderTypeSetting this[string key]
        {
            get { return BaseGet(key) as OrderTypeSetting; }
        }

    }
}
        
