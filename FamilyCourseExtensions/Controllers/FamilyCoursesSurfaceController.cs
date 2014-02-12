using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FamilyCourseExtensions.Infrastructure;
using FamilyCourseExtensions.Models;
using Umbraco.Web.Mvc;

namespace FamilyCourseExtensions.Controllers
{
    [PluginController("FamilyCourseExtensions")]
    public class FamilyCoursesSurfaceController : SurfaceController
    {
        //
        // GET: /FamilyCourses/

        public ActionResult DisplayBookingForm()
        {
            var locationRepository=new LocationRepository();
            List<Location> listOfLocations = locationRepository.GetLocations();
            return PartialView("FAMDisplayBookingForm",listOfLocations);
        }

        public JsonResult GetLocation(int id)
        {
            var locationRepository = new LocationRepository();
            return Json(locationRepository.GetLocation(id), JsonRequestBehavior.AllowGet); ;
        }

    }
}
