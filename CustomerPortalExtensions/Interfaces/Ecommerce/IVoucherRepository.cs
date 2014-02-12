using CustomerPortalExtensions.Domain.Ecommerce;

namespace CustomerPortalExtensions.Interfaces.Ecommerce
{
    public interface IVoucherRepository
    {
        VoucherOperationStatus GetVoucher(string voucherCode);
    }
}