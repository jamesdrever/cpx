using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.ECommerce;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.Models
{

    public class OrdersViewModel : RenderModel
    {
        public OrdersViewModel(RenderModel model)
            : base(model.Content, model.CurrentCulture)
        {
        }

        public List<Order> Orders;
    }
}