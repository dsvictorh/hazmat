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
    

    public class CreateEditModuleSimpleCardsCardSubFormModel : BaseSubFormModel<CreateEditModuleSimpleCardsCardSubFormModel>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string Subtitle { get; set; }
        
        public int Position { get; set; }

        [Required]
        public string Image { get; set; }

        public ModuleSimpleCards ModuleSimpleCards { get; set; }

        public bool IsDelete { get; set; }

        public bool IsNewModule { get; set; }

        [JsonIgnore]
        internal string PageName { get; set; }

        private ModuleSimpleCardsCard _card;
        

        protected override bool OnValidate()
        {
            if (ModuleSimpleCards == null)
            {
                AddMessage(nameof(Title) + GetErrorMessageSufix(), new Message("Attempted to save card to non-existing simple cards module", MessageTypes.Error));
                return false;
            }

            _card = ModuleSimpleCardsCard.Query.SingleOrDefault(mbb => mbb.Id == Id && mbb.ModuleSimpleCardsId == ModuleSimpleCards.Id);
            if (Id != 0 && _card == null)
            {
                AddMessage(nameof(Title) + GetErrorMessageSufix(), new Message("Card does not belong to current simple cards module or does not exist", MessageTypes.Error));
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
                    _card.Delete(transaction);
                }
            }
            else
            {
                _card = _card ?? new ModuleSimpleCardsCard();
                _card.Title = Title;
                _card.Subtitle = Subtitle;
                _card.Image = Image;
                _card.Position = Position;
                _card.ModuleSimpleCardsId = ModuleSimpleCards.Id;
                _card.Save(transaction);

                Id = _card.Id;
            }
            
        }

        protected override string GetErrorMessageSufix()
        {
            return "Card" + Position + "SimpleCards" + (!IsNewModule && ModuleSimpleCards != null ? ModuleSimpleCards.Id.ToString() : string.Empty);
        }
    }
}