using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Models;
using CustomerPortalExtensions.MVC.Models.Admin;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Controllers
{
    public class CpxBaseController : Umbraco.Web.Mvc.RenderMvcController
    {
        protected ActionResult ReturnErrorView<T>(T operationStatus, RenderModel model) where T : OperationStatus
        {
            var statusViewModel = new OperationStatusViewModel(model);
            statusViewModel.Message = operationStatus.Message;
            statusViewModel.FullErrorDetails = operationStatus.FullErrorDetails;
            return View("CPXDisplayError", statusViewModel);
        }

    }
}
