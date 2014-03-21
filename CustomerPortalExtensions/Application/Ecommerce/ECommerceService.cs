using System;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Omu.ValueInjecter;

namespace CustomerPortalExtensions.Application.Ecommerce
{
    public class EcommerceService : IEcommerceService
    {

        private readonly IOrderCoordinator _orderCoordinator;
        private readonly IProductRepository _productRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IContactService _contactService;

        private readonly IBespokePricingHandlerFactory _bespokePricingHandlerFactory;


        public EcommerceService(IOrderCoordinator orderCoordinator, IProductRepository productRepository,
            IVoucherRepository voucherRepository, IContactService contactService,
            IBespokePricingHandlerFactory bespokePricingHandlerFactory)
        {
            if (orderCoordinator == null)
            {
                throw new ArgumentNullException("orderCoordinator");
            }
            _orderCoordinator = orderCoordinator;
            if (productRepository == null)
            {
                throw new ArgumentNullException("productRepository");
            }
            _productRepository = productRepository;
            if (voucherRepository == null)
            {
                throw new ArgumentNullException("voucherRepository");
            }
            _voucherRepository = voucherRepository;
            if (contactService == null) throw new ArgumentNullException("contactService");
            _contactService = contactService;

            if (bespokePricingHandlerFactory == null)
            {
                throw new ArgumentNullException("bespokePricingHandlerFactory");
            }
            _bespokePricingHandlerFactory = bespokePricingHandlerFactory;
        }

        public OrderOperationStatus GetOrder()
        {
            return GetOrder(1);
        }

        public OrderOperationStatus GetOrder(int orderIndex)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {

                var contactOperationStatus = _contactService.GetContact();
                if (!contactOperationStatus.Status)
                    return (OrderOperationStatus) orderOperationStatus.InjectFrom(contactOperationStatus);

                var contact = contactOperationStatus.Contact;

                orderOperationStatus = _orderCoordinator.GetOrder(contactOperationStatus.Contact, orderIndex);

                if (orderOperationStatus.Status)
                {
                    var order = orderOperationStatus.Order;
                    order.ContactDetails = contact;
                    order.UserName = contact.UserName;
                    //update the shipping and discounts
                    order = _orderCoordinator.UpdateOrderWithShippingAndDiscounts(order, contact);
                    orderOperationStatus = _orderCoordinator.SaveOrder(order, false);
                    if (orderOperationStatus.Status)
                    {
                        //add the product options to the order
                        foreach (var orderLine in order.CurrentOrderLines)
                        {
                            var productOperationStatus = _productRepository.GetProductOptions(orderLine.ProductId);
                            if (productOperationStatus.Status)
                                orderLine.ProductOptions = productOperationStatus.ProductOptions;
                            var paymentOperationStatus = _productRepository.GetPaymentOptions(orderLine.ProductId,
                                orderLine.ProductOptionId, orderLine.ProductLineTotal, orderLine.Quantity);
                            if (paymentOperationStatus.Status)
                                orderLine.PaymentOptions = paymentOperationStatus.PaymentOptions;
                        }

                    }
                    orderOperationStatus.Order = order;
                }
            }
            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e, "An error has occurred retrieving the order");
            }
            return orderOperationStatus;
        }

        public OrderSummaryOperationStatus GetOrderSummaryById(int orderId)
        {
            var orderSummaryOperationStatus = new OrderSummaryOperationStatus();
            try
            {
                orderSummaryOperationStatus = _orderCoordinator.GetOrderSummary(orderId);
            }
            catch (Exception e)
            {
                orderSummaryOperationStatus.LogFailedOperation(e, "An error has occurred retrieving the order");
            }
            return orderSummaryOperationStatus;
        }



        /// <summary>
        /// make a full payment for the product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public OrderOperationStatus AddProductToOrder(int productId, int quantity)
        {
            return AddProductToOrder(productId, 0, quantity, null, null, "F", null);
        }

        public OrderOperationStatus AddProductToOrder(int productId, int quantity, string paymentType)
        {
            return AddProductToOrder(productId, 0, quantity, null, null, paymentType, null);
        }


        public OrderOperationStatus AddProductToOrder(int productId, int optionId, int quantity, string paymentType)
        {
            return AddProductToOrder(productId, optionId, quantity, null, null, paymentType, null);
        }

        public OrderOperationStatus AddCourseWithFlexibleDatesToOrder(int productId, int optionId, DateTime startDate,
            DateTime finishDate, int quantity, string paymentType)
        {
            return AddProductToOrder(productId, optionId, quantity, startDate, finishDate, paymentType, null);
        }


        public OrderOperationStatus AddDonationToOrder(int productId)
        {
            return AddProductToOrder(productId, 0, 1, null, null, "DON", null);
        }


        public OrderOperationStatus AddFlexibleDonationToOrder(int productId, decimal paymentAmount)
        {
            return AddProductToOrder(productId, 0, 1, null, null, "DON", paymentAmount);
        }

        private OrderOperationStatus AddProductToOrder(int productId, int optionId, int quantity, DateTime? startDate,
            DateTime? finishDate, string paymentType,
            decimal? paymentAmount)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                //get the product
                var productOperationStatus = GetProduct(productId, optionId, quantity, startDate, finishDate,
                    paymentType, paymentAmount);
                var product = productOperationStatus.Product;
                if (!productOperationStatus.Status)
                    return (OrderOperationStatus) orderOperationStatus.InjectFrom(productOperationStatus);
                if (paymentAmount == null) paymentAmount = GetPaymentAmount(product, paymentType);
                orderOperationStatus = AddProductToOrder(product, quantity, paymentType, (decimal) paymentAmount);
            }
            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e, "An error has occurred adding the product to the order");
            }
            return orderOperationStatus;
        }

        private OrderOperationStatus AddProductToOrder(Product product, int quantity, string paymentType,
            decimal paymentAmount)
        {

            var orderOperationStatus = new OrderOperationStatus();
            //get the contact
            try
            {
                var contactOperationStatus = _contactService.GetContact();
                if (!contactOperationStatus.Status)
                    return (OrderOperationStatus) new OrderOperationStatus().InjectFrom(contactOperationStatus);

                var contact = contactOperationStatus.Contact;

                //get the current order
                orderOperationStatus = _orderCoordinator.GetOrder(contactOperationStatus.Contact, product.OrderIndex);
                if (orderOperationStatus.Status)
                {
                    var order = orderOperationStatus.Order;
                    //check the quantity
                    if (product.MaximumQuantity != 0 &&
                        quantity + order.GetQuantityOfProduct(product) > product.MaximumQuantity)
                        return new OrderOperationStatus
                        {
                            Message = "No more items of this type can be added!",
                            Status = false
                        };
                    //add the order line
                    orderOperationStatus = order.Add(product, quantity, paymentType, (decimal) paymentAmount);
                    var orderLine = orderOperationStatus.OrderLine;
                    if (orderOperationStatus.Status)
                    {
                        order = _orderCoordinator.UpdateOrderWithShippingAndDiscounts(order, contact);
                        orderOperationStatus = _orderCoordinator.SaveOrder(order, true);
                        if (orderOperationStatus.Status)
                        {
                            orderOperationStatus.OrderLine = orderLine;
                            orderOperationStatus.Message = "Added " + product.Title + " successfully!";
                        }
                    }
                }

            }

            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e, "An error has occurred adding the product to the order");
            }
            return orderOperationStatus;
        }

        public ProductOperationStatus GetProduct(int productId)
        {
            return GetProduct(productId, 0, 1, null, null, "F",
                null);
        }

        public ProductOperationStatus GetProduct(int productId, int optionId)
        {
            return GetProduct(productId, optionId, 1, null, null, "F",
                null);
        }

        public ProductOperationStatus GetProduct(int productId, int optionId, int quantity, DateTime? startDate,
DateTime? finishDate)
        {
            return GetProduct(productId, optionId, quantity, startDate, finishDate, "F",
                null);
        }

        private ProductOperationStatus GetProduct(int productId, int optionId, int quantity, DateTime? startDate, DateTime? finishDate, string paymentType,
                                                       decimal? paymentAmount)
        {

            //get the product
            var productOperationStatus = _productRepository.GetProduct(productId, optionId);
            if (productOperationStatus.Status)
            {

                Product product = productOperationStatus.Product;

                if (startDate != null)
                    product.StartDate = startDate;
                if (finishDate != null)
                    product.FinishDate = finishDate;

                if (product.HasBespokePricing())
                {
                    product = GetBespokePricing(product, quantity);
                }

                if (product.HasRestrictedDateRange())
                {
                    if (!product.HasStartAndFinishDatesWithinRestrictedRange())
                        return new ProductOperationStatus
                        {
                            Message = "The specified start and finish dates are not within the allowed range",
                            MessageCode = "CPX.DatesOutOfRange",
                            Status = false
                        };
                }

                if (product.Price == 0 && paymentType == "DON") product.Price = (decimal)paymentAmount;
                productOperationStatus.Product = product;
            }
            return productOperationStatus;
        }

        public ProductOptionOperationStatus GetProductOptions(int productId)
        {
            return _productRepository.GetProductOptions(productId);
        }

        public PaymentOptionOperationStatus GetPaymentOptions(int productId)
        {
            return _productRepository.GetPaymentOptions(productId);
        }

        public ProductSummaryOperationStatus GetProductSummary(int productId)
        {
            var productSummaryOperationStatus = new ProductSummaryOperationStatus();
            try
            {
                var productOperationStatus = GetProduct(productId, 0, 0, null, null);
                productSummaryOperationStatus.Status = productOperationStatus.Status;
                if (productSummaryOperationStatus.Status)
                {
                    var product = productOperationStatus.Product;
                    productSummaryOperationStatus.Product = product;
                    productSummaryOperationStatus.Status = true;
                    productSummaryOperationStatus.OrderPage = _orderCoordinator.GetOrderPage(product.OrderIndex);
                }

            }
            catch (Exception e)
            {
                productSummaryOperationStatus.LogFailedOperation(e, "An error has occurred getting the product summary");
            }
            return productSummaryOperationStatus;
        }

        




        private Product GetBespokePricing(Product product, int quantity)
        {

            IBespokePricingHandler pricingHandler =
                _bespokePricingHandlerFactory.GetBespokePricingHandler(product.BespokePricingHandler);
            return pricingHandler.CreateBespokePrice(product, quantity);
        }

        public OrderOperationStatus Remove(int productId, int optionId)
        {

            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                //get the product
                var productOperationStatus = _productRepository.GetProduct(productId, optionId);
                if (!productOperationStatus.Status)
                    return (OrderOperationStatus) orderOperationStatus.InjectFrom(productOperationStatus);

                Product product = productOperationStatus.Product;

                //get the contact
                var contactOperationStatus = _contactService.GetContact();
                if (!contactOperationStatus.Status)
                    return (OrderOperationStatus) new OrderOperationStatus().InjectFrom(contactOperationStatus);
                var contact = contactOperationStatus.Contact;

                //get the order
                orderOperationStatus = _orderCoordinator.GetOrder(contact,product.OrderIndex);
                var order = orderOperationStatus.Order;
                orderOperationStatus=order.Remove(product);
                var orderLine = orderOperationStatus.OrderLine;
                if (orderOperationStatus.Status)
                {
                    order = _orderCoordinator.UpdateOrderWithShippingAndDiscounts(order, contact);
                    orderOperationStatus = _orderCoordinator.SaveOrder(order,true);
                    orderOperationStatus.OrderLine = orderLine;
                    orderOperationStatus.OrderLineDeleted = true;
                    orderOperationStatus.Message = "Removed " + product.Title + " successfully!";
                }
            }
            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e,"An error has occurred saving the order");
            }
            return orderOperationStatus;

        }
        public OrderOperationStatus Update(int productId,int optionId, int quantity)
        {
            return Update(productId, 0, 0,quantity, "F");
        }

        public OrderOperationStatus Update(int productId, int optionId, int newOptionId, int quantity, 
                                           string paymentType)
        {
            
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                //get the product
                var productOperationStatus = _productRepository.GetProduct(productId, optionId);

                Product oldProduct = productOperationStatus.Product;
                productOperationStatus = _productRepository.GetProduct(productId, newOptionId);

                if (!productOperationStatus.Status)
                    return (OrderOperationStatus) new OrderOperationStatus().InjectFrom(productOperationStatus);

                Product newProduct = productOperationStatus.Product;

                if (quantity > newProduct.MaximumQuantity && newProduct.MaximumQuantity != 0)
                    return new OrderOperationStatus { Message = "No more items of this type can be added!", Status = false };



                //get the contact
                var contactOperationStatus = _contactService.GetContact();
                if (!contactOperationStatus.Status)
                    return (OrderOperationStatus) new OrderOperationStatus().InjectFrom(contactOperationStatus);
                var contact = contactOperationStatus.Contact;


                //get the order
                orderOperationStatus = _orderCoordinator.GetOrder(contact, oldProduct.OrderIndex);
                var order = orderOperationStatus.Order;

                if (newProduct.HasBespokePricing())
                {
                    var oldOrderLine = order.GetOrderLine(oldProduct);
                    newProduct.StartDate = oldOrderLine.StartDate;
                    newProduct.FinishDate = oldOrderLine.FinishDate;
                    newProduct = GetBespokePricing(newProduct, quantity);
                }

                decimal paymentAmount = GetPaymentAmount(newProduct, paymentType);
                
                orderOperationStatus=order.Update(oldProduct, newProduct, quantity, paymentType, paymentAmount);
                var orderLine = orderOperationStatus.OrderLine;

                if (orderOperationStatus.Status)
                {
                    order = _orderCoordinator.UpdateOrderWithShippingAndDiscounts(order, contact);
                    orderOperationStatus = _orderCoordinator.SaveOrder(order,true);
                    orderOperationStatus.OrderLine = orderLine;
                    orderOperationStatus.Message = "Updated " + oldProduct.Title + " successfully!";
                }
            }
            catch (Exception e)
            {
               orderOperationStatus.LogFailedOperation(e,"An error has occurred saving the order");
            }
            return orderOperationStatus;

        }


        private decimal GetPaymentAmount(Product product, string paymentType)
        {
            decimal paymentAmount = 0;
            if (paymentType == "D") { paymentAmount = product.DepositAmount; }
            if (paymentType == "F"||paymentType=="DON") { paymentAmount = (decimal) product.ChargedPrice; }
            return paymentAmount;
        }

        public OrderOperationStatus UpdateSpecialRequirements(string specialRequirements)
        {
            return UpdateSpecialRequirements(specialRequirements, 1);
        }

        public OrderOperationStatus UpdateSpecialRequirements(string specialRequirements, int orderIndex)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                var contactOperationStatus = _contactService.GetContact();
                if (!contactOperationStatus.Status)
                {
                    orderOperationStatus =
                        (OrderOperationStatus) contactOperationStatus.InjectFrom(contactOperationStatus);
                }
                else
                {
                    orderOperationStatus = _orderCoordinator.GetOrder(contactOperationStatus.Contact, orderIndex);
                    if (orderOperationStatus.Status)
                    {
                        var order = orderOperationStatus.Order;
                        order.SpecialRequirements = specialRequirements;
                        orderOperationStatus = _orderCoordinator.SaveOrder(order, false);
                        if (orderOperationStatus.Status)
                            orderOperationStatus.Message = "Special requirements updated successfully!";
                    }
                }
            }
            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e, "An error has occurred adding the voucher");
            }
            return orderOperationStatus;
     

            //var contactOperationStatus = _contactService.GetContact();
            //if (contactOperationStatus.Status)
            //{
            //    return _orderRepository.UpdateSpecialRequirements(specialRequirements, orderIndex, contactOperationStatus.Contact);
            //}
            //return (OrderOperationStatus)new OrderOperationStatus().InjectFrom(contactOperationStatus);

        }

        public OrderOperationStatus UpdateGiftAidAgreement(bool agreementStatus, int orderIndex)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                var contactOperationStatus = _contactService.GetContact();
                if (contactOperationStatus.Status)
                {
                    //orderOperationStatus =
                    //    (OrderOperationStatus) orderOperationStatus.InjectFrom(contactOperationStatus);
                    //}
                    //else
                    //{
                    var contact = contactOperationStatus.Contact;
                    if (contact.ContactId > 0)
                    {
                        contact.GiftAidAgreement = agreementStatus;
                        contactOperationStatus = _contactService.SaveContact(contact);
                        if (!contactOperationStatus.Status)
                        {
                            orderOperationStatus =
                                (OrderOperationStatus) orderOperationStatus.InjectFrom(contactOperationStatus);
                            return orderOperationStatus;
                        }
                    }
                }
                orderOperationStatus = _orderCoordinator.GetOrder(contactOperationStatus.Contact, orderIndex);
                if (orderOperationStatus.Status)
                {
                    var order = orderOperationStatus.Order;
                    order.GiftAidAgreement = agreementStatus;
                    orderOperationStatus = _orderCoordinator.SaveOrder(order, false);
                    if (orderOperationStatus.Status)
                        orderOperationStatus.Message = "Gift Aid agreement updated successfully!";
                }


            }
            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e, "An error has occurred updating the Gift Aid agreement");
            }
            return orderOperationStatus;
        }


        public OrderOperationStatus AddVoucherToOrder(string voucherCode)
        {
            var orderOperationStatus=new OrderOperationStatus();
            try
            {
                var voucherOperationStatus = _voucherRepository.GetVoucher(voucherCode);

                if (!voucherOperationStatus.Status)
                {
                    orderOperationStatus =
                        (OrderOperationStatus) orderOperationStatus.InjectFrom(voucherOperationStatus);
                }
                else
                {
                    Voucher voucher = voucherOperationStatus.Voucher;

                    var contactOperationStatus = _contactService.GetContact();
                    if (!contactOperationStatus.Status)
                    {
                        orderOperationStatus =
                            (OrderOperationStatus) orderOperationStatus.InjectFrom(contactOperationStatus);
                    }
                    else
                    {
                        orderOperationStatus = _orderCoordinator.GetOrder(contactOperationStatus.Contact, voucher.OrderIndex);
                        if (orderOperationStatus.Status)
                        {
                            var order = orderOperationStatus.Order;
                            order.VoucherId = voucher.VoucherId;
                            order.VoucherAmount = voucher.Amount;
                            order.VoucherPercentage = voucher.Percentage;
                            order.VoucherPerItemAmount = voucher.PerItemAmount;
                            order.VoucherProductCategoryFilter = voucher.ProductCategoryFilter;
                            order.VoucherCategoryFilter = voucher.VoucherCategoryFilter;
                            order.VoucherMinimumItems = voucher.MinimumItems;
                            order.VoucherMinimumPayment = voucher.MinimumPayment;
                            order.VoucherInfo = "Voucher used: " + voucher.Title;
                            orderOperationStatus = _orderCoordinator.SaveOrder(order, false);
                            if (orderOperationStatus.Status)
                                orderOperationStatus.Message = "Voucher " + voucher.Title + " added successfully!";
                            if (order.GetVoucherTotal() == 0)
                                orderOperationStatus.Message +=
                                "  However, none of your current items are discounted with this voucher";

                        }

                    }
                }
            }
            catch (Exception e)
            {
                orderOperationStatus.LogFailedOperation(e,"An error has occurred adding the voucher");
            }
            return orderOperationStatus;
            
        }



        public OrderOperationStatus ConfirmOrder(int orderIndex)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                var contactOperationStatus = _contactService.GetContact();

                if (!contactOperationStatus.Status)
                {
                    orderOperationStatus =
                        (OrderOperationStatus) orderOperationStatus.InjectFrom(contactOperationStatus);
                }
                else
                {
                    orderOperationStatus = _orderCoordinator.GetOrder(contactOperationStatus.Contact, orderIndex);
                    if (orderOperationStatus.Status)
                    {
                        var order = orderOperationStatus.Order;
                        order.Status = "CONF";
                        order.UpdateOrderLines("CONF");
                        orderOperationStatus = _orderCoordinator.SaveOrder(order, true);
                        if (orderOperationStatus.Status)
                            orderOperationStatus.Message = "Thankyou for confirming your order.";

                    }
                }
            }
            catch (Exception e)
            {
                orderOperationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred updating the order line", e);
              
            }
            return orderOperationStatus;
        }
    }
}