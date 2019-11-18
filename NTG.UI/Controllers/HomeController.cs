using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Extensions;
using NTG.UI.Models.Hazmat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Controllers
{
    [Route("~/")]
    public class HomeController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            bool footerTheme;
            var page = Page.Query.SingleOrDefault(p =>  p.Position == 1);
            if (page == null)
            {
                return new HttpNotFoundResult();
            }

            var modules = ModuleService.GetModulesFromCache(page.Id, out footerTheme, PageModuleStates.Published);
            ViewBag.HeroImage = page.HeroImage;
            ViewBag.MobileHeroImage = page.MobileHeroImage;
            ViewBag.Title = page.Name;
            ViewBag.FooterTheme = footerTheme;
            return View("~/Views/NTG/Home/Page.cshtml", (new { Modules = modules.Select(m => m.ToExpando()) }).ToExpando());
        }

        [Route("page/{pageName}")]
        public ActionResult GetPage(string pageName)
        {
            bool footerTheme;
            var page = Page.Query.SingleOrDefault(p => p.Name.Equals(pageName.Replace("-", " "), StringComparison.OrdinalIgnoreCase) && p.Position != 1 && p.Visible);
            if (page == null)
            {
                return new HttpNotFoundResult();
            }

            var modules = ModuleService.GetModulesFromCache(page.Id, out footerTheme, PageModuleStates.Published);
            ViewBag.HeroImage = page.HeroImage;
            ViewBag.MobileHeroImage = page.MobileHeroImage;
            ViewBag.Title = page.Name;
            ViewBag.FooterTheme = footerTheme;
            return View("~/Views/NTG/Home/Page.cshtml", (new { Modules = modules.Select(m => m.ToExpando()) }).ToExpando());
        }

        [HttpPost]
        [Route("loot-box/claim")]
        public JsonResult ClaimLootBox(CreateLootBoxClaimFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [Route("loot-box/info")]
        public JsonResult GetLootBoxInfo(LootBoxInfoViewModel viewModel)
        {
            viewModel.GetInfo();
            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }
    }
}