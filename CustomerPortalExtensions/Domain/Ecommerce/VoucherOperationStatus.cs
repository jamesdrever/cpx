using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.Ecommerce
{
    public class VoucherOperationStatus : OperationStatus
    {
        public Voucher Voucher { get; set; }
    }
}