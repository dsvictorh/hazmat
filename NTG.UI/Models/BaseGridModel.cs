using NTG.Logic;
using NTG.Logic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace NTG.UI.Models
{
    public class BaseGridModel<T> : BaseAjaxModel where T : BaseModel<T>
    {
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Rows { get; private set; }
        public int TotalCount { get; private set; }
        public int Pages { get; private set; }

        protected void SetGrid(IQueryable<T> query)
        {
            

            TotalCount = query.Count();
            Pages = (int)Math.Ceiling((decimal)TotalCount / PageSize);

            if (string.IsNullOrEmpty(SortColumn))
            {
                query = OrderByDefault(query)
                        .Skip(Page * PageSize - PageSize)
                        .Take(PageSize);
            }
            else
            {
                query = query
                        .OrderBy($"{SortColumn} {SortDirection}")
                        .Skip(Page * PageSize - PageSize)
                        .Take(PageSize);
            }

            Rows = SelectAfterFilter(query);
        }

        protected virtual IQueryable<T> OrderByDefault(IQueryable<T> query)
        {
            return query.OrderBy($"Id");
        }

        protected virtual IEnumerable<T> SelectAfterFilter(IEnumerable<T> list)
        {
            return list;
        }
    }
}