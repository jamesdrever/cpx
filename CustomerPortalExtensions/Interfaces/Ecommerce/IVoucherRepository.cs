using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Ecommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IVoucherRepository
    {
        VoucherOperationStatus GetVoucher(string voucherCode);
    }
}