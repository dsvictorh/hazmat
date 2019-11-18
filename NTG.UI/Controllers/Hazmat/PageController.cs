using NTG.Logic.Models;
using NTG.UI.Models.Hazmat;
using System.Linq;
using System.Web.Mvc;
using System;
using NTG.UI.Session;
using System.Net;

namespace NTG.UI.Controllers.Hazmat
{
    [RoutePrefix("page")]
    public class PageController : HazmatBaseController
    {

        [Route("list")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult PageList()
        {
            var viewModel = new PageListViewModel();
            viewModel.GetPages();

            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("list/save")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageList(PageListFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [Route("create")]
        [Authorize(Roles = "Admin, Content Manager")]
        public ActionResult Create()
        {
            return View("~/Views/Hazmat/Dashboard/Page.cshtml");
        }

        [Route("{pageName}")]
        [Authorize(Roles = "Admin, Content Manager")]
        public ActionResult Index(string pageName)
        {
            var page = Page.Query.SingleOrDefault(p => p.Name.Equals(pageName.Replace("-", " "), StringComparison.OrdinalIgnoreCase));
            if (page == null)
            {
                return new HttpNotFoundResult();
            }
            else if (page.AdminLocked && !SessionVariables.User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View("~/Views/Hazmat/Dashboard/Page.cshtml");
        } 

        [Route("{pageName}/modules")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult PageModules(string pageName)
        {
            var viewModel = new PageModulesViewModel();
            viewModel.GetModules(pageName.Replace("-", " "));

            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("{pageName}/save")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePage(string pageName, CreateEditPageFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/callout")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModuleCallout(string pageName, CreateEditModuleCalloutFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/boxes")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModuleBoxes(string pageName, CreateEditModuleBoxesFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/simplecards")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModuleSimpleCards(string pageName, CreateEditModuleSimpleCardsFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/profilecards")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModuleProfileCards(string pageName, CreateEditModuleProfileCardsFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/freetext")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModuleFreeText(string pageName, CreateEditModuleFreeTextFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/gallery")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModuleGallery(string pageName, CreateEditModuleGalleryFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("{pageName}/save/promo")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult SavePageModulePromo(string pageName, CreateEditModulePromoFormModel viewModel)
        {
            viewModel.PageName = pageName.Replace("-", " ");
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }
    }
}