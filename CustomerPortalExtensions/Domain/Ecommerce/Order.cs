using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Helper.DateTimeExtensions;
using CustomerPortalExtensions.Infrastructure.Services.Database;


namespace CustomerPortalExtensions.Domain.ECommerce
{
    //TODO: would love to lose the PetaPoco attributes
    //have resigned myself to using the ignore ones, hopefully
    //can leave it there..
    [TableName("cpxOrder")]
    [PrimaryKey("OrderId")]
    public class Order 
    {
       public Order()
       {
           Status = "PROV";
           DiscountTotal = 0;
           ShippingTotal = 0;
           VoucherAmount = 0;
           VoucherPerItemAmount = 0;
           VoucherPercentage = 0;
           VoucherId = 0;
           VoucherCategoryFilter = "";
           OrderCreated = DateTime.Now;
           OrderLines = new List<OrderLine>();
           GiftAidAgreement = false;
       }



        public int OrderId { get; set; }
        [Ignore]
        public string Description { get; set; }
        public int OrderIndex { get; set;  }
        public int ContactId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SpecialRequirements { get; set; }
        public string Status { get; set; }
        public DateTime OrderCreated { get; set;  }
        [Ignore]
        public int NumberOfItems
        {
            get { return CurrentOrderLines == null ? 0 : CurrentOrderLines.Sum(i => i.Quantity); }
        }
        [Ignore]
        public int NumberOfItemsExcludingDonations
        {
            get { return CurrentOrderLines == null ? 0 : CurrentOrderLines.Where(i=>i.PaymentType!="DON").Sum(i => i.Quantity); }
        }

        [Ignore]
        public decimal ProductSubTotal
        {
            get { return CurrentOrderLines == null ? 0 : CurrentOrderLines.Sum(i => (i.ProductLineTotal)); }
        }
        [Ignore]
        public decimal PaymentSubTotal
        {
            get { return CurrentOrderLines == null ? 0 : CurrentOrderLines.Sum(i => (i.PaymentLineTotal)); }
        }
        [Ignore]
        public decimal PaymentSubTotalExcludingDonations
        {
            get { return CurrentOrderLines == null ? 0 : CurrentOrderLines.Where(i => i.PaymentType != "DON").Sum(i => (i.PaymentLineTotal)); }
        }
        [Ignore]
        public string ShippingInfo { get; set; }
        public decimal ShippingTotal { get; set; }
        public string DiscountInfo { get; set; }
        public decimal DiscountTotal { get; set; }
        public int VoucherId { get; set; }
        public string VoucherInfo { get; set; }
        public int VoucherPercentage { get; set; }
        public decimal VoucherAmount { get; set; }
        public decimal VoucherPerItemAmount { get; set; }
        public string VoucherCategoryFilter { get; set; }
        public string VoucherProductCategoryFilter { get; set; }
        public decimal VoucherMinimumPayment { get; set; }
        public int VoucherMinimumItems { get; set; }
        //[Ignore]
        //public decimal VoucherTotal
        //{
        //    get { return GetVoucherTotal(); }
        //}

        public decimal GetVoucherTotal()
        {
            if (VoucherId == 0 || VoucherMinimumPayment > PaymentSubTotalExcludingDonations || VoucherMinimumItems > NumberOfItemsExcludingDonations)
                return 0;
            if (VoucherAmount > 0)
                return VoucherAmount;
            if (VoucherPercentage > 0)
            {
                if (!String.IsNullOrEmpty(VoucherCategoryFilter))
                {
                    var selectedOrderLines =
                        CurrentOrderLines.Where(i => i.ProductVoucherCategory.StartsWith(VoucherCategoryFilter));
                    return (selectedOrderLines == null)
                               ? 0
                               : ((decimal)VoucherPercentage / 100) * selectedOrderLines.Sum(i => i.PaymentLineTotal);
                }
                if (!String.IsNullOrEmpty(VoucherProductCategoryFilter))
                {
                    var selectedOrderLines =
                        CurrentOrderLines.Where(i => i.ProductCategory.StartsWith(VoucherProductCategoryFilter));
                    return (selectedOrderLines == null)
                               ? 0
                               : ((decimal)VoucherPercentage / 100) * selectedOrderLines.Sum(i => i.PaymentLineTotal);
                }
                return ((decimal) VoucherPercentage/100)*PaymentSubTotalExcludingDonations;
            }
            if (VoucherPerItemAmount > 0)
            {
                if (!String.IsNullOrEmpty(VoucherCategoryFilter))
                {
                    var selectedOrderLines =
                        CurrentOrderLines.Where(i => i.ProductVoucherCategory.StartsWith(VoucherCategoryFilter));
                    return (selectedOrderLines == null)
                               ? 0
                               : selectedOrderLines.Sum(i => i.Quantity) * VoucherPerItemAmount;
                }
                if (!String.IsNullOrEmpty(VoucherProductCategoryFilter))
                {
                    var selectedOrderLines =
                        CurrentOrderLines.Where(i => i.ProductCategory.StartsWith(VoucherProductCategoryFilter));
                    return (selectedOrderLines == null)
                               ? 0
                               : selectedOrderLines.Sum(i => i.Quantity) * VoucherPerItemAmount;
                }
                return NumberOfItemsExcludingDonations * VoucherPerItemAmount;
                
                
            }
            return 0;
        }

        [Ignore]
        public decimal PaymentSubTotalIncludingDiscountAndVoucher
        {
            get { return PaymentSubTotal - DiscountTotal-GetVoucherTotal(); }
        }
        //TODO: figure how to update this field in the databse, but not select it (PetaPoco falls over with an error due to the lack of set accessor)
        [Ignore]
        public decimal PaymentTotal
        {
            get { return PaymentSubTotalIncludingDiscountAndVoucher + ShippingTotal; }
        }
        public bool GiftAidAgreement { get; set; }
        /// <summary>
        /// NOTE: Includes deleted order lines!  Use CurrentOrderLines whenever
        /// displaying the order or calculating totals (excludes deleted order lines)
        /// </summary>
        [Ignore]
        public IList<OrderLine> OrderLines { get; set; }
        /// <summary>
        /// use whenever displaying the order or calculating totals (excludes deleted order lines)
        /// </summary>
        [Ignore]
        public IList<OrderLine> CurrentOrderLines
        {
            get
            {
                return OrderLines.Where(x => x.OrderStatus != "DEL").ToList();
        } }

        public OrderOperationStatus Add(Product product, int quantity,string paymentType,decimal paymentAmount)
        {
            var orderOperationStatus = new OrderOperationStatus();
            if (ContainsProduct(product)&&product.ProductType!="D")
            {
                return Update(product, product, quantity, paymentType, paymentAmount);
            }

            if (product.ChargedPrice == null)
            {
                orderOperationStatus.Message = "The item cannot be added because no price has been specified";
                orderOperationStatus.MessageCode = "CPX.NoPriceSpecified";
                orderOperationStatus.Status = false;
                return orderOperationStatus;
            }

            var orderLine = new OrderLine
            {
                OrderId = OrderId,
                ProductId = product.ProductId,
                ProductExternalId = product.ProductExternalId,
                ProductPrice = (decimal)product.ChargedPrice,
                ProductTitle = product.Title,
                ProductOptionId = product.OptionId,
                ProductOptionTitle = product.OptionTitle,
                ProductCategory = product.Category,
                ProductVoucherCategory = product.VoucherCategory,
                ProductCode = product.Code,
                ProductOptionExternalId = product.OptionExternalId,
                ProductType = product.ProductType,
                ProductUrl = product.ProductUrl,
                PaymentType = paymentType,
                PaymentAmount = paymentAmount,
                Location=product.Location,
                LocationCode=product.LocationCode,
                LocationEmail=product.LocationEmail,
                StartDate = product.StartDate,
                FinishDate=product.FinishDate,
                Quantity = quantity,
                OrderStatus="PROV",
                PaymentStatus="N"
            };
            OrderLines.Add(orderLine);

            orderOperationStatus.Status = true;
            orderOperationStatus.Order = this;
            orderOperationStatus.OrderLine = orderLine;
            return orderOperationStatus;
        }

        public OrderOperationStatus Remove(Product product)
        {
            var orderOperationStatus = new OrderOperationStatus();
            OrderLine orderLine = GetOrderLine(product);
            orderLine.OrderStatus = "DEL";
            orderOperationStatus.Order = this;
            orderOperationStatus.Status = true;
            orderOperationStatus.OrderLine = orderLine;
            orderOperationStatus.OrderLineDeleted = true;
            return orderOperationStatus;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldProduct"></param>
        /// <param name="newProduct"></param>
        /// <param name="quantity"></param>
        /// <param name="paymentType"></param>
        /// <param name="paymentAmount"></param>
        /// <returns>if product to be updated doesn't exist in order, </returns>
        public OrderOperationStatus Update(Product oldProduct, Product newProduct, int quantity, string paymentType,
                                decimal paymentAmount)
        {
            var orderOperationStatus = new OrderOperationStatus();
            // if the quantity is zero, do a remove instead
            if (quantity == 0)
                return Remove(oldProduct);

            if (newProduct.ChargedPrice == null)
            {
                orderOperationStatus.Message = "The item cannot be added because no price has been specified";
                orderOperationStatus.MessageCode = "CPX.NoPriceSpecified";
                orderOperationStatus.Status = false;
                return orderOperationStatus;
            }


            if (!ContainsProduct(oldProduct))
            {
                orderOperationStatus.Message = "The item cannot be updated because it does not exist in the order";
                orderOperationStatus.MessageCode = "CPX.ItemNotInOrder";
                orderOperationStatus.Status = false;
                return orderOperationStatus;
            }

            //check that the product to be udated to doesn't exist
            if (newProduct.OptionId > 0 && oldProduct.OptionId != newProduct.OptionId &&
                                    ContainsProduct(newProduct))
            {
                orderOperationStatus.Message = "There is already an item of this type in the order";
                orderOperationStatus.MessageCode = "CPX.ItemAlreadyInOrder";
                orderOperationStatus.Status = false;
                return orderOperationStatus;
            }

            if (paymentAmount > newProduct.ChargedPrice)
            {
                orderOperationStatus.Message = "The item cannot be updated because it would mean an over-payment";
                orderOperationStatus.MessageCode = "CPX.OverPayment";
                orderOperationStatus.Status = false;
                return orderOperationStatus;
                
            }


            //get the order line to update
            OrderLine orderLine = GetOrderLine(oldProduct);
            //merge in changes from the update
            orderLine.ProductPrice = (decimal)newProduct.ChargedPrice;
            orderLine.ProductOptionTitle = newProduct.OptionTitle;
            orderLine.ProductOptionId = newProduct.OptionId;
            orderLine.ProductOptionExternalId = newProduct.OptionExternalId;

            orderLine.Quantity = quantity;
            orderLine.PaymentType = paymentType;
            orderLine.PaymentAmount = paymentAmount;

            orderOperationStatus.Status = true;
            orderOperationStatus.Order = this;
            orderOperationStatus.OrderLine = orderLine;
            return orderOperationStatus;
        }

        public bool ContainsProduct(Product product)
        {
            if (product.OptionId==0)
            {
                return CurrentOrderLines.Any(x => x.ProductId == product.ProductId);
            }
            return CurrentOrderLines.Any(x => x.ProductId == product.ProductId && x.ProductOptionId == product.OptionId);

        }

        public int GetQuantityOfProduct(Product product)
        {
            if (product.OptionId == 0)
            {
                return CurrentOrderLines.Where(x => x.ProductId == product.ProductId).Sum(x => x.Quantity);
            }
            else
            {
                return CurrentOrderLines.Where(x => x.ProductId == product.ProductId && x.ProductOptionId == product.OptionId).Sum(x => x.Quantity);
            } 
        }

        public bool ContainsProductType(string productType)
        {
            return CurrentOrderLines.Any(x => x.ProductType == productType);
        }

        public bool ContainsCourses()
        {
            return CurrentOrderLines.Any(x => x.ProductType == "C");
        }

        public bool ContainsMemberships()
        {
            return CurrentOrderLines.Any(x => x.ProductType == "M");
        }

        public bool ContainsDonations()
        {
            return CurrentOrderLines.Any(x => x.ProductType == "D");
        }

        public bool ContainsProducts()
        {
            return CurrentOrderLines.Any(x => x.ProductType == "P");
        }

        public bool ContainsGiftAidableProducts()
        {
            return CurrentOrderLines.Any(x => x.ProductType == "D" || x.ProductType == "M");
        }

        public List<string> GetLocationEmails()
        {
            return CurrentOrderLines.Select(x => x.LocationEmail).Distinct().ToList();
        }

        public List<OrderLine> GetCourseOrderLines()
        {
            return CurrentOrderLines.Where(x => x.ProductType == "C").ToList();
        }

        public List<OrderLine> GetMembershipOrderLines()
        {
            return CurrentOrderLines.Where(x => x.ProductType == "M").ToList();
        }

        public List<OrderLine> GetDonationOrderLines()
        {
            return CurrentOrderLines.Where(x => x.ProductType == "D").ToList();
        }

        public List<OrderLine> GetProductOrderLines()
        {
            return CurrentOrderLines.Where(x => x.ProductType == "P").ToList();
        }


        public OrderLine GetOrderLine(Product product)
        {
            if (ContainsProduct(product))
            {
                if (product.OptionId == 0) 
                {
                    return CurrentOrderLines.FirstOrDefault(x => x.ProductId == product.ProductId);
                }
                return CurrentOrderLines.FirstOrDefault(x => x.ProductId == product.ProductId && x.ProductOptionId == product.OptionId);
            }
                
            else
            {
                throw new DataException("The product does not exist in the order, and therefore cannot be updated.");
            }
        }

        public void UpdateOrderLines(string orderStatus)
        {
            for (int i=0;i<CurrentOrderLines.Count;i++)
            {
                CurrentOrderLines[i].OrderStatus = orderStatus;
            }
        }


        public OrderLine GetOrderLine(int orderLineId)
        {
            return CurrentOrderLines.FirstOrDefault(x => x.OrderLineId == orderLineId);
        }




        [Ignore]
        public string CheckoutPage { get; set; }
        [Ignore]
        public string ContactDetailsPage { get; set; }
        [Ignore]
        public string OrderPage { get; set; }
        [Ignore]
        public Contact ContactDetails { get; set; }
        [Ignore]
        public string SpecialRequirementsText { get; set; }
        [Ignore]
        public string PaymentGatewayForm { get; set; }
        [Ignore]
        public string PaymentGatewayAccount { get; set; }
        [Ignore]
        public string PaymentGatewayCheckCode { get; set; }
        [Ignore]
        public string PaymentGatewayCallbackUrl { get; set; }
        [Ignore]
        public string PaymentGatewayCompletionPage { get; set; }
        [Ignore]
        public DateTime? LocationApproval { get; set; }
        [Ignore]
        public string AdditionalQueueProcessingHandler { get; set; }
    }
    [TableName("cpxOrderLines")]
    [PrimaryKey("OrderLineId")]
    public class OrderLine
    {
        public int OrderId { get; set;  }
        public int OrderLineId { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        [Ignore]
        public string ProductDescription
        {
            get
            {
                return (ProductType == "C")
                           ? StartDate.ToStringOrDefault("dddd dd MMMM", "n/a") + " to " +
                             FinishDate.ToStringOrDefault("dddd dd MMMM", "n/a") + " at " + Location
                           : "";
            }
        }

        public string ProductType { get; set; }
        public int ProductOptionId { get; set; } 
        public string ProductOptionTitle { get; set; }
        [Ignore]
        public List<ProductOption> ProductOptions { get; set; }
        [Ignore]
        public List<PaymentOption> PaymentOptions { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategory { get; set; }
        public string ProductVoucherCategory { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Location { get; set; }
        public string LocationCode { get; set; }
        public string LocationEmail { get; set;  }
        public int Quantity { get; set; }
        public string ProductExternalId { get; set; }
        public string ProductOptionExternalId { get; set; }
        public string ProductUrl { get; set; }
        public decimal LineDiscount { get; set;  }
        public string PaymentType { get; set; }

        [Ignore]
        public string PaymentTypeDescription
        {
            get
            {
                switch (PaymentType)
                {
                    case "F":
                        return "Full Payment";
                    case "D":
                        return "Deposit Payment";
                    case "P":
                        return "Provisional Booking";
                    case "W":
                        return "Add to Waiting List";
                    case "DON":
                        return "Donation";
                    default:
                        return "Unknown payment type";
                }

            }
        }

        public decimal PaymentAmount { get; set;  }
        [Ignore]
        public decimal ProductLineTotal
        {
            get { return Quantity*ProductPrice; }
        }
        [Ignore]
        public decimal PaymentLineTotal
        {
            get { return Quantity*PaymentAmount; }
        }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
    public class ProductOption
    {
        public int OptionId { get; set; }
        public string Name { get; set; }
        public decimal OptionPrice { get; set; }
    }

    public class PaymentOption
    {
        /// <summary>
        /// P = provisional
        /// D = deposit
        /// F = full payment
        /// W = waiting list
        /// DON - donation
        /// </summary>
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderResultSummary
    {
        public int OrderId { get; set;  }
        public OrderLine OrderLine;
        public int NumberOfItems { get; set; }
        public string ProductSubTotal { get; set; }
        public string PaymentSubTotal { get; set; }
        public string DiscountInfo { get; set; }
        public string DiscountTotal { get; set; }
        public string VoucherInfo { get; set; }
        public string VoucherTotal { get; set; }
        public string PaymentSubTotalIncludingDiscountAndVoucher { get; set; }
        public string ShippingInfo { get; set; }
        public string ShippingTotal { get; set; }
        public string PaymentTotal { get; set; }
        public string PaymentLineTotal;
        public bool Status { get; set; }
        public string Message;
        public bool ContainsGiftAidableProducts { get; set; }
        
    }
    public class OrderQueueItem 
    {
        public int OrderId { get; set; }
        public DateTime OrderCreated { get; set; }
        public int OrderLineId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription
        {
            get
            {
                return (ProductType == "C")
                           ? StartDate.ToStringOrDefault("dddd dd MMMM", "n/a") + " to " +
                             FinishDate.ToStringOrDefault("dddd dd MMMM", "n/a") + " at " + Location +
                             (String.IsNullOrEmpty(ProductExternalId) ? "" : " Course Number: " + ProductExternalId) +
                             (String.IsNullOrEmpty(ProductOptionTitle) ? "": " Option : " + ProductOptionTitle ) + 
                             (String.IsNullOrEmpty(ProductOptionExternalId) ? "": " Option Number: " + ProductOptionExternalId)
                           : "";
            }
        }
        public string ProductExternalId { get; set; }
        public string ProductOptionTitle { get; set; }
        public string ProductOptionExternalId { get; set; }
        public string ProductType { get; set; }
        public string PaymentType { get; set; }
        [Ignore]
        public string PaymentTypeDescription
        {
            get
            {
                switch (PaymentType)
                {
                    case "F":
                        return "Full Payment";
                    case "D":
                        return "Deposit Payment";
                    case "P":
                        return "Provisional Booking";
                    case "W":
                        return "Waiting List";
                    case "DON":
                        return "Donation";
                    default:
                        return "Unknown payment type";
                }

            }
        }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int ContactId { get; set; }
        public int ExternalContactNumber { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string Email { get; set;  }
        public bool GiftAidAgreement { get; set; }

    }

    public class OrderStatus
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

}

