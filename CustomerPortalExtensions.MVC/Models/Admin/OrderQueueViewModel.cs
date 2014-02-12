using System.Collections.Generic;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Ecommerce;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Models.Admin
{
    public class OrderQueueViewModel : RenderModel
    {
        public OrderQueueViewModel(RenderModel model)
            : base(model.Content, model.CurrentCulture)
        {
            OrderStatusOptions = new List<OrderStatus>();
            OrderStatusOptions.Add(new OrderStatus { Code = "PROV", Description = "Provisional" });
            OrderStatusOptions.Add(new OrderStatus { Code = "QUE", Description = "Queued" });
            OrderStatusOptions.Add(new OrderStatus { Code = "CONF", Description = "Confirmed" });
            OrderStatusOptions.Add(new OrderStatus { Code = "CONP", Description = "Confirmed - Awaiting Payment" });
            OrderStatusOptions.Add(new OrderStatus { Code = "PAID", Description = "Paid" });
            OrderStatusOptions.Add(new OrderStatus { Code = "REJ", Description = "Rejected" });

        }

        public List<OrderQueueItem> OrderQueueItems { get; set; }
        public string OrderStatusCode { get; set; }
        public List<OrderStatus> OrderStatusOptions { get; set; }
        public string Location { get; set; }
        public List<Location> LocationOptions { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string User { get; set; }

    }
}