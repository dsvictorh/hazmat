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
    public class SiteLogsGridModel : BaseGridModel<SiteLog>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string IPAddress { get; set; }
        public string Admin { get; set; }
        public string Action { get; set; }
        public string SitePage { get; set; }

        public void Load() {
            var query = SiteLog.Query.Include(sl => sl.Admin) as IQueryable<SiteLog>;

            if (!string.IsNullOrEmpty(Admin))
            {
                query = query.Where(el => el.Admin.Email.Contains(Admin));
            }

            if (!string.IsNullOrEmpty(IPAddress))
            {
                query = query.Where(el => el.IPAddress.Contains(IPAddress));
            }

            if (!string.IsNullOrEmpty(Action))
            {
                query = query.Where(el => el.Action.Contains(Action));
            }

            if (!string.IsNullOrEmpty(SitePage))
            {
                query = query.Where(el => el.Page.Contains(SitePage));
            }

            if (DateFrom.HasValue)
            {
                var utcFrom = DateFrom.Value.ToUniversalTime();
                query = query.Where(el => el.Date >= utcFrom);
            }

            if (DateTo.HasValue)
            {
                var utcTo = DateTo.Value.ToUniversalTime();
                query = query.Where(el => el.Date <= utcTo);
            }

            SetGrid(query);
        }

        protected override IEnumerable<SiteLog> SelectAfterFilter(IEnumerable<SiteLog> list)
        {
            return list.Select(sl => sl.StripAdminPassword());
        }
    }
}