using NTG.UI.Principal;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

namespace NTG.UI.Extensions
{
    public static class NTGPrincipalExtensions
    {
        public static bool IsInRoles(this NTGPrincipal principal, string[] roles)
        {
            var user = SessionVariables.User;
            if (user == null)
            {
                return false;
            }

            return user.Roles.Any(r => roles.Contains(r));
        }
    }
}