using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    

    public class CreateEditModuleBoxesBoxSubFormModel : BaseSubFormModel<CreateEditModuleBoxesBoxSubFormModel>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Title { get; set; }

        [Required]
        [MaxLength(8)]
        public string Icon { get; set; }

        [Required]
        [MaxLength(20)]
        public string Color { get; set; }

        [Required]
        [MaxLength(95)]
        public string Text { get; set; }

        public string Url { get; set; }

        public int Position { get; set; }

        public ModuleBoxes ModuleBoxes { get; set; }

        public bool IsDelete { get; set; }

        public bool IsNewModule { get; set; }

        [JsonIgnore]
        internal string PageName { get; set; }

        private ModuleBoxesBox _box;
        

        protected override bool OnValidate()
        {
            if (ModuleBoxes == null)
            {
                AddMessage(nameof(Title) + GetErrorMessageSufix(), new Message("Attempted to save box to non-existing boxes module", MessageTypes.Error));
                return false;
            }

            _box = ModuleBoxesBox.Query.SingleOrDefault(mbb => mbb.Id == Id && mbb.ModuleBoxesId == ModuleBoxes.Id);
            if (Id != 0 && _box == null)
            {
                AddMessage(nameof(Title) + GetErrorMessageSufix(), new Message("Box does not belong to current box module or does not exist", MessageTypes.Error));
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
                    _box.Delete(transaction);
                }
            }
            else
            {
                _box = _box ?? new ModuleBoxesBox();
                _box.Title = Title;
                _box.Icon = Icon;
                _box.Color = Color;
                _box.Text = Text;
                _box.Url = Url;
                _box.Position = Position;
                _box.ModuleBoxesId = ModuleBoxes.Id;
                _box.Save(transaction);

                Id = _box.Id;
            }
            
        }

        protected override string GetErrorMessageSufix()
        {
            return "Box" + Position + "Boxes" + (!IsNewModule && ModuleBoxes != null ? ModuleBoxes.Id.ToString() : string.Empty);
        }
    }
}