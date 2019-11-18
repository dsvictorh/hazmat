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
    public class CreateEditModuleProfileCardsCardSubFormModel : BaseSubFormModel<CreateEditModuleProfileCardsCardSubFormModel>
    {
        public int Id { get; set; }

        public List<ModuleProfileCardsCardLink> Links { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        public string FacebookUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string TwitchUrl { get; set; }

        public string InstagramUrl { get; set; }

        public string YouTubeUrl { get; set; }

        public int Position { get; set; }

        public ModuleProfileCards ModuleProfileCards { get; set; }

        public bool IsDelete { get; set; }

        public bool IsNewModule { get; set; }

        private ModuleProfileCardsCard _card;

        protected override bool OnValidate()
        {
            if (ModuleProfileCards == null)
            {
                AddMessage(nameof(Name) + GetErrorMessageSufix(), new Message("Attempted to save card to non-existing profile cards module", MessageTypes.Error));
                return false;
            }

            _card = ModuleProfileCardsCard.Query.SingleOrDefault(mbb => mbb.Id == Id && mbb.ModuleProfileCardsId == ModuleProfileCards.Id);
            if (Id != 0 && _card == null)
            {
                AddMessage(nameof(Name) + GetErrorMessageSufix(), new Message("Card does not belong to current profile cards module or does not exist", MessageTypes.Error));
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
                    foreach (var link in ModuleProfileCardsCardLink.Query.Where(mpccl => mpccl.ModuleProfileCardsCardId == _card.Id))
                    {
                        link.Delete(transaction);
                    }
                    _card.Delete(transaction);
                }
            }
            else
            {
                _card = _card ?? new ModuleProfileCardsCard();
                _card.Image = Image;
                _card.Name = Name;
                _card.FacebookUrl = FacebookUrl;
                _card.TwitterUrl = TwitterUrl;
                _card.TwitchUrl = TwitchUrl;
                _card.InstagramUrl = InstagramUrl;
                _card.YouTubeUrl = YouTubeUrl;
                _card.Position = Position;
                _card.ModuleProfileCardsId = ModuleProfileCards.Id;
                _card.Save(transaction);

                var subFormSuccess = true;
                if (Links != null)
                {
                    var subForm = new CreateEditModuleProfileCardsCardLinkSubFormModel();
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
                            subForm.Url = link.Url;
                            subForm.Position = link.Position;
                            subForm.ModuleProfileCardsCard = _card;
                            subForm.IsNewCard = Id == 0;
                            subForm.IsDelete = link.IsDelete;
                            subForm.Submit(transaction, Messages);

                            subFormSuccess = subFormSuccess && subForm.Success;
                            link.Id = subForm.Id;
                            link.ModuleProfileCardsCardId = subForm.ModuleProfileCardsCard.Id;
                            link.IsModified = false;
                        }
                    }

                }

                if (subFormSuccess)
                {
                    Id = _card.Id;
                    if(Links != null)
                    {
                        Links.RemoveAll(l => l.IsDelete);
                    }
                }
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Card" + Position + "ProfileCards" + (!IsNewModule && ModuleProfileCards != null ? ModuleProfileCards.Id.ToString() : string.Empty);
        }
    }
}