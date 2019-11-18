using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Extensions;
using NTG.UI.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Controllers
{
    public class ErrorController : Controller
    {

        [Route("error")]
        public ActionResult Error()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.BodyClass = "light-body stop-loading";
            ViewBag.HideFooter = true;
            ViewBag.Text = "An unknown error has occured. Please try again later or contact support";
            ViewBag.Title = "Error";
            ViewBag.ErrorTopLogo = GlobalVariables.SiteSettings.ErrorImageIconTopUrl;
            ViewBag.Image = GlobalVariables.SiteSettings.ErrorImage500Url;
            ViewBag.Alt = "Error";
            return View("~/Views/NTG/Error.cshtml");
        }

        [Route("401")]
        public ActionResult Unauthorized()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.BodyClass = "light-body stop-loading";
            ViewBag.HideFooter = true;
            ViewBag.Text = "You do not have permission to access this page.";
            ViewBag.ErrorTopLogo = GlobalVariables.SiteSettings.ErrorImageIconTopUrl;
            ViewBag.Title = "Unauthorized";
            ViewBag.Image = GlobalVariables.SiteSettings.ErrorImage401Url;
            ViewBag.Alt = "Unauthorized";
            return View("~/Views/NTG/Error.cshtml");
        }


        [Route("403")]
        public ActionResult Forbidden()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.BodyClass = "light-body stop-loading";
            ViewBag.HideFooter = true;
            ViewBag.Text = "You do not have permission to access this page.";
            ViewBag.ErrorTopLogo = GlobalVariables.SiteSettings.ErrorImageIconTopUrl;
            ViewBag.Title = "Forbidden";
            ViewBag.Image = GlobalVariables.SiteSettings.ErrorImage403Url;
            ViewBag.Alt = "Forbidden";
            return View("~/Views/NTG/Error.cshtml");
        }

        [Route("404")]
        public ActionResult NotFound()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.BodyClass = "light-body stop-loading";
            ViewBag.HideFooter = true;
            ViewBag.Text = "The page you are looking for could not be found.";
            ViewBag.ErrorTopLogo = GlobalVariables.SiteSettings.ErrorImageIconTopUrl;
            ViewBag.Title = "Not Found";
            ViewBag.Image = GlobalVariables.SiteSettings.ErrorImage404Url;
            ViewBag.Alt = "Not Found";
            return View("~/Views/NTG/Error.cshtml");
        }

        [Route("500")]
        public ActionResult InternalError()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.BodyClass = "light-body stop-loading";
            ViewBag.HideFooter = true;
            ViewBag.Text = "An error has occured. Please try again later or contact support.";
            ViewBag.ErrorTopLogo = GlobalVariables.SiteSettings.ErrorImageIconTopUrl;
            ViewBag.Title = "Error";
            ViewBag.Image = GlobalVariables.SiteSettings.ErrorImage500Url;
            ViewBag.Alt = "Error";
            return View("~/Views/NTG/Error.cshtml");
        }

        [Route("503")]
        public ActionResult Maintenance()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.BodyClass = "light-body stop-loading";
            ViewBag.HideFooter = true;
            ViewBag.Text = "The site is down for maintenance. Please try again later";
            ViewBag.ErrorTopLogo = GlobalVariables.SiteSettings.ErrorImageIconTopUrl;
            ViewBag.Title = "Maintenance";
            ViewBag.Image = GlobalVariables.SiteSettings.ErrorImage503Url;
            ViewBag.Alt = "Maintenance";
            return View("~/Views/NTG/Error.cshtml");
        }
    }
}