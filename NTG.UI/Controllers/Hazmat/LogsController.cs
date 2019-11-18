using NTG.UI.Models.Hazmat;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace NTG.UI.Controllers.Hazmat
{
    [RoutePrefix("logs")]
    public class LogsController : HazmatBaseController
    {
        [HttpPost]
        [Route("error")]
        [Authorize(Roles = "Admin")]
        public JsonResult ErrorLogs(ErrorLogsGridModel viewModel)
        {
            viewModel.Load();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("site")]
        [Authorize(Roles = "Admin")]
        public JsonResult SiteLogs(SiteLogsGridModel viewModel)
        {
            viewModel.Load();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("security")]
        [Authorize(Roles = "Admin")]
        public JsonResult SecurityLogs(SecurityLogsGridModel viewModel)
        {
            viewModel.Load();
            return Json(JsonConvert.SerializeObject(viewModel));
        }
    }
}