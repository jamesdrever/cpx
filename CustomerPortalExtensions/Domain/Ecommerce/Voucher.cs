using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerPortalExtensions.Domain.Ecommerce
{
    public class Voucher
    {
        public int VoucherId { get; set; }
        public string Title { get; set; }
        public int Percentage { get; set; }
        public decimal Amount { get; set; }
        public decimal PerItemAmount { get; set; }
        public string ProductCategoryFilter { get; set; }
        public string VoucherCategoryFilter { get; set; }
        public decimal MinimumPayment { get; set; }
        public int MinimumItems { get; set; }
        public int OrderIndex { get; set; }

    }
}