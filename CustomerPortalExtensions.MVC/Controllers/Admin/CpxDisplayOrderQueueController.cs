using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using CustomerPortalExtensions.MVC.Models.Admin;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Controllers.Admin
{
    public class CpxDisplayOrderQueueController : CpxBaseController
    {
        private readonly IOrderQueueService _orderQueueService;
        private readonly ILocationHandler _locationHandler;
        private readonly IContactAuthenticationHandler _authenticationHandler;

        public CpxDisplayOrderQueueController(IOrderQueueService orderQueueService, ILocationHandler locationHandler, IContactAuthenticationHandler authenticationHandler) 
        {
            if (orderQueueService == null)
            {
                throw new ArgumentNullException("orderQueueService");
            }
            _orderQueueService = orderQueueService;
            if (locationHandler == null)
            {
                throw new ArgumentNullException("locationHandler");
            }
            _locationHandler = locationHandler;
            if (authenticationHandler == null) throw new ArgumentNullException("authenticationHandler");
            
            _authenticationHandler = authenticationHandler;
        }
        [Authorize (Roles="cpxAdmin")]
        public override ActionResult Index(RenderModel model)
        {
            var requestOrderStatus = Request["OrderStatus"] ?? "QUE";
            var orderStatus = Response.Cookies["OrderStatus"] == null ? "" : Response.Cookies["OrderStatus"].Value;

            if (requestOrderStatus != orderStatus)
            {
                Response.Cookies["OrderStatus"].Value = requestOrderStatus;
                orderStatus = requestOrderStatus;
            }

            var requestLocation = Request["Location"] ?? _authenticationHandler.GetPropertyValue("location");
            var location = Response.Cookies["Location"] == null ? "" : Response.Cookies["Location"].Value;

            if (requestLocation != location)
            {
                Response.Cookies["Location"].Value = requestLocation;
                location = requestLocation;
            }

            var operationStatus = _orderQueueService.GetOrderQueue(orderStatus, location);
            var orderQueueViewModel = new OrderQueueViewModel(model)
                {
                    Status = operationStatus.Status,
                    Message = operationStatus.Message
                };

            if (operationStatus.Status)
            {
                orderQueueViewModel.OrderQueueItems = operationStatus.OrderQueueItems;
                orderQueueViewModel.OrderStatusCode = orderStatus;
                orderQueueViewModel.Location = location;
                orderQueueViewModel.LocationOptions = new List<Location> {new Location {Code = "", Title = ""}};
                orderQueueViewModel.LocationOptions.AddRange(_locationHandler.GetLocations());
                return CurrentTemplate(orderQueueViewModel);
            }
            return ReturnErrorView(operationStatus, model);
        }
    }
}
