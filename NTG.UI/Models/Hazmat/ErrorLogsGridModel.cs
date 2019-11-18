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
    public class ErrorLogsGridModel : BaseGridModel<ErrorLog>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string IPAddress { get; set; }
        public string Message { get; set; }

        public void Load() {
            var query = ErrorLog.Query as IQueryable<ErrorLog>;

            if (!string.IsNullOrEmpty(IPAddress))
            {
                query = query.Where(el => el.IPAddress.Contains(IPAddress));
            }

            if (!string.IsNullOrEmpty(Message))
            {
                query = query.Where(el => el.Message.Contains(Message) || el.InnerMessage.Contains(Message));
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
    }
}