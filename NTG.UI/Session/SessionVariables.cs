
using NTG.UI.Principal;
using System;
using System.Web;

namespace NTG.UI.Session
{
    public static class SessionVariables
    {
        public static NTGPrincipal User
        {
            get {
                try {
                    return (NTGPrincipal)HttpContext.Current.User;
                }
                catch(InvalidCastException)
                {
                    return null;
                }
            }
        }
    }
}