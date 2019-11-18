using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Principal;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    public class CreateAdminFormModel : BaseFormModel<CreateAdminFormModel>
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[A-z1-9¿?¡!_.@$\\-\\+]+$", ErrorMessageResourceName = "RegularExpressionAttributePassword")]
        public string Password { get; set; }

        public string VerifyPassword { get; set; }

        [Required]
        public int? RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        private Role _role { get; set; }

        private NTGPrincipal _sessionUser { get; set; }

        protected override bool OnValidate()
        {
            _sessionUser = SessionVariables.User;
            var valid = true;


            _role = Role.Query.SingleOrDefault(r => r.Id == RoleId);
            if (_role == null)
            {
                AddMessage(nameof(RoleId), new Message("Role does not exist", MessageTypes.Error));
                return false;
            }

            if (Admin.Query.Any(a => a.Email == Email))
            {
                AddMessage(nameof(Email), new Message("Email is already in use", MessageTypes.Warning));
                valid = false;
            }

            if (!string.IsNullOrEmpty(Password) && !Password.Equals(VerifyPassword))
            {
                AddMessage(nameof(Password), new Message("Passwords do not match", MessageTypes.Warning));
                valid = false;
            }

            return valid;
        }

        protected override void OnSubmit()
        {
            var admin = new Admin()
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = EncryptionService.Encrypt(Password),
                RoleId = _role.Id,
                Active = true
            };

            using (var conn = new NTGDBTransactional())
            {
                admin.Save(conn);
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Created user " + admin.Id + " '" + admin.Email + "' with role " + _role.Name, conn);
                conn.Commit();
            }

            AddMessage(Message.GLOBAL, new Message("User successfully created", MessageTypes.Success));
        }
    }
}