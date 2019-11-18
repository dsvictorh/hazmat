using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTG.UI.Models.Hazmat
{
    public class CreateEditModuleGalleryImageLinkSubFormModel : BaseSubFormModel<CreateEditModuleGalleryImageLinkSubFormModel>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Text { get; set; }

        [Required]
        public string Image { get; set; }

        public int Position { get; set; }

        public ModuleGalleryImage ModuleGalleryImage { get; set; }

        public bool IsDelete { get; set; }

        public bool IsNewImage { get; set; }

        private ModuleGalleryImageLink _link;

        protected override bool OnValidate()
        {
            if (ModuleGalleryImage == null)
            {
                AddMessage(nameof(Text) + GetErrorMessageSufix(), new Message("Attempted to save link to non-existing image", MessageTypes.Error));
                return false;
            }

            _link = ModuleGalleryImageLink.Query.SingleOrDefault(mgil => mgil.Id == Id && mgil.ModuleGalleryImageId == ModuleGalleryImage.Id);
            if (Id != 0 && _link == null)
            {
                AddMessage(nameof(Text) + GetErrorMessageSufix(), new Message("Link does not belong to current image or does not exist", MessageTypes.Error));
                return false;
            }

            return true;
        }

        protected override void OnSubmit(NTGDBTransactional transaction)
        {
            if (IsDelete)
            {
                if (Id != 0)
                {
                    _link.Delete(transaction);
                }
            }
            else
            {
                _link = _link ?? new ModuleGalleryImageLink();
                _link.Text = Text;
                _link.Image = Image;
                _link.Position = Position;
                _link.ModuleGalleryImageId = ModuleGalleryImage.Id;
                _link.Save(transaction);

                Id = _link.Id;
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Link" + Position + "GalleryImage" + (!IsNewImage && ModuleGalleryImage != null ? ModuleGalleryImage.Id.ToString() : string.Empty);
        }
    }
}