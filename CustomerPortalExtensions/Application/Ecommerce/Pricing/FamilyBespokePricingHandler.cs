using System;
using System.IO;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Pricing
{
    public class FamilyBespokePricingHandler : IBespokePricingHandler
    {
        public Product CreateBespokePrice(Product product, int quantity)
        {
            if (product.StartDate == null || product.FinishDate == null)
            {

                throw new InvalidDataException("Cannot create pricing: start and finish date must not be null"); 
            }
            TimeSpan spanOfDays = (DateTime)product.FinishDate - (DateTime)product.StartDate;
            double numberOfNights = spanOfDays.TotalDays;

            int numberOfCostedNights = Convert.ToInt32(numberOfNights - (Math.Floor((numberOfNights/4))));
            product.Price = product.Price*numberOfCostedNights;
            if (product.CanUseEarlyBirdPrice())
                product.EarlyBirdPrice = (decimal)
                                         product.EarlyBirdPrice*numberOfCostedNights;
               
            return product;
        }
    }
}