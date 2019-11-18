using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace NTG.UI.Models.Hazmat
{
    public class ModuleAction
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public bool Delete { get; set; }
    };

    public class CreateEditPageFormModel : BaseFormModel<CreateEditPageFormModel>
    {     
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-z\\s]+$", ErrorMessageResourceName = "RegularExpressionAttributeAlpha")]
        [MaxLength(20)]
        public string Name { get; set; }

        public string HeroImage { get; set; }

        public string MobileHeroImage { get; set; }

        public bool InMenu { get; set; }

        public bool InBottomMenu { get; set; }

        public bool Visible { get; set; }

        public bool? AdminLocked { get; set; }

        public List<ModuleAction> ModuleActions {get; set;}

        public string RedirectTo { get; private set; }

        [JsonIgnore]
        internal string PageName { get; set; }

        private Page _page;

        protected override bool OnValidate()
        {
            if (Name.ToLower().Equals("create"))
            {
                AddMessage(Message.GLOBAL, new Message("Non-valid page name: Create", MessageTypes.Error));
                return false;
            }

            _page = Page.Query.Include(nameof(Page.PageModules)).SingleOrDefault(p => p.Name == PageName || p.Id == Id);
            if (_page != null && _page.Id != Id)
            {
                AddMessage(Message.GLOBAL, new Message("Page with name " + Name + " already exists", MessageTypes.Error));
                return false;
            }

            if (AdminLocked.HasValue && !SessionVariables.User.IsInRole("Admin"))
            {
                AddMessage(Message.GLOBAL, new Message("You are not permitted to edit the admin lock on pages", MessageTypes.Error));
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, SessionVariables.User, "Attempted to edit admin lock on " + _page.Id + " '" + _page.Name + "'");
                return false;
            }

            if (Id != 0)
            {
                if (_page == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Page does not exist", MessageTypes.Error));
                    return false;
                }

                if (_page.AdminLocked && !SessionVariables.User.IsInRole("Admin"))
                {
                    AddMessage(Message.GLOBAL, new Message("You are not permitted to edit an admin locked page", MessageTypes.Error));
                    NTGLogger.LogSecurityAction(HttpContext.Current.Request, SessionVariables.User, "Attempted to edit admin locked page " + _page.Id + " '" + _page.Name + "'");
                    return false;
                }

                if (ModuleActions != null)
                {
                    if (ModuleActions.GroupBy(mp => mp.Position).Any(mp => mp.Count() > 1))
                    {
                        AddMessage(Message.GLOBAL, new Message("Duplicate module positions are not allowed", MessageTypes.Error));
                        return false;
                    }

                    if (ModuleActions.Any(mp => mp.Position < 1 || mp.Position > _page.PageModules.Count()))
                    {
                        AddMessage(Message.GLOBAL, new Message("A module has a non-valid position", MessageTypes.Error));
                        return false;
                    }
                }
            }            
            
            return true;
        }

        protected override void OnSubmit()
        {
            var isNew = _page == null;
            if (isNew)
            {
                _page = new Page();
                _page.UploadFolder = Path.GetRandomFileName();
                _page.Position = Page.Query.Count() + 1;
            }

            using (var conn = new NTGDBTransactional())
            {
                if (_page.Id == 0 || !_page.Name.Equals(Name))
                {
                    RedirectTo = Name.Replace(" ", "-").ToLower();
                }

                _page.Name = Name;
                _page.HeroImage = HeroImage;
                _page.MobileHeroImage = !string.IsNullOrEmpty(HeroImage) ? MobileHeroImage : null;
                /*The page with position 1 is the equivalent of home and should always have
                 InMenu and as false because it has its own logic to appear in the top menu as the home link*/
                _page.InMenu = _page.Position == 1 ? false : InMenu;
                _page.InBottomMenu = _page.Position == 1 ? false : InBottomMenu;
                //The home page cannot be inactive
                _page.Visible = _page.Position == 1 ? true : Visible;

                if (AdminLocked.HasValue)
                {
                    _page.AdminLocked = AdminLocked.Value;
                    if (!isNew)
                    {
                        NTGLogger.LogSiteAction(HttpContext.Current.Request,
                        SessionVariables.User,
                       "Changed admin lock on page",
                        _page.Id,
                        _page.Name,
                        null,
                        null,
                        conn);
                    }
                }
                
                _page.Save(conn);

                foreach (var module in _page.PageModules.OrderBy(pm => ModuleActions.OrderBy(ma => ma.Position).Select(ma => ma.Id).ToList().IndexOf(pm.Id)))
                {
                    var action = ModuleActions.SingleOrDefault(ma => ma.Id == module.Id);
                    if (action != null)
                    {
                        if (action.Delete)
                        {
                            ModuleService.DeleteModule(module, conn);
                            foreach (var nextAction in ModuleActions.Where(ma => ma.Position > action.Position))
                            {
                                nextAction.Position -= 1;
                            }
                            NTGLogger.LogSiteAction(HttpContext.Current.Request,
                            SessionVariables.User,
                            "Deleted Module",
                            _page.Id,
                            _page.Name,
                            module.ModuleId,
                            ModuleType.Query.FirstOrDefault(mt => mt.Id == module.ModuleTypeId)?.Name,
                            conn);
                        }
                        else if(module.Position != action.Position)
                        {
                            module.Position = action.Position;
                            module.Save(conn);
                        }
                    }
                }

                NTGLogger.LogSiteAction(HttpContext.Current.Request,
                        SessionVariables.User,
                        (isNew ? "Created" : "Editted") + " Page",
                        _page.Id,
                        _page.Name,
                        null,
                        null,
                        conn);

                conn.Commit();
                ModuleService.RefreshModulesCache(_page.Id);
                AddMessage(Message.GLOBAL, new Message("Page " + Name + " successfully saved", MessageTypes.Success));
            }
        }
    }
}