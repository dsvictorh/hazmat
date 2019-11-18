using NTG.UI.Models.Hazmat;
using System.Web.Mvc;

namespace NTG.UI.Controllers.Hazmat
{
    [RoutePrefix("media")]
    public class MediaController : HazmatBaseController
    {
        [Route("images/folder")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult ImagesByFolder(string folderStructure, string tags)
        {
            var viewModel = new ImagesListViewModel();
            viewModel.GetImagesByFolder(folderStructure, tags.Split(','));
            return Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("images/upload")]
        [Authorize(Roles = "Admin, Content Manager")]
        public JsonResult UploadImageToFolder(UploadImageFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }

        [HttpPost]
        [Route("images/delete")]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteImageToFolder(DeleteImageFormModel viewModel)
        {
            viewModel.Submit();
            return Json(JsonConvert.SerializeObject(viewModel));
        }
    }
}