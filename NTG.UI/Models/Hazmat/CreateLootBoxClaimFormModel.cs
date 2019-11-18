using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace NTG.UI.Models.Hazmat
{
    public class CreateLootBoxClaimFormModel : BaseFormModel<CreateLootBoxClaimFormModel>
    {     
        public int Id { get; private set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public int PageModuleId { get; set; }

        public bool Claimed { get; private set; }

        private PageModule _pageModule;

        protected override bool OnValidate()
        {
            _pageModule = PageModule.Query.SingleOrDefault(pm => pm.Id == PageModuleId);
            if (_pageModule == null)
            {
                AddMessage(Message.GLOBAL, new Message("We know this loot box doesn't exist! Stop trying to be the smart guy...", MessageTypes.Error));
                return false;
            }

            if (!_pageModule.LootBox)
            {
                Claimed = true;
                AddMessage(Message.GLOBAL, new Message("Oh no! It seems someone already claimed this loot box. Better luck next time.", MessageTypes.Warning));
                return false;
            }
            
            return true;
        }

        protected override void OnSubmit()
        {
            using (var conn = new NTGDBTransactional())
            {
                var claim = new LootBoxClaim();
                claim.Email = Email;
                claim.FirstName = FirstName;
                claim.LastName = LastName;
                claim.Date = DateTime.UtcNow;
                claim.Save(conn);

                _pageModule.LootBox = false;
                _pageModule.Save(conn);
                conn.Commit();

                AddMessage(Message.GLOBAL, new Message("Congratulations!!! You have claimed this loot box!", MessageTypes.Success));
            }
        }
    }
}