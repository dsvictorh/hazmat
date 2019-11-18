using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Principal;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    public class EditLootBoxClaimFormModel : BaseFormModel<EditLootBoxClaimFormModel>
    {
        public int Id { get; set; }

        public string Prize { get; set; }

        public bool Redeemed { get; set; }

        private LootBoxClaim _lootBoxClaim { get; set; }

        protected override bool OnValidate()
        {

            _lootBoxClaim = LootBoxClaim.Query.SingleOrDefault(r => r.Id == Id);
            if (_lootBoxClaim == null)
            {
                AddMessage(Message.GLOBAL, new Message("Loot Box Claim does not exist", MessageTypes.Error));
                return false;
            }

            return true;
        }

        protected override void OnSubmit()
        {
            _lootBoxClaim.Prize = Prize;
            _lootBoxClaim.Redeemed = Redeemed;
            _lootBoxClaim.Save();

            AddMessage(Message.GLOBAL, new Message("Loot Box Claim successfully created", MessageTypes.Success));
        }
    }
}