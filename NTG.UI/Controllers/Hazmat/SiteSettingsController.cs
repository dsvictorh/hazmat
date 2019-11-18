using NTG.Logic.Models;
using NTG.UI.Models.Hazmat;
using System.Linq;
using System.Web.Mvc;
using System;
using NTG.UI.Session;
using System.Net;

namespace NTG.UI.Controllers.Hazmat
{
    [RoutePrefix("site-settings")]
    public class SiteSettingsController : HazmatBaseController
    {

        [Route]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View("~/Views/Hazmat/Dashboard/SiteSettings.cshtml");
        }


        [Route("info")]
        [Authorize(Roles = "Admin")]
        public JsonResult GetSettings()
        {
            var viewModel = new SiteSettingsViewModel();
            viewModel.GetSettings();

            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("save")]
        [Authorize(Roles = "Admin")]
        public JsonResult Save(EditSiteSettingsFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }
        
    }
}