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
    public class CreateEditModuleProfileCardsCardLinkSubFormModel : BaseSubFormModel<CreateEditModuleProfileCardsCardLinkSubFormModel>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Text { get; set; }

        [Required]
        public string Url { get; set; }

        public int Position { get; set; }

        public ModuleProfileCardsCard ModuleProfileCardsCard { get; set; }

        public bool IsDelete { get; set; }

        public bool IsNewCard { get; set; }

        private ModuleProfileCardsCardLink _link;

        protected override bool OnValidate()
        {
            if (ModuleProfileCardsCard == null)
            {
                AddMessage(nameof(Text) + GetErrorMessageSufix(), new Message("Attempted to save link to non-existing card", MessageTypes.Error));
                return false;
            }

            _link = ModuleProfileCardsCardLink.Query.SingleOrDefault(mbb => mbb.Id == Id && mbb.ModuleProfileCardsCardId == ModuleProfileCardsCard.Id);
            if (Id != 0 && _link == null)
            {
                AddMessage(nameof(Text) + GetErrorMessageSufix(), new Message("Link does not belong to current card or does not exist", MessageTypes.Error));
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
                _link = _link ?? new ModuleProfileCardsCardLink();
                _link.Text = Text;
                _link.Url = Url;
                _link.Position = Position;
                _link.ModuleProfileCardsCardId = ModuleProfileCardsCard.Id;
                _link.Save(transaction);

                Id = _link.Id;
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Link" + Position + "ProfileCardsCard" + (!IsNewCard && ModuleProfileCardsCard != null  ? ModuleProfileCardsCard.Id.ToString() : string.Empty);
        }
    }
}