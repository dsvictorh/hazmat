using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTG.UI.Models.Hazmat
{
    public class PageModulesViewModel : BaseAjaxModel
    {
        public Page Page { get; set; }
        public IReadOnlyCollection<object> Modules { get; set; }

        public void GetModules(string pageName)
        {
            Page = Page.Query.SingleOrDefault(p => p.Name.Equals(pageName, StringComparison.OrdinalIgnoreCase));
            if (Page == null)
            {
                AddMessage(Message.GLOBAL, new Message("Page does not exist", MessageTypes.Error));
            }

            Modules = ModuleService.GetModulesFromCache(Page.Id);
        }
    }
}