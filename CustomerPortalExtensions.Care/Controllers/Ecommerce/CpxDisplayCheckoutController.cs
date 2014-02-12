using System;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Models.Ecommerce;
using Omu.ValueInjecter;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.Controllers.Ecommerce
{
    public class CpxDisplayCheckoutController : CpxBaseController
    {
        private readonly IEcommerceService _ecommerceService;
        public CpxDisplayCheckoutController(IEcommerceService ecommerceService)
        {
            if (ecommerceService == null)
            {
                throw new ArgumentNullException("ecommerceService");
            }
            _ecommerceService = ecommerceService;
        }

        public override ActionResult Index(RenderModel model)
        {
            int orderIndex = 1;
            if (CurrentPage.GetProperty("orderIndex") != null)
                Int32.TryParse(CurrentPage.GetProperty("orderIndex").Value.ToString(), out orderIndex);

            OrderOperationStatus operationStatus = _ecommerceService.GetOrder(orderIndex);
            if (operationStatus.Status)
            {
                var viewOrder = new TestOrderViewModel(model);
                viewOrder.InjectFrom(operationStatus.Order);
                viewOrder.Status = operationStatus.Status;
                viewOrder.Message = operationStatus.Message;
                return CurrentTemplate(viewOrder);
            }
            return ReturnErrorView(operationStatus, model);
        }
        

        

    }
}
