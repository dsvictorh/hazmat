using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Models.Hazmat
{
    public class AdminListGridModel : BaseGridModel<Admin>
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int? RoleId { get; set; }
        public bool? Status { get; set; }

        public void Load() {
            var query = Admin.Query.Include(a => a.Role) as IQueryable<Admin>;

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(a => a.Email.Contains(Email));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                query = query.Where(a => a.FirstName.Contains(Name) || a.LastName.Contains(Name) || (a.FirstName + " " + a.LastName).Contains(Name));
            }

            if (RoleId.HasValue)
            {
                query = query.Where(a => a.RoleId == RoleId);
            }

            if (Status.HasValue)
            {
                query = query.Where(a => a.Active == Status);
            }

            SetGrid(query);
        }

        protected override IEnumerable<Admin> SelectAfterFilter(IEnumerable<Admin> list)
        {
            return list.Select(a => a.StripPassword());
        }
    }
}