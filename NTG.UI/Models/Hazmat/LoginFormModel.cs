using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.UI.Loggers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    public class LoginFormModel : BaseFormModel<LoginFormModel>
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Persist { get; set; }

        public string ReturnUrl { get; set; }

        private string _roles;
        private Admin _admin;

        protected override bool OnValidate()
        {
            var password = EncryptionService.Encrypt(Password);
            Password = string.Empty;

            _admin = Admin.Query.Include(nameof(Admin.Role)).SingleOrDefault(a => a.Email == Email && a.Password == password && a.Active);
            if (_admin == null)
            {
                AddMessage(Message.GLOBAL, new Message("Invalid email or password", MessageTypes.Warning));
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, "Failed login attempt", null, Email, "Hazmat");
                return false;
            }

            _roles = _admin.Role.Name + ",Hazmat";
            return true;
        }

        protected override void OnSubmit()
        {
            var now = DateTime.UtcNow;
            var authTicket = new FormsAuthenticationTicket(
                1,
                Email,
                now,
                now.AddYears(100),
                Persist,
                "Hazmat",
                FormsAuthentication.FormsCookiePath
            );

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,  FormsAuthentication.Encrypt(authTicket));

            if (authTicket.IsPersistent) {
                cookie.Expires = now.AddDays(30);
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
            NTGLogger.LogSecurityAction(HttpContext.Current.Request, "Successful login", _admin.Id, _admin.Email, _roles);
        }
    }
}