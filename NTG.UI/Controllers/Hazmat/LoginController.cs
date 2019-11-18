using NTG.UI.Models.Hazmat;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace NTG.UI.Controllers.Hazmat
{
    public class LoginController : HazmatBaseController
    {
        [Route("auth")]
        public ActionResult Index()
        {
            var returnUrl = HttpContext.Request.Params["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                var sections = returnUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var section = sections.Length > 0 ? sections[0] : string.Empty;

                if (section == "hazmat")
                {
                    return RedirectToAction("Admin", new { returnUrl = returnUrl });
                }
            }

            return RedirectToAction("Member", new { returnUrl = returnUrl });
        }

        [Route("admin")]
        public ActionResult Admin()
        {
            if (User != null && User.IsInRole("Admin")) {
                return Redirect("~/hazmat");
            }

            return View("~/Views/Hazmat/Login.cshtml", new LoginViewModel { IsAdmin = true });
        }

        [Route("member")]
        public ActionResult Member()
        {
            return View("~/Views/Hazmat/Login.cshtml", new LoginViewModel { IsAdmin = false });
        }

        [HttpPost]
        [Route("login")]
        public JsonResult Login(LoginFormModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.ReturnUrl))
            {
                viewModel.ReturnUrl = Url.Action("Index", "Dashboard");
            }

            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/");
        }
    }
}