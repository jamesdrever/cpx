using System;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Helper.Umbraco;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Umbraco.Web.Mvc;

namespace CustomerPortalExtensions.Controllers.Admin
{
    [PluginController("CustomerPortal")]
    public class OrderQueueSurfaceController : CpxBaseSurfaceController
    {
        private readonly IOrderQueueService _orderQueueService;
        private readonly IContactService _contactService;

        public OrderQueueSurfaceController(IOrderQueueService orderQueueService, IContactService contactService )
        {
            if (orderQueueService == null)
            {
                throw new ArgumentNullException("orderQueueService");
            }
            _orderQueueService = orderQueueService;
            if (contactService == null)
            {
                throw new ArgumentNullException("contactService");
            }
            _contactService = contactService;
        }

        [HttpPost]
        public ActionResult QueueOrder(int orderId, string url)
        {
            OrderOperationStatus operationStatus = _orderQueueService.QueueOrder(orderId);
            return ReturnBasedOnStatus(operationStatus, url);
        }
        
        public ActionResult QueueOrderWithTransaction(int orderId, string transactionNumber)
        {
            //TODO: need to log failed ecommerce transactions properly?
            if (transactionNumber != "-1")
            {
                OrderOperationStatus operationStatus = _orderQueueService.QueueOrder(orderId);

                if (operationStatus.Status)
                    return Content("Success");
                else
                {
                    return Content("Failed");
                }
            }
            return Content("Success");
        }

        [HttpPost]
        [Authorize(Roles = "cpxAdmin")]
        public ActionResult UpdateOrderLine(int orderId, int orderLineId, string status)
        {
            var operationStatus = _orderQueueService.UpdateOrderLine(orderId, orderLineId, status);
            if (operationStatus.Status)
            {
                TempData["StatusMessage"] = operationStatus.Message;
                return RedirectToCurrentUmbracoPage();
            }
            else
            {
                ModelState.AddModelError("Contact", operationStatus.FullErrorDetails);
                return CurrentUmbracoPage();
            }
        }

        [HttpPost]
        [Authorize(Roles = "cpxAdmin")]
        public ActionResult UpdateOrderLineAndContact(int orderId, int orderLineId, string status,int contactId, int externalContactNumber)
        {
            if (contactId!=0)
            {
                if (externalContactNumber == 0)
                {
                    ModelState.AddModelError("Contact", "You must specify a contact number");
                    return CurrentUmbracoPage();
                }
                else
                {
                    var contactOperationStatus=_contactService.UpdateExternalContactNumber(contactId, externalContactNumber);
                    if (!contactOperationStatus.Status)
                    {
                        ModelState.AddModelError("Contact", contactOperationStatus.FullErrorDetails);
                        return CurrentUmbracoPage();
                    }
                }
            }

            return UpdateOrderLine(orderId, orderLineId, status);
        }
    }
}
