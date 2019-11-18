using Newtonsoft.Json;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace NTG.UI.Models.Hazmat
{
    public class UploadImageFormModel : BaseFormModel<UploadImageFormModel>
    {
        public string Url { get; private set; }
        public string PublicId { get; private set; }
        public string Folder { get; set; }
        public string[] Tags { get; set; }

        [JsonIgnore]
        public HttpPostedFileBase Image { get; set; }

        protected override bool OnValidate()
        {     
            if (Image == null)
            {
                AddMessage(Message.GLOBAL, new Message("No file chosen", MessageTypes.Warning));
                return false;
            }

            if (Image != null)
            {
                var isImageError = Image.IsImage();
                if (!string.IsNullOrEmpty(isImageError)) {
                    AddMessage(Message.GLOBAL, new Message("Chosen file is not an image", MessageTypes.Warning));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, Session.SessionVariables.User, "Attempted to upload Cloudinary non-image file of type" + Image.ContentType.ToLower() + ": " + isImageError);
                    return false;
                }
            }
            return true;
        }

        protected override void OnSubmit()
        {
            var fileName = Path.GetRandomFileName() + Path.GetExtension(Image.FileName).ToLower();
            var tempPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/temp"), fileName);
            Image.SaveAs(tempPath);
            var result = CloudinaryService.UploadImage(Folder, fileName, Tags, tempPath);
            File.Delete(tempPath);

            if (string.IsNullOrEmpty(result.error))
            {
                Url = result.uri.ToString();
                PublicId = result.publicId;

                var page = Page.Query.SingleOrDefault(p => p.UploadFolder == Folder);
                NTGLogger.LogSiteAction(HttpContext.Current.Request, Session.SessionVariables.User, "Uploaded image " + Url + " in folder: " + Folder, page?.Id, page?.Name, null, null);
                AddMessage(Message.GLOBAL, new Message("File successfully uploaded", MessageTypes.Success));
            }
            else
            {
                AddMessage(Message.GLOBAL, new Message("An error occurred while uploading the file. Please try again later", MessageTypes.Error));
                NTGLogger.LogError(HttpContext.Current.Request, "Cloudinary Error", result.error, nameof(UploadImageFormModel), nameof(CloudinaryService.UploadImage));
            }
        }
    }
}