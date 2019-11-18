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
    public class DeleteImageFormModel : BaseFormModel<DeleteImageFormModel>
    {
        [Required]
        public string PublicId { get; set; }

        [Required]
        public string Url { get; set; }

        protected override bool OnValidate()
        {
            if (!Url.Contains(PublicId))
            {
                AddMessage(Message.GLOBAL, new Message("Cannot delete image: Url and Public Id differ", MessageTypes.Error));
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, Session.SessionVariables.User, "Attempted to delete Cloudinary image with difference between Url and Public Id");
            }
            return true;
        }

        protected override void OnSubmit()
        {
            var result = CloudinaryService.DeleteImage(PublicId);
            if (string.IsNullOrEmpty(result.error))
            {
                NTGLogger.LogSiteAction(HttpContext.Current.Request, Session.SessionVariables.User, "Deleted image " + Url, null, null, null, null);
                AddMessage(Message.GLOBAL, new Message("File successfully deleted", MessageTypes.Success));
            }
            else
            {
                AddMessage(Message.GLOBAL, new Message("An error occurred while deleting the image. Please try again later", MessageTypes.Error));
                NTGLogger.LogError(HttpContext.Current.Request, "Cloudinary Error", result.error, nameof(DeleteImageFormModel), nameof(CloudinaryService.DeleteImage));
            }
        }
    }
}