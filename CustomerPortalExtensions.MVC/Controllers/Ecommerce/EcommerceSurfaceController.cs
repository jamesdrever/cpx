using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Helper.Umbraco;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.MVC.Models.Ecommerce;
using Omu.ValueInjecter;
using Umbraco.Web.Mvc;

namespace CustomerPortalExtensions.MVC.Controllers.Ecommerce
{
    [PluginController("CustomerPortal")]
    public class ECommerceSurfaceController : CpxBaseSurfaceController
    {
        //
        private readonly IEcommerceService _ecommerceService;

        public ECommerceSurfaceController(IEcommerceService ecommerceService)
        {
            if (ecommerceService == null)
            {
                throw new ArgumentNullException("ecommerceService");
            }
            _ecommerceService = ecommerceService;
        }


        

        private ProductViewModel GetCurrentProductViewModel()
        {
            int productId = CurrentPage.Id;
            var operationStatus = _ecommerceService.GetProduct(productId);
            Product product;
            List<ProductOption> productOptions;
            var viewProduct = new ProductViewModel();
            if (operationStatus.Status)
            {
                product = operationStatus.Product;
                var productOptionsOperationStatus = _ecommerceService.GetProductOptions(productId);
                if (productOptionsOperationStatus.Status)
                {
                    viewProduct.ProductOptions = productOptionsOperationStatus.ProductOptions;
                    var paymentOptionsOperationStatus = _ecommerceService.GetPaymentOptions(productId);
                    if (paymentOptionsOperationStatus.Status)
                        viewProduct.PaymentOptions = paymentOptionsOperationStatus.PaymentOptions;
                }
                viewProduct.InjectFrom(product);
                viewProduct.ProductStatus = product.Status;
            }
            
           
            viewProduct.Status = operationStatus.Status;
            viewProduct.Message = operationStatus.Message;
            viewProduct.FullErrorDetails = operationStatus.FullErrorDetails;
            
            return viewProduct;
        }


        public ActionResult DisplayOrder()
        {
            return PartialView("DisplayOrder", GetOrderViewModel());
        }

        public ActionResult DisplayCheckout()
        {
            return PartialView("DisplayCheckout", GetOrderViewModel());
        }

        public ActionResult DisplayPrintableOrder()
        {
            return PartialView("DisplayPrintableOrder", GetOrderViewModel());
        }

        public ActionResult GetOrderSummary(string partialViewName)
        {
            OrderSummaryOperationStatus operationSummaryOperationStatus;
            var orderSummaryViewModel = new OrderSummaryViewModel();
            int orderId = 0;
            if (Request.QueryString["OrderId"] != null)
            {
                Int32.TryParse(Request.QueryString["OrderId"], out orderId);
                if (orderId > 0)
                {
                    operationSummaryOperationStatus = _ecommerceService.GetOrderSummaryById(orderId);
                    if (operationSummaryOperationStatus.Status)
                    {
                        orderSummaryViewModel.InjectFrom(operationSummaryOperationStatus.OrderSummary);
                    }
                    orderSummaryViewModel.Status = operationSummaryOperationStatus.Status;
                    orderSummaryViewModel.FullErrorDetails = operationSummaryOperationStatus.FullErrorDetails;
                }
                orderSummaryViewModel.Status = false;
                orderSummaryViewModel.FullErrorDetails = "An valid order number has not been specified.";
            }
            else
            {
                orderSummaryViewModel.Status = false;
                orderSummaryViewModel.FullErrorDetails = "An order has not been specified.";
            }



            return PartialView(partialViewName,orderSummaryViewModel);
        }

        private OrderViewModel GetOrderViewModel()
        {

            int orderIndex = 1;
            if (CurrentPage.GetProperty("orderIndex") != null)
                Int32.TryParse(CurrentPage.GetProperty("orderIndex").Value.ToString(), out orderIndex);

            var operationStatus = _ecommerceService.GetOrder(orderIndex);

            var viewOrder = new OrderViewModel();

            viewOrder.Status = operationStatus.Status;
            viewOrder.Message = operationStatus.Message;
            viewOrder.FullErrorDetails = operationStatus.FullErrorDetails;
            if (operationStatus.Status)
            {
                viewOrder.InjectFrom(operationStatus.Order);
                viewOrder.VoucherTotal = operationStatus.Order.GetVoucherTotal();
                viewOrder.ContainsGiftAidableProducts = operationStatus.Order.ContainsGiftAidableProducts();
            }
            return viewOrder;
        }

        public JsonResult AddProductToOrderJson(int productId)
        {
            return AddProductAndQuantityToOrderJson(productId, 1);
        }

        public ActionResult AddProductToOrder(int productId)
        {
            return AddProductAndQuantityToOrder(productId, 1, null);
        }

        public ActionResult AddProductToOrderWithRedirect(int productId, string url)
        {
            return AddProductAndQuantityToOrder(productId, 1, url);
        }

        public JsonResult AddProductAndQuantityToOrderJson(int productId, int quantity)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddProductToOrder(productId, quantity);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProductAndQuantityToOrder(int productId, int quantity, string url)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddProductToOrder(productId, quantity);
            return ReturnBasedOnStatus(operationStatus,url);
        }


        public ActionResult AddDonationToOrderWithRedirect(int productId, string url)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddDonationToOrder(productId);
            return ReturnBasedOnStatus(operationStatus, url);
        }

        public ActionResult AddDonationToOrder(int productId)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddDonationToOrder(productId);
            return ReturnBasedOnStatus(operationStatus, null);
        }

        public ActionResult AddFlexibleDonationToOrderWithRedirect(int productId, decimal paymentAmount,string url)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddFlexibleDonationToOrder(productId, paymentAmount);
            return ReturnBasedOnStatus(operationStatus, url);
        }

        public ActionResult AddFlexibleDonationToOrder(int productId, decimal paymentAmount)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddFlexibleDonationToOrder(productId, paymentAmount);
            return ReturnBasedOnStatus(operationStatus,null);
        }

        public JsonResult AddProductAndSpecifiedPaymentToOrderJson(int productId,int optionId, int quantity, string paymentType)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddProductToOrder(productId, optionId, quantity,paymentType);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProductAndSpecifiedPaymentToOrder(int productId, int optionId, int quantity, string paymentType, string url)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddProductToOrder(productId, optionId, quantity, paymentType);
            return ReturnBasedOnStatus(operationStatus,url);
        }

        public ActionResult AddProductWithFlexibleDatesToOrder(int productId, int optionId, int quantity, DateTime startDate, DateTime finishDate, string paymentType, string url)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddCourseWithFlexibleDatesToOrder(productId, optionId, startDate, finishDate, quantity, paymentType);
            return ReturnBasedOnStatus(operationStatus, url);
        }

        public JsonResult GetBespokeProductPrice(int productId, int optionId, int quantity, DateTime startDate,
                                                 DateTime finishDate)
        {
            ProductOperationStatus operationStatus = _ecommerceService.GetProduct(productId, optionId, quantity,startDate,finishDate);
            return Json(operationStatus,JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrderLineJson(int productId, int optionId, int quantity)
        {
            OrderOperationStatus operationStatus = _ecommerceService.Update(productId, optionId,quantity);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateOrderLine(int productId, int optionId, int quantity)
        {
            OrderOperationStatus operationStatus = _ecommerceService.Update(productId, optionId, quantity);
            return ReturnBasedOnStatus(operationStatus,null);
        }

        public JsonResult UpdateOrderLineWithSpecifiedPaymentJson(int productId, int optionId, int newOptionId, int quantity, string paymentType)
        {
            OrderOperationStatus operationStatus = _ecommerceService.Update(productId,optionId,newOptionId, quantity, paymentType);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateOrderLineWithSpecifiedPayment(int productId, int optionId, int newOptionId, int quantity, string paymentType)
        {
            OrderOperationStatus operationStatus = _ecommerceService.Update(productId, optionId, newOptionId, quantity, paymentType);
            return ReturnBasedOnStatus(operationStatus,null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="optionId">if you specify this as zero, it will just remove the product</param>
        /// <returns></returns>
        public JsonResult RemoveFromOrderJson(int productId, int optionId)
        {
            OrderOperationStatus operationStatus = _ecommerceService.Remove(productId,optionId);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveFromOrder(int productId, int optionId)
        {
            OrderOperationStatus operationStatus = _ecommerceService.Remove(productId, optionId);
            return ReturnBasedOnStatus(operationStatus,null);
        }

        public JsonResult UpdateSpecialRequirementsJson(string specialRequirements,int orderIndex)
        {
            OrderOperationStatus operationStatus = _ecommerceService.UpdateSpecialRequirements(specialRequirements,orderIndex);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSpecialRequirements(string specialRequirements, int orderIndex)
        {
            OrderOperationStatus operationStatus = _ecommerceService.UpdateSpecialRequirements(specialRequirements,orderIndex);
            return ReturnBasedOnStatus(operationStatus,null);
        }

        public ActionResult UpdateGiftAidAgreement(bool agreementStatus, int orderIndex)
        {
            OrderOperationStatus operationStatus = _ecommerceService.UpdateGiftAidAgreement(agreementStatus,orderIndex);
            return ReturnBasedOnStatus(operationStatus, null);
        }


        public JsonResult AddVoucherToOrderJson(string voucherCode)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddVoucherToOrder(voucherCode);
            return Json(GetOrderResultSummary(operationStatus), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddVoucherToOrder(string voucherCode)
        {
            OrderOperationStatus operationStatus = _ecommerceService.AddVoucherToOrder(voucherCode);
            return ReturnBasedOnStatus(operationStatus, null);
        }

        public string ConfirmOrder()
        {
            int orderIndex = 1;
            if (CurrentPage.GetProperty("orderIndex") != null)
                Int32.TryParse(CurrentPage.GetProperty("orderIndex").Value.ToString(), out orderIndex);

            OrderOperationStatus operationStatus = _ecommerceService.ConfirmOrder(orderIndex);
            return operationStatus.Message;
        }


        public ActionResult DisplayAddProductToBasket(int productId, string content, string buttonContent)
        {
            ProductSummaryOperationStatus operationStatus = _ecommerceService.GetProductSummary(productId);
            var viewProduct = new ProductSummaryViewModel();
            viewProduct.InjectFrom(operationStatus.Product);
            viewProduct.OrderPage = operationStatus.OrderPage;
            viewProduct.Content = content;
            viewProduct.ButtonContent = buttonContent;
            return PartialView("DisplayAddProductToBasket", viewProduct);
        }

        public ActionResult DisplayAddCurrentCourseToBasket()
        {
            return PartialView("DisplayAddProductToBasket", GetCurrentProductViewModel());
        }

        public ActionResult DisplayAddDonationToBasket(int productId)
        {
            ProductSummaryOperationStatus operationStatus = _ecommerceService.GetProductSummary(productId);
            var viewProduct = new ProductSummaryViewModel();
            viewProduct.InjectFrom(operationStatus.Product);
            viewProduct.OrderPage = operationStatus.OrderPage;
            return PartialView("DisplayAddDonationToBasket", viewProduct);
        }

        public ActionResult DisplayAddFlexibleDonationToBasket(int productId)
        {
            ProductSummaryOperationStatus operationStatus = _ecommerceService.GetProductSummary(productId);
            var viewProduct = new ProductSummaryViewModel();
            viewProduct.InjectFrom(operationStatus.Product);
            viewProduct.OrderPage = operationStatus.OrderPage;
            return PartialView("DisplayAddFlexibleDonationToBasket", viewProduct);
        }

        private OrderResultSummary GetOrderResultSummary(OrderOperationStatus operationStatus)
        {
            var resultSummary=new OrderResultSummary();
            
            resultSummary.Status = operationStatus.Status;

            string umbracoCode = "";
            if (!String.IsNullOrEmpty(operationStatus.MessageCode))
                umbracoCode = UmbracoHelper.GetDictionaryItem(operationStatus.MessageCode);
            resultSummary.Message = String.IsNullOrEmpty(umbracoCode) ? operationStatus.Message : umbracoCode;
            
            if (operationStatus.Status)
            {
                resultSummary.NumberOfItems = operationStatus.Order.NumberOfItems;
                resultSummary.OrderId = operationStatus.Order.OrderId;
                resultSummary.PaymentSubTotal = string.Format("{0:C}",operationStatus.Order.PaymentSubTotal);
                resultSummary.PaymentSubTotalIncludingDiscountAndVoucher = string.Format("{0:C}",operationStatus.Order.PaymentSubTotalIncludingDiscountAndVoucher);
                resultSummary.PaymentTotal = string.Format("{0:C}",operationStatus.Order.PaymentTotal);
                resultSummary.ProductSubTotal = string.Format("{0:C}",operationStatus.Order.ProductSubTotal);
                resultSummary.ShippingInfo = operationStatus.Order.ShippingInfo;
                resultSummary.ShippingTotal = string.Format("{0:C}",operationStatus.Order.ShippingTotal);
                resultSummary.DiscountInfo = operationStatus.Order.DiscountInfo;
                resultSummary.DiscountTotal = string.Format("{0:C}",-operationStatus.Order.DiscountTotal);
                resultSummary.VoucherInfo = operationStatus.Order.VoucherInfo;
                resultSummary.VoucherTotal = string.Format("{0:C}",-operationStatus.Order.GetVoucherTotal());
                resultSummary.ContainsGiftAidableProducts = operationStatus.Order.ContainsGiftAidableProducts();
                //resultSummary.InjectFrom<IgnoreNulls>(operationStatus.Order);
                if (operationStatus.OrderLine!=null)
                {
                    resultSummary.OrderLine = operationStatus.OrderLine;
                    resultSummary.PaymentLineTotal = string.Format("{0:C}",operationStatus.OrderLine.PaymentLineTotal);
                    
                }
                resultSummary.OrderId = operationStatus.Order.OrderId;
            }
            return resultSummary;
        }
    }
}
