using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Models.Hazmat
{
    public class LootBoxInfoViewModel : BaseAjaxModel
    {
        public string ImageUrl { get; private set; }
        public string Title { get; private set; }
        public string Text { get; private set; }

        public void GetInfo()
        {
            var siteSettings = GlobalVariables.SiteSettings;
            if (siteSettings != null)
            {
                ImageUrl = siteSettings.LootBoxImageUrl;
                Title = siteSettings.LootBoxPopupTitle;
                Text = siteSettings.LootBoxPopupText;
            }
        }
    }
}