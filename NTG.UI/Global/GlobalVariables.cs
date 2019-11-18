using NTG.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTG.UI.Global
{
    public static class GlobalVariables
    {
        public static SiteSettings SiteSettings
        {
            get {
                if (HttpContext.Current.Application["SiteSettings"] == null)
                {
                    HttpContext.Current.Application["SiteSettings"] = SiteSettings.Query.FirstOrDefault();
                }

                return HttpContext.Current.Application["SiteSettings"] as SiteSettings;
            }
        }
    }
}