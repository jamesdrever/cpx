using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Helper.Umbraco;
using Umbraco.Web.Mvc;

namespace CustomerPortalExtensions.MVC.Controllers
{
    public class CpxBaseSurfaceController : SurfaceController
    {
        protected ActionResult ReturnBasedOnStatus<T>(T status, string url) where T : OperationStatus
        {
            if (!status.Status&&String.IsNullOrEmpty(url))
            {
                ModelState.AddModelError("CPX", status.Message);
                return CurrentUmbracoPage();
            }
            TempData["Status"] = status.Status;
            string umbracoCode = "";
            if (!String.IsNullOrEmpty(status.MessageCode))
                umbracoCode = UmbracoHelper.GetDictionaryItem(status.MessageCode);
            TempData["StatusMessage"] = String.IsNullOrEmpty(umbracoCode) ? status.Message : umbracoCode;
            string referrer=(HttpContext.Request.UrlReferrer==null) ? HttpContext.Request.Url.AbsoluteUri : HttpContext.Request.UrlReferrer.AbsoluteUri;
            url = (String.IsNullOrEmpty(url)) ? referrer : url + "?message=" + status.Message;
            return new RedirectResult(url);
            
        }

        protected RedirectResult ReturnToCurrentPage()
        {
            string referrer = (HttpContext.Request.UrlReferrer == null) ? HttpContext.Request.Url.AbsoluteUri : HttpContext.Request.UrlReferrer.AbsoluteUri;
            string url = (String.IsNullOrEmpty(Request["returnUrl"]))
                             ? referrer
                             : Request["returnUrl"];
            return new RedirectResult(url);
        }

    }
}
