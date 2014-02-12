using System;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Shipping
{
    public class QuantityAndLocationShippingHandler : IShippingHandler
    {

        #region IShippingHandler Members

        public Order UpdateShipping(Order order, Contact contact)
        {
            if (order.NumberOfItems == 0)
            {
                order.ShippingTotal = 0;
                return order;
            }

            //default country to UK if not specified
            string country = (String.IsNullOrEmpty(contact.Country)) ? "United Kingdom" : contact.Country;

            decimal total = (decimal) order.PaymentSubTotalIncludingDiscountAndVoucher;
            order.ShippingInfo = "Shipping based on delivery to " + country;
            if (country == "United Kingdom - Mainland" || country == "United Kingdom" || country == "UK")
            {
                if (total > 0 && total <= (decimal) 6.50)
                {
                    order.ShippingTotal = 1;
                    return order;
                }
                if (total <= 15 && total > 6)
                {
                    order.ShippingTotal = 2;
                    return order;
                }
                if (total <= 30 && total > 15)
                {
                    order.ShippingTotal = 3;
                    return order;
                }
                if (total <= 100 && total > 30)
                {
                    order.ShippingTotal = 5;
                    return order;
                }
                if (total <= 150 && total > 100)
                {
                    order.ShippingTotal = 8;
                    return order;
                }
                order.ShippingTotal = 0;
                return order;
            }
            /*
            Offshore post 
            Upto £6.50 - £1 
            £6.51to £15 - £2 
            £15.01-30 - £3 
            £30.01-£100 - £5 
            £100.01-£150 - £12 
            £150.05 and over - £15
            */
            if (country == "United Kingdom - Islands" || country == "UKI")
            {

                if (total > 0 && total <= (decimal) 6.50)
                {
                    order.ShippingTotal = 1;
                    return order;
                }
                if (total <= 15 && total > 6)
                {
                    order.ShippingTotal = 2;
                    return order;
                }
                if (total <= 30 && total > 15)
                {
                    order.ShippingTotal = 3;
                    return order;
                }
                if (total <= 100 && total > 30)
                {
                    order.ShippingTotal = 5;
                    return order;
                }
                if (total <= 150 && total > 100)
                {
                    order.ShippingTotal = 12;
                    return order;
                }
                order.ShippingTotal = 15;
                return order;
            }
            if (total > 0 && total <= 15)
            {
                order.ShippingTotal = 4;
                return order;
            }
            if (total <= 30 && total > 15)
            {
                order.ShippingTotal = 7;
                return order;
            }
            if (total <= 75 && total > 30)
            {
                order.ShippingTotal = 15;
                return order;
            }
            if (total <= 150 && total > 75)
            {
                order.ShippingTotal = 20;
                return order;
            }
            order.ShippingTotal = 25;
            return order;

        }

        #endregion
    }
}