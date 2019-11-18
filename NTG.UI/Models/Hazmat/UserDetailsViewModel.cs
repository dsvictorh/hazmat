using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.UI.Session;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    public class UserDetailsViewModel : BaseAjaxModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public void GetUser()
        {
            var sessionUser = SessionVariables.User;
            if (sessionUser != null && sessionUser.IsHazmat)
            {
                var user = Admin.Query.Include(nameof(Admin.Role)).SingleOrDefault(a => a.Id == sessionUser.Id && a.Active);
                if (user != null)
                {
                    Id = user.Id;
                    Email = user.Email;
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    Role = user.Role.Name;
                }
            }
            
        }
    }
}