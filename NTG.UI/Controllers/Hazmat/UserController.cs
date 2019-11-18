using NTG.Logic.Models;
using NTG.UI.Models.Hazmat;
using NTG.UI.Session;
using System.Linq;
using System.Web.Mvc;

namespace NTG.UI.Controllers.Hazmat
{
    [RoutePrefix("user")]
    public class UserController : HazmatBaseController
    {

        [Route("admin")]
        [Authorize(Roles = "Hazmat")]
        public ActionResult AdminDetails()
        {
            return View("~/Views/Hazmat/Dashboard/Admin.cshtml");
        }

        [Route("details")]
        [Authorize]
        public JsonResult UserDetails()
        {
            var viewModel = new UserDetailsViewModel();
            viewModel.GetUser();

            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }

        [Route("admin/list")]
        [Authorize(Roles = "Admin")]
        public ActionResult UserList()
        {
            return View("~/Views/Hazmat/Dashboard/UserList.cshtml");
        }

        [HttpPost]
        [Route("admin/list")]
        [Authorize(Roles = "Admin")]
        public JsonResult AdminListGrid(AdminListGridModel viewModel)
        {
            viewModel.Load();

            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("admin/save")]
        [Authorize(Roles = "Hazmat")]
        public JsonResult SaveDetails(EditAdminFormModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.User))
            {
                viewModel.User = SessionVariables.User.Email;
            }
            viewModel.Submit();

            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("admin/create")]
        [Authorize(Roles = "Admin")]
        public JsonResult Create(CreateAdminFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [Route("admin/roles")]
        [Authorize(Roles = "Admin")]
        public JsonResult RoleList()
        {
            var viewModel = new RoleListViewModel();
            viewModel.GetRoles();

            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }
    }
}