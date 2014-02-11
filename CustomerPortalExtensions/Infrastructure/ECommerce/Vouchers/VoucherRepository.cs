using System;
using System.Configuration;
using System.Linq;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Helper.Umbraco;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Examine;
using umbraco.NodeFactory;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Vouchers
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly IConfigurationService _config;

        public VoucherRepository(IConfigurationService config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            _config = config;
        }

        public VoucherOperationStatus GetVoucher(string voucherCode)
        {
            var operationStatus = new VoucherOperationStatus();
            try
            {
                var voucher = new Voucher();
                //get the info from Umbraco
                //var voucherNode = new Node(voucherId);



                string searchProvider=_config.GetConfiguration().VoucherSearchProvider;
                string voucherCodeProperty = _config.GetConfiguration().VoucherCodeProperty;
                
                var voucherSearcher = ExamineManager.Instance.SearchProviderCollection[searchProvider];
                var voucherSearchCriteria = voucherSearcher.CreateSearchCriteria();
                var voucherQuery = voucherSearchCriteria.Field(voucherCodeProperty, voucherCode);
                var voucherSearchResults = voucherSearcher.Search(voucherQuery.Compile());

                if (voucherSearchResults.TotalItemCount > 0)
                {
                    string voucherDocType = UmbracoHelper.GetPropertyAlias(voucherSearchResults, "nodeTypeAlias");

                    IVoucherTypeSetting voucherTypeSetting =
                        _config.GetConfiguration().VoucherTypeSettings[voucherDocType];

                    voucher.Title = (voucherTypeSetting != null)
                                        ? UmbracoHelper.GetPropertyAlias(voucherSearchResults,
                                                                         voucherTypeSetting.TitleProperty)
                                        : UmbracoHelper.GetPropertyAlias(voucherSearchResults,
                                                                         _config.GetConfiguration().VoucherTitleProperty);

                    voucher.VoucherId = (voucherTypeSetting != null)
                                            ? UmbracoHelper.GetIntegerPropertyAlias(voucherSearchResults,
                                                                                    voucherTypeSetting.IdProperty)
                                            : UmbracoHelper.GetIntegerPropertyAlias(voucherSearchResults,
                                                                                    _config.GetConfiguration()
                                                                                           .VoucherIdProperty);

                    decimal amount = 0;
                    string amountAlias = (voucherTypeSetting != null)
                                             ? voucherTypeSetting.AmountProperty
                                             : _config.GetConfiguration().VoucherAmountProperty;

                    decimal.TryParse(UmbracoHelper.GetPropertyAlias(voucherSearchResults, amountAlias), out amount);
                    voucher.Amount = amount;

                    decimal amountPerItem = 0;
                    string amountPerItemAlias = (voucherTypeSetting != null)
                                                    ? voucherTypeSetting.AmountPerItemProperty
                                                    : _config.GetConfiguration().VoucherAmountPerItemProperty;

                    decimal.TryParse(UmbracoHelper.GetPropertyAlias(voucherSearchResults, amountPerItemAlias),
                                     out amountPerItem);
                    voucher.PerItemAmount = amountPerItem;

                    voucher.Percentage = (voucherTypeSetting != null)
                                             ? UmbracoHelper.GetIntegerPropertyAlias(voucherSearchResults,
                                                                                     voucherTypeSetting
                                                                                         .PercentageProperty)
                                             : UmbracoHelper.GetIntegerPropertyAlias(voucherSearchResults,
                                                                                     _config.GetConfiguration()
                                                                                            .VoucherPercentageProperty);
                    voucher.ProductCategoryFilter = (voucherTypeSetting != null)
                                                        ? UmbracoHelper.GetPropertyAlias(voucherSearchResults,
                                                                                         voucherTypeSetting
                                                                                             .ProductCategoryFilterProperty)
                                                        : UmbracoHelper.GetPropertyAlias(voucherSearchResults,
                                                                                         _config.GetConfiguration()
                                                                                                .VoucherProductCategoryFilterProperty);

                    voucher.VoucherCategoryFilter = (voucherTypeSetting != null)
                                                        ? UmbracoHelper.GetPropertyAlias(voucherSearchResults,
                                                                                         voucherTypeSetting
                                                                                             .CategoryFilterProperty)
                                                        : UmbracoHelper.GetPropertyAlias(voucherSearchResults,
                                                                                         _config.GetConfiguration()
                                                                                                .VoucherCategoryFilterProperty);

                    decimal minimumPayment = 0;
                    string minimumPaymentAlias = (voucherTypeSetting != null)
                                                     ? voucherTypeSetting.MinimumPaymentProperty
                                                     : _config.GetConfiguration().VoucherMinimumPaymentProperty;

                    decimal.TryParse(UmbracoHelper.GetPropertyAlias(voucherSearchResults, minimumPaymentAlias),
                                     out minimumPayment);
                    voucher.MinimumPayment = minimumPayment;

                    voucher.MinimumItems = (voucherTypeSetting != null)
                                               ? UmbracoHelper.GetIntegerPropertyAlias(voucherSearchResults,
                                                                                       voucherTypeSetting
                                                                                           .MinimumItemsProperty)
                                               : UmbracoHelper.GetIntegerPropertyAlias(voucherSearchResults,
                                                                                       _config.GetConfiguration()
                                                                                              .VoucherMinimumItemsProperty);

                    voucher.OrderIndex = (voucherTypeSetting != null)
                                             ? voucherTypeSetting.OrderIndex
                                             : 1;

                    operationStatus.Voucher = voucher;
                    operationStatus.Status = true;
                }
                else
                {
                    operationStatus.Status = false;
                    operationStatus.Message = "The voucher was not recognised";
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<VoucherOperationStatus>
                    .CreateFromException("An error has occurred retrieving the voucher", e);
            }

            return operationStatus;

        }
    }
}