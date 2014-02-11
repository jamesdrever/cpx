using System;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.ECommerce
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductExternalId { get; set; }
        public string Title { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeCode { get; set; }
        public string Category { get; set; }
        public string VoucherCategory { get; set; }
        public string Code { get; set; }
        public int OptionId { get; set; }
        public string OptionTitle { get; set; }
        public string OptionExternalId { get; set; }
        public decimal? Price { get; set; }
        public decimal? EarlyBirdPrice { get; set; }
        public DateTime? EarlyBirdPriceCutOffDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? StartDateRange1 { get; set; }
        public DateTime? FinishDateRange1 { get; set; }
        public DateTime? StartDateRange2 { get; set; }
        public DateTime? FinishDateRange2 { get; set; }
        public string Location { get; set; }
        public string LocationCode { get; set; }
        public string LocationEmail { get; set; }
        public string ProductUrl { get; set; }
        public int OrderIndex { get; set; }
        /// <summary>
        /// O - open
        /// B - fully booked (course)
        /// C - cancelled (course)
        /// </summary>
        public string Status { get; set; }
        public bool EcommerceDisabled { get; set; }
        public bool DepositOptionDisabled { get; set; }
        public decimal DepositAmount { get; set; }
        public int MaximumQuantity { get; set; }
        public string BespokePricingHandler { get; set; }

        public bool HasBespokePricing()
        {
            return (!String.IsNullOrEmpty(BespokePricingHandler));
        }

        public bool HasRestrictedDateRange()
        {
            return (!(StartDateRange1 == null));
        }
        public bool HasStartAndFinishDatesWithinRestrictedRange()
        {
            if (StartDateRange2 == null)
                return (StartDate >= StartDateRange1 && FinishDate <= FinishDateRange1);
           return ((StartDate >= StartDateRange1 && FinishDate <= FinishDateRange1) || (StartDate >= StartDateRange2 && FinishDate <= FinishDateRange2));
        }

        public bool CanUseEarlyBirdPrice()
        {
            return (EarlyBirdPriceCutOffDate != null && EarlyBirdPrice != null &&
                    ((DateTime)EarlyBirdPriceCutOffDate - DateTime.Now).TotalDays >= 0 && EarlyBirdPrice!=null);
        }

        public decimal? ChargedPrice
        {
            get { return CanUseEarlyBirdPrice() ? (decimal)EarlyBirdPrice : Price;  }
        }

    }

}