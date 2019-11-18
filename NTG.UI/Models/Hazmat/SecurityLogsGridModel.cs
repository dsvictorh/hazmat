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
    public class SecurityLogsGridModel : BaseGridModel<SecurityLog>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string IPAddress { get; set; } 
        public string Action { get; set; }
        public string UserEmail { get; set; }

        public void Load() {
            var query = SecurityLog.Query as IQueryable<SecurityLog>;

            if (!string.IsNullOrEmpty(UserEmail))
            {
                query = query.Where(sl => sl.UserEmail.Contains(UserEmail));
            }

            if (!string.IsNullOrEmpty(IPAddress))
            {
                query = query.Where(sl => sl.IPAddress.Contains(IPAddress));
            }

            if (!string.IsNullOrEmpty(Action))
            {
                query = query.Where(sl => sl.Action.Contains(Action));
            }

            if (DateFrom.HasValue)
            {
                var utcFrom = DateFrom.Value.ToUniversalTime();
                query = query.Where(sl => sl.Date >= utcFrom);
            }

            if (DateTo.HasValue)
            {
                var utcTo = DateTo.Value.ToUniversalTime();
                query = query.Where(el => el.Date <= utcTo);
            }

            SetGrid(query);
        }
    }
}