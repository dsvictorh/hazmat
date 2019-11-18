using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Models.Hazmat
{
    public class ImagesListViewModel : BaseAjaxModel
    {
        public List<ImageInfo> Images { get; set; }

        public void GetImagesByFolder(string folderStructure, string[] tags)
        {
            Images = CloudinaryService.GetImages(folderStructure, tags).ToList();
        }
    }
}