using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.Ecommerce
{
    public class VoucherOperationStatus : OperationStatus
    {
        public Voucher Voucher { get; set; }
    }
}