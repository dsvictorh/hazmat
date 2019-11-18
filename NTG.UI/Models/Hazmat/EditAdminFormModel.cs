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
    public class EditAdminFormModel : BaseFormModel<EditAdminFormModel>
    {
        public string User { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [RegularExpression("^[A-z1-9¿?¡!_.@$\\-\\+]+$", ErrorMessageResourceName = "RegularExpressionAttributePassword")]
        public string Password { get; set; }

        public string VerifyPassword { get; set; }
        public int? RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public bool? Active { get; set; }

        public bool Relog { get; private set; }

        private Admin _admin { get; set; }
        private Role _role { get; set; }

        private NTGPrincipal _sessionUser { get; set; }

        protected override bool OnValidate()
        {
            _sessionUser = SessionVariables.User;
            var valid = true;

            _admin = Admin.Query.Include(nameof(Admin.Role)).SingleOrDefault(a => a.Email == User);
            if (_admin == null)
            {
                AddMessage(Message.GLOBAL, new Message("User " + User + " does not exist", MessageTypes.Error));
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to edit non-existing admin '" + User + "'");
                return false;
            }

            if (_sessionUser.IsInRole("Admin"))
            {
                if (!_admin.Email.Equals(_sessionUser.Email) && _admin.Role.Name.Equals("Admin"))
                {
                    AddMessage(Message.GLOBAL, new Message("You are not permitted to modify other users with the Admin role", MessageTypes.Error));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to edit admin " + _admin.Id + " '" + _admin.Email + "'");
                    return false;
                }

                if (RoleId.HasValue && _admin.RoleId != RoleId)
                {
                    _role = Role.Query.SingleOrDefault(r => r.Id == RoleId.Value);
                    if (_role == null)
                    {
                        AddMessage(nameof(RoleId), new Message("Attempted to assign non-existing role", MessageTypes.Error));
                        NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to assign non-existing role to admin " + _admin.Id + " '" + _admin.Email + "'");
                        return false;
                    }
                }
            }
            else
            {
                if (!_admin.Active)
                {
                    AddMessage(Message.GLOBAL, new Message("Unable to save changes: user is no longer active", MessageTypes.Error));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to save changes to inactive admin " + _admin.Id + " '" + _admin.Email + "'");
                    return false;
                }

                if (!_admin.Email.Equals(_sessionUser.Email))
                {
                    AddMessage(Message.GLOBAL, new Message("You are not permitted to modify other users", MessageTypes.Error));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to modify admin " + _admin.Id + " '" + _admin.Email + "'");
                    return false;
                }

                if (Active.HasValue)
                {
                    AddMessage(Message.GLOBAL, new Message("You are not permitted to change activation of users", MessageTypes.Error));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to reactivate/deactivate admin " + _admin.Id + " '" + _admin.Email + "'");
                    return false;
                }

                if (RoleId.HasValue)
                {
                    AddMessage(Message.GLOBAL, new Message("You are not permitted to change the role of users", MessageTypes.Error));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Attempted to change role of admin " + _admin.Id + " '" + _admin.Email + "'");
                    return false;
                }
            }

            if (Admin.Query.Any(a => a.Email == Email && a.Id != _admin.Id))
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
            string oldRole = null;
            var emailChanged = !_admin.Email.Equals(Email);
            var passwordChanged = !string.IsNullOrEmpty(Password);
            var activeChanged = Active.HasValue && _admin.Active != Active.Value;

            _admin.FirstName = FirstName;
            _admin.LastName = LastName;

            using (var conn = new NTGDBTransactional())
            {
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Editted user " + _admin.Id + " '" + _admin.Email + "'", conn);

                if (activeChanged)
                {
                    _admin.Active = Active.Value;
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, (_admin.Active ? "Reactivated" : "Deactivated") + " admin " + _admin.Id + " '" + _admin.Email + "'", conn);
                }

                if (emailChanged)
                {
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Changed email of admin " + _admin.Id + " '" + _admin.Email + "': " + _admin.Email + " to " + Email);
                    _admin.Email = Email;
                }

                if (passwordChanged)
                {
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Changed password of admin " + _admin.Id + " '" + _admin.Email + "'", conn);
                    var encryptedPassword = EncryptionService.Encrypt(Password);
                    _admin.Password = encryptedPassword;
                    Password = string.Empty;
                    VerifyPassword = string.Empty;
                }

                if (_role != null)
                {
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, _sessionUser, "Changed role of admin " + _admin.Id + " '" + _admin.Email + "': " + oldRole + " to " + _role.Name, conn);
                    oldRole = _admin.Role.Name;
                    _admin.RoleId = _role.Id;
                }

                _admin.Save(conn);
                conn.Commit();
            }
            

            Relog = User == _sessionUser.Email && (emailChanged || passwordChanged || activeChanged);
            if (Relog)
            {
                FormsAuthentication.SignOut();
                if (activeChanged)
                {
                    AddMessage(Message.GLOBAL, new Message("User has been deactivated. Good Bye!", MessageTypes.Success));
                }
                else
                {
                    AddMessage(Message.GLOBAL, new Message("User access information changed. Please log in again", MessageTypes.Success));
                }
            }

            AddMessage(Message.GLOBAL, new Message("Changes successfully saved", MessageTypes.Success));
        }
    }
}