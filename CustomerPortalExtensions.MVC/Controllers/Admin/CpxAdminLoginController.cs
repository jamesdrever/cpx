using System.Web.Mvc;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Controllers.Admin
{
    public class CpxAdminLoginController : CpxBaseController
    {
 
        public override ActionResult Index(RenderModel model)
        {
            return CurrentTemplate(model);
        }
    }
}
