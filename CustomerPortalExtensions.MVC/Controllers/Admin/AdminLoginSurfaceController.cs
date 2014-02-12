using System;
using System.Web.Mvc;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Models;
using CustomerPortalExtensions.MVC.Models;

namespace CustomerPortalExtensions.MVC.Controllers.Admin
{
    public class AdminLoginSurfaceController : CpxBaseSurfaceController
    {
        private readonly IContactService _contactService;

        public AdminLoginSurfaceController(IContactService contactService)
        {
            if (contactService == null) throw new ArgumentNullException("contactService");
            _contactService = contactService;
        }

        //
        // GET: /AdminLogicSurface/

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var operationStatus = _contactService.Authenticate(login.Email, login.Password);
                if (operationStatus.Status)
                {
                    return ReturnToCurrentPage();
                }
                else
                {
                    ModelState.AddModelError("Login.Email", "Your username and password have not been recognised.");
                    return CurrentUmbracoPage();
                }
            }
            return CurrentUmbracoPage();
        }
    }
}
