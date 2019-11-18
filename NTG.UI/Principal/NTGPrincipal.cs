using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace NTG.UI.Principal
{
    public class NTGPrincipal : GenericPrincipal
    {
        public int Id { get; private set; }
        public string Email { get; private set; }

        public string[] Roles { get; private set; }
        public bool IsHazmat { get; private set; }

        public NTGPrincipal(IIdentity identity, string[] roles, int id, string email, bool isHazmat) : base(identity, roles)
        {
            Roles = roles;
            Id = id;
            Email = email;
            IsHazmat = isHazmat;
        }

        public new bool IsInRole(string role)
        {
            //return (principal.IsInRole(role));
            if (Roles == null)
                return false;
            return Roles.Contains(role);
        }
    }
}