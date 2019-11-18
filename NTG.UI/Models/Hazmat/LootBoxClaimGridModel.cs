using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Models.Hazmat
{
    public class LootBoxClaimGridModel : BaseGridModel<LootBoxClaim>
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool Redeemed { get; set; }

        public void Load() {
            var query = LootBoxClaim.Query as IQueryable<LootBoxClaim>;
            query = query.Where(lbc => lbc.Redeemed == Redeemed);

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(lbc => lbc.Email.Contains(Email));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                query = query.Where(lbc => lbc.FirstName.Contains(Name) || lbc.LastName.Contains(Name) || (lbc.FirstName + " " + lbc.LastName).Contains(Name));
            }

            if (DateFrom.HasValue)
            {
                var utcFrom = DateFrom.Value.ToUniversalTime();
                query = query.Where(lbc => lbc.Date >= utcFrom);
            }

            if (DateTo.HasValue)
            {
                var utcTo = DateTo.Value.ToUniversalTime();
                query = query.Where(lbc => lbc.Date <= utcTo);
            }

            SetGrid(query);
        }
    }
}