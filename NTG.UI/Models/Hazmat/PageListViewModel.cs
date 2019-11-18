using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Models.Hazmat
{
    public class PageListViewModel : BaseAjaxModel
    {
        public List<object> Pages { get; set; }

        public void GetPages()
        {
            var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Pages = new List<object>();
            foreach (var page in Page.Query.OrderBy(p => p.Position))
            {
                Pages.Add(new
                {
                    Id = page.Id,
                    Url = helper.Action("Index", "Page", new { pageName = page.Name.Replace(" ", "-").ToLower() }),
                    Name = page.Name,
                    Position = page.Position,
                    Visible = page.Visible,
                    InMenu = page.InMenu,
                    InBottomMenu = page.InBottomMenu,
                    AdminLocked = page.AdminLocked,
                });
            }
        }
    }
}