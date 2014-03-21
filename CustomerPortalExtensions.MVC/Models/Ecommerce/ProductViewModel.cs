using System;
using System.Collections.Generic;
using CustomerPortalExtensions.Domain.ECommerce;
using umbraco.presentation.install.utills;

namespace CustomerPortalExtensions.MVC.Models.Ecommerce
{
    public class ProductSummaryViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string OrderPage { get; set; }
        public string ButtonContent { get; set; }
    }

    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string OrderPage { get; set; }
        public string ProductStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public List<ProductOption> ProductOptions { get; set; }
        public List<PaymentOption> PaymentOptions { get; set; }
 
        public string Message { get; set; }
        public string FullErrorDetails { set; get; }
        public bool Status { get; set; }
        public bool CanAddToBasket { get; set; }

    }
}