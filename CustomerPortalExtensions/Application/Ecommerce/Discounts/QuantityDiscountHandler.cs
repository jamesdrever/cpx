using System;
using System.Linq;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Discounts
{
    public class QuantityDiscountHandler : IDiscountHandler
    {

        #region IDiscountHandler Members
        private readonly int _qty;
        private readonly int _perCent;

        public QuantityDiscountHandler(int qty,int perCent)
        {
            _qty = qty;
            _perCent = perCent;
        }
        public Order UpdateDiscount(Order order,Contact contact)
        {
            if (order.NumberOfItems >= _qty)
            {
                decimal discountTotal = 0;

                //loops through each item, to match functionality in current
                //Publications database
                foreach (OrderLine item in order.CurrentOrderLines)
                {

                    decimal discountOnRow =
                        Decimal.Round(
                            Decimal.Multiply((Decimal.Divide(_perCent, Convert.ToDecimal(100))),
                                             (Convert.ToDecimal(item.PaymentLineTotal))), 2);
                    order.CurrentOrderLines.First(x => x.OrderLineId == item.OrderLineId).LineDiscount = discountOnRow;
                    discountTotal = Decimal.Round(Decimal.Add(discountTotal, discountOnRow), 2);
                }
                order.DiscountTotal = discountTotal;
                order.DiscountInfo = "Since twenty or more items are ordered, a 10% discount is applied.";
            }
            else
            {
                order.DiscountTotal = 0;
                order.DiscountInfo =
                    "No discount has been applied on this order.  If you order twenty or more items, a 10% discount is applied.";
            }
            return order;
        }

        #endregion
    }
}