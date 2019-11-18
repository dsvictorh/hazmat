using NTG.UI.Models.Hazmat;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace NTG.UI.Controllers.Hazmat
{
    [RoutePrefix("loot-box-claim")]
    public class LootBoxClaimController : HazmatBaseController
    {
        [HttpPost]
        [Route("list")]
        [Authorize(Roles = "Admin, ContentManager")]
        public JsonResult GetLootBoxClaims(LootBoxClaimGridModel viewModel)
        {
            viewModel.Load();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("save")]
        [Authorize(Roles = "Admin, ContentManager")]
        public JsonResult SaveLootBoxClaim(EditLootBoxClaimFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }
    }
}