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
    public class CreateEditModuleGalleryImageSubFormModel : BaseSubFormModel<CreateEditModuleGalleryImageSubFormModel>
    {
        public int Id { get; set; }

        public List<ModuleGalleryImageLink> Links { get; set; }

        [Required]
        public string Image { get; set; }

        public int Position { get; set; }

        public ModuleGallery ModuleGallery { get; set; }

        public bool IsDelete { get; set; }

        public bool IsNewModule { get; set; }

        private ModuleGalleryImage _image;

        protected override bool OnValidate()
        {
            if (ModuleGallery == null)
            {
                AddMessage(nameof(Image) + GetErrorMessageSufix(), new Message("Attempted to save image to non-existing gallery module", MessageTypes.Error));
                return false;
            }

            _image = ModuleGalleryImage.Query.SingleOrDefault(mgi => mgi.Id == Id && mgi.ModuleGalleryId == ModuleGallery.Id);
            if (Id != 0 && _image == null)
            {
                AddMessage(nameof(Image) + GetErrorMessageSufix(), new Message("Image does not belong to current gallery module or does not exist", MessageTypes.Error));
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
                    foreach (var link in ModuleGalleryImageLink.Query.Where(mgil => mgil.ModuleGalleryImageId == _image.Id))
                    {
                        link.Delete(transaction);
                    }
                    _image.Delete(transaction);
                }
            }
            else
            {
                _image = _image ?? new ModuleGalleryImage();
                _image.Image = Image;
                _image.Position = Position;
                _image.ModuleGalleryId = ModuleGallery.Id;
                _image.Save(transaction);

                var subFormSuccess = true;
                if (Links != null)
                {
                    var subForm = new CreateEditModuleGalleryImageLinkSubFormModel();
                    var positionReduction = 0;
                    foreach (var link in Links.OrderBy(l => l.Position))
                    {
                        if (link.IsDelete)
                        {
                            positionReduction++;
                        }
                        else if(positionReduction > 0)
                        {
                            link.IsModified = true;
                            link.Position -= positionReduction;
                        }

                        if (link.IsModified || link.IsDelete)
                        {
                            subForm.Id = link.Id;
                            subForm.Text = link.Text;
                            subForm.Image = link.Image;
                            subForm.Position = link.Position;
                            subForm.ModuleGalleryImage = _image;
                            subForm.IsDelete = link.IsDelete;
                            subForm.IsNewImage = Id == 0;
                            subForm.Submit(transaction, Messages);

                            subFormSuccess = subFormSuccess && subForm.Success;
                            link.Id = subForm.Id;
                            link.ModuleGalleryImageId = subForm.ModuleGalleryImage.Id;
                            link.IsModified = false;
                        }
                    }

                }

                if (subFormSuccess)
                {
                    Id = _image.Id;
                    if(Links != null)
                    {
                        Links.RemoveAll(l => l.IsDelete);
                    }
                }
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Image" + Position + "Gallery" + (!IsNewModule && ModuleGallery != null ? ModuleGallery.Id.ToString() : string.Empty);
        }
    }
}