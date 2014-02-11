using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Helper.Umbraco;
using CustomerPortalExtensions.Domain.Ecommerce;
using umbraco.interfaces;
using umbraco.NodeFactory;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfigurationService _config;
        private readonly ILocationHandlerFactory _locationHandlerFactory;

        public ProductRepository(IConfigurationService config,ILocationHandlerFactory locationHandlerFactory)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this._config = config;
            _locationHandlerFactory = locationHandlerFactory;
            if (locationHandlerFactory == null)
            {
                throw new ArgumentNullException("locationHandlerFactory");
            }
            this._locationHandlerFactory = locationHandlerFactory;
        }

        public ProductOperationStatus GetProduct(int productId, int optionId)
        {
           
            //TODO: give option of using Examine to get nodes
            var operationStatus = new ProductOperationStatus();
            try
            {
                var product = new Product();
                //get the info from Umbraco
                Node productNode = new Node(productId);
                Node optionNode = (optionId == 0) ? new Node(productId) : new Node(optionId);

                product.ProductId = productId;



                IProductTypeSetting productTypeSetting =
                    _config.GetConfiguration().ProductTypeSettings[productNode.NodeTypeAlias];

                product.OrderIndex = (productTypeSetting != null)
                                         ? productTypeSetting.OrderIndex
                                         : 1;


                product.Title = (productTypeSetting != null)
                                    ? UmbracoHelper.InsertPropertyAliases(productNode, productTypeSetting.TitleProperty)
                                    : UmbracoHelper.InsertPropertyAliases(productNode,
                                                                          _config.GetConfiguration()
                                                                                 .ProductTitleProperty);

                product.ProductUrl = (productNode.template > 0) ? productNode.NiceUrl : "";

                product.ProductExternalId = (productTypeSetting != null)
                                                ? UmbracoHelper.GetPropertyAlias(productNode,
                                                                                 productTypeSetting.ExternalIdProperty)
                                                : UmbracoHelper.GetPropertyAlias(productNode,
                                                                                 _config.GetConfiguration()
                                                                                        .ProductExternalIdProperty);

                string locationConfig = (productTypeSetting != null)
                                            ? productTypeSetting.LocationHandler
                                            : _config.GetConfiguration().LocationHandler;

                var locationHandler = _locationHandlerFactory.GetLocationHandler(locationConfig);

                string locationCode = (productTypeSetting != null)
                                          ? UmbracoHelper.GetPropertyAlias(productNode,
                                                                           productTypeSetting.LocationProperty)
                                          : "";



                product.LocationCode = locationCode;

                var location = locationHandler.GetLocation(locationCode);

                product.Location = location.Title;
                product.LocationEmail = location.Email;

                product.StartDate = (productTypeSetting != null)
                                        ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                 productTypeSetting
                                                                                     .StartDateProperty)
                                        : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                 _config.GetConfiguration()
                                                                                        .ProductStartDateProperty);

                product.FinishDate = (productTypeSetting != null)
                                         ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  productTypeSetting.FinishDateProperty)
                                         : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  _config.GetConfiguration()
                                                                                         .ProductFinishDateProperty);

                product.StartDateRange1 = (productTypeSetting != null)
                                        ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                 productTypeSetting
                                                                                     .StartDateRange1Property)
                                        : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                 _config.GetConfiguration()
                                                                                        .ProductStartDateRange1Property);

                product.FinishDateRange1 = (productTypeSetting != null)
                                         ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  productTypeSetting.FinishDateRange1Property)
                                         : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  _config.GetConfiguration()
                                                                                         .ProductFinishDateRange1Property);

                product.StartDateRange2 = (productTypeSetting != null)
                                        ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                 productTypeSetting
                                                                                     .StartDateRange2Property)
                                        : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                 _config.GetConfiguration()
                                                                                        .ProductStartDateRange2Property);

                product.FinishDateRange2 = (productTypeSetting != null)
                                         ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  productTypeSetting.FinishDateRange2Property)
                                         : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  _config.GetConfiguration()
                                                                                         .ProductFinishDateRange2Property);


                product.ProductType = (productTypeSetting != null)
                                          ? productTypeSetting.ProductType
                                          : _config.GetConfiguration().ProductType;

                product.ProductTypeCode = productNode.NodeTypeAlias;

                product.Category = (productTypeSetting != null)
                                       ? UmbracoHelper.GetPropertyAlias(productNode, productTypeSetting.CategoryProperty)
                                       : UmbracoHelper.GetPropertyAlias(productNode,
                                                                        _config.GetConfiguration()
                                                                               .ProductCategoryProperty);

                product.VoucherCategory = (productTypeSetting != null)
                                              ? UmbracoHelper.GetPropertyAlias(productNode,
                                                                               productTypeSetting
                                                                                   .VoucherCategoryProperty)
                                              : UmbracoHelper.GetPropertyAlias(productNode,
                                                                               _config.GetConfiguration()
                                                                                      .ProductVoucherCategoryProperty);

                product.Code = (productTypeSetting != null)
                                   ? UmbracoHelper.GetPropertyAlias(productNode, productTypeSetting.CodeProperty)
                                   : UmbracoHelper.GetPropertyAlias(productNode,
                                                                    _config.GetConfiguration().ProductCodeProperty);



                product.Status = (productTypeSetting != null)
                                     ? UmbracoHelper.GetPropertyAlias(productNode, productTypeSetting.StatusProperty)
                                     : UmbracoHelper.GetPropertyAlias(productNode,
                                                                      _config.GetConfiguration().ProductStatusProperty);

                product.EcommerceDisabled = (productTypeSetting != null)
                                                ? UmbracoHelper.GetBooleanPropertyAlias(productNode,
                                                                                        productTypeSetting
                                                                                            .EcommerceDisabledProperty, false)
                                                : UmbracoHelper.GetBooleanPropertyAlias(productNode,
                                                                                        _config.GetConfiguration()
                                                                                               .ProductEcommerceDisabledProperty,
                                                                                        false);

                product.DepositOptionDisabled = (productTypeSetting != null)
                                                ? UmbracoHelper.GetBooleanPropertyAlias(productNode,
                                                                                        productTypeSetting
                                                                                            .DepositOptionDisabledProperty, false)
                                                : UmbracoHelper.GetBooleanPropertyAlias(productNode,
                                                                                        _config.GetConfiguration()
                                                                                               .ProductDepositOptionDisabledProperty,
                                                                                        false);



                product.DepositAmount = (productTypeSetting != null)
                                            ? productTypeSetting.DepositAmount
                                            : _config.GetConfiguration().ProductDepositAmount;

                product.MaximumQuantity = (productTypeSetting != null)
                                              ? productTypeSetting.MaximumQuantity
                                              : 0;

                product.OptionId = optionId;

                string priceAlias = (productTypeSetting != null)
                                        ? productTypeSetting.PriceProperty
                                        : _config.GetConfiguration().ProductPriceProperty;

                string earlyBirdPriceAlias = (productTypeSetting != null)
                        ? productTypeSetting.EarlyBirdPriceProperty
                        : _config.GetConfiguration().ProductEarlyBirdPriceProperty;

                decimal? price, earlyBirdPrice = null;
                decimal tempPrice, tempEarlyBirdPrice;

                if (optionId > 0)
                {
                    product.OptionTitle = (productTypeSetting != null)
                                              ? UmbracoHelper.InsertPropertyAliases(optionNode,
                                                                                    productTypeSetting
                                                                                        .OptionProperty)
                                              : UmbracoHelper.InsertPropertyAliases(optionNode,
                                                                                    _config.GetConfiguration()
                                                                                           .ProductOptionProperty);
                    product.OptionExternalId = (productTypeSetting != null)
                                                   ? UmbracoHelper.GetPropertyAlias(optionNode,
                                                                                    productTypeSetting
                                                                                        .OptionExternalIdProperty)
                                                   : UmbracoHelper.GetPropertyAlias(optionNode,
                                                                                    _config.GetConfiguration()
                                                                                           .ProductOptionExternalIdProperty);
                    price = decimal.TryParse(UmbracoHelper.GetPropertyAlias(optionNode, priceAlias), out tempPrice) ? tempPrice : (decimal?)null;
                    earlyBirdPrice=decimal.TryParse(UmbracoHelper.GetPropertyAlias(optionNode, earlyBirdPriceAlias), out tempEarlyBirdPrice) ? tempEarlyBirdPrice : (decimal?)null;

                }
                else
                {
                    price = decimal.TryParse(UmbracoHelper.GetPropertyAlias(productNode, priceAlias), out tempPrice) ? tempPrice : (decimal?)null;
                    earlyBirdPrice = decimal.TryParse(UmbracoHelper.GetPropertyAlias(productNode, earlyBirdPriceAlias),
                        out tempEarlyBirdPrice)
                        ? tempEarlyBirdPrice
                        : (decimal?) null;

                }
                product.Price = price;
                product.EarlyBirdPrice = earlyBirdPrice;
                product.BespokePricingHandler = (productTypeSetting != null)
                                                           ? productTypeSetting.BespokePricingHandler
                                                           : "";


                product.EarlyBirdPriceCutOffDate = (productTypeSetting != null)
                                         ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  productTypeSetting.EarlyBirdPriceCutOffDateProperty)
                                         : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                                                                  _config.GetConfiguration()
                                                                                         .ProductEarlyBirdPriceCutOffDateProperty);

                operationStatus.Product = product;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ProductOperationStatus>
                    .CreateFromException("An error has occurred retrieving the product", e);
            }


            return operationStatus;
        }
 

        public ProductOptionOperationStatus GetProductOptions(int productId)
        {
            var operationStatus= new ProductOptionOperationStatus();
            try
            {

                operationStatus.ProductOptions=GetOptions(productId).ToList();
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ProductOptionOperationStatus>
                    .CreateFromException("An error has occurred retrieving the product options", e);
            }
            return operationStatus;
        }

        private IEnumerable<ProductOption> GetOptions(int productId)
        {
            var node = new Node(productId);
            var productTypeSetting = _config.GetConfiguration().ProductTypeSettings[node.NodeTypeAlias];
            if (productTypeSetting == null)
                throw new ObjectNotFoundException("No options for that id exists");

            string docType = productTypeSetting.OptionDocType;

            foreach (INode optionNode in node.ChildrenAsList)
            {
                if (optionNode.NodeTypeAlias.Equals(docType.Trim()))
                {
                    string optionName = (productTypeSetting.OptionProperty != null)
                                         ? UmbracoHelper.InsertPropertyAliases(optionNode, productTypeSetting.OptionProperty)
                                         : UmbracoHelper.InsertPropertyAliases(optionNode, _config.GetConfiguration().ProductOptionProperty);
                    string priceAlias = productTypeSetting.PriceProperty;

                    decimal price = 0;
                    decimal.TryParse(UmbracoHelper.GetPropertyAlias(optionNode, priceAlias), out price);                
                    yield return new ProductOption { Name = optionName, OptionId = optionNode.Id, OptionPrice = price };
                }
            }
        }


        public PaymentOptionOperationStatus GetPaymentOptions(int productId)
        {
            return GetPaymentOptions(productId, 0, 0, 0);
        }


        //TODO: changed to use existing paymentTotal and quantity - we need another method for use before we have a payment Total & quantity
        //i.e. for listing of options prior to add to basket
        public PaymentOptionOperationStatus GetPaymentOptions(int productId, int optionId, decimal productLineTotal,int quantity)
        {
            var operationStatus = new PaymentOptionOperationStatus();
            try
            {
                Node productNode= new Node(productId);

                var productTypeSetting = _config.GetConfiguration().ProductTypeSettings[productNode.NodeTypeAlias];

                string status = (productTypeSetting != null)
                     ? UmbracoHelper.GetPropertyAlias(productNode, productTypeSetting.StatusProperty)
                     : UmbracoHelper.GetPropertyAlias(productNode, _config.GetConfiguration().ProductStatusProperty);

                bool depositOptionDisabled = (productTypeSetting != null)
                                                ? UmbracoHelper.GetBooleanPropertyAlias(productNode,
                                                                                        productTypeSetting
                                                                                            .DepositOptionDisabledProperty, false)
                                                : UmbracoHelper.GetBooleanPropertyAlias(productNode,
                                                                                        _config.GetConfiguration()
                                                                                               .ProductDepositOptionDisabledProperty,
                                                                                        false);

                var paymentOptions = new List<PaymentOption>();

                //if fully booked
                if (status == "B")
                {
                    bool waitingList = (productTypeSetting != null) && productTypeSetting.WaitingList;
                    if (waitingList)
                    {
                        paymentOptions.Add(new PaymentOption {Code = "W", Description = "JOIN WAITING LIST", Amount = 0});
                    }
                }
                else
                {
                    bool allowProvisionalBookings = (productTypeSetting != null) &&
                                                    productTypeSetting.AllowProvisionalBookings;
                    if (allowProvisionalBookings)
                    {
                        paymentOptions.Add(new PaymentOption
                        {
                            Code = "P",
                            Description = "MAKE A PROVISIONAL BOOKING",
                            Amount = 0
                        });

                    }

                    if (!depositOptionDisabled)
                    {
                        //check whether a deposit option is allowed in this case
                        decimal depositAmount = (productTypeSetting != null)
                            ? productTypeSetting.DepositAmount
                            : _config.GetConfiguration().ProductDepositAmount;

                        decimal minAmountForDepositOption = (productTypeSetting != null)
                            ? productTypeSetting.MinAmountForDepositOption
                            : _config.GetConfiguration().ProductMinAmountForDepositOption;

                        bool depositAllowed = ((quantity == 0) ||
                                               (depositAmount*quantity < productLineTotal &&
                                                productLineTotal > minAmountForDepositOption));

                        if (depositAllowed)
                        {
                            int daysDepositLimit = (productTypeSetting != null)
                                ? productTypeSetting.DaysDepositLimit
                                : 1;

                            int daysToStartDate = 0;

                            DateTime? startDate = (productTypeSetting != null)
                                ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                    productTypeSetting.StartDateProperty)
                                : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                    _config.GetConfiguration()
                                        .ProductStartDateProperty);

                            if (startDate == null)
                                startDate = (productTypeSetting != null)
                                    ? UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                        productTypeSetting.StartDateRange1Property)
                                    : UmbracoHelper.GetDateTimePropertyAlias(productNode,
                                        _config.GetConfiguration()
                                            .ProductStartDateRange1Property);
                            if (startDate != null)
                            {
                                daysToStartDate = ((DateTime) startDate).Subtract(DateTime.Now).Days;
                            }



                            //if a the start date is outside of the deposit limit in days 
                            if (daysToStartDate > daysDepositLimit)
                            {
                                paymentOptions.Add(new PaymentOption
                                {
                                    Code = "D",
                                    Description = "PAY A DEPOSIT PER PERSON",
                                    Amount = depositAmount
                                });
                            }
                        }
                    }
                    paymentOptions.Add(new PaymentOption
                        {
                            Code = "F",
                            Description = "MAKE A FULL PAYMENT PER PERSON"
                        });
                }
                operationStatus.PaymentOptions = paymentOptions;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<PaymentOptionOperationStatus>
                    .CreateFromException("An error has occurred retrieving the product options", e);
            }
            return operationStatus;
            

        }
    }
}

