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
    public class SiteSettingsViewModel : BaseAjaxModel
    {
        public SiteSettings SiteSettings { get; set; }

        public void GetSettings()
        {
            SiteSettings = GlobalVariables.SiteSettings;
        }
    }
}