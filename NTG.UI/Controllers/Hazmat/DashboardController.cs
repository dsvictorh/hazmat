using NTG.UI.Models.Hazmat;
using System.Web.Mvc;

namespace NTG.UI.Controllers.Hazmat
{
    public class DashboardController : HazmatBaseController
    {
        [Route]
        [Authorize(Roles = "Admin, Content Manager")]
        public ActionResult Index()
        {
            return View("~/Views/Hazmat/Dashboard/Index.cshtml");            
        }

        [Route("modules")]
        [Authorize(Roles = "Admin, Content Manager")]
        public ActionResult Modules()
        {
            return View("~/Views/Hazmat/Dashboard/Modules.cshtml");
        }

        [Route("site-log")]
        [Authorize(Roles = "Admin")]
        public ActionResult SiteLog()
        {
            return View("~/Views/Hazmat/Dashboard/SiteLog.cshtml");
        }

        [Route("security-log")]
        [Authorize(Roles = "Admin")]
        public ActionResult SecurityLog()
        {
            return View("~/Views/Hazmat/Dashboard/SecurityLog.cshtml");
        }

        [Route("error-log")]
        [Authorize(Roles = "Admin, Content Manager")]
        public ActionResult ErrorLog()
        {
            return View("~/Views/Hazmat/Dashboard/ErrorLog.cshtml");
        }

        [Route("encrypt")]
        [Authorize(Roles = "Admin")]
        public ActionResult Encrypt()
        {
            return View("~/Views/Hazmat/Dashboard/Encrypt.cshtml");
        }

        [HttpPost]
        [Route("encrypt")]
        [Authorize(Roles = "Admin")]
        public JsonResult Encrypt(EncryptFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

#if DEBUG
        [Route("devencrypt")]
        public ActionResult DevEncrypt()
        {
            return View("~/Views/Hazmat/Dashboard/Encrypt.cshtml");
        }

        [HttpPost]
        [Route("devencrypt")]
        public JsonResult DevEncrypt(EncryptFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }
#endif
    }
}