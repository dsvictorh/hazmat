using NTG.Logic.Models;
using NTG.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NTG.UI.Models.Hazmat
{
    public class RoleListViewModel : BaseAjaxModel
    {
        public List<Role> Roles { get; set; }

        public void GetRoles()
        {
            Roles = Role.Query.ToList();
        }
    }
}