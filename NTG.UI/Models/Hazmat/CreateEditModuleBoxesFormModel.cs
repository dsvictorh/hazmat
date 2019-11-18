using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    public class CreateEditModuleBoxesFormModel : BaseFormModel<CreateEditModuleBoxesFormModel>
    {
        public int Id { get; set; }

        public int PageModuleId { get; private set; }
        public int Position { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public bool BlueTitle { get; set; }

        [Required]
        public string Text { get; set; }

        public List<ModuleBoxesBox> Boxes { get; set; }

        public PageModuleStates State { get; set; }

        public bool Theme { get; set; }

        public bool TransparentBackground { get; set; }

        public bool LootBox { get; set; }

        [Range(0, 100)]
        public int LootBoxTop { get; set; }

        [Range(0, 100)]
        public int LootBoxLeft { get; set; }

        [JsonIgnore]
        internal string PageName { get; set; }

        private ModuleBoxes _module;
        private PageModule _pageModule;
        private Page _page;

        protected override bool OnValidate()
        {
            _page = Page.Query.Include(nameof(Page.PageModules)).SingleOrDefault(p => p.Name.Equals(PageName, StringComparison.OrdinalIgnoreCase));
            if (_page == null)
            {
                AddMessage(Message.GLOBAL, new Message("Attempted to save module to non-existing page", MessageTypes.Error));
                return false;
            }

            if (_page.AdminLocked && !SessionVariables.User.IsInRole("Admin"))
            {
                AddMessage(Message.GLOBAL, new Message("You are not permitted to edit an admin locked page", MessageTypes.Error));
                NTGLogger.LogSecurityAction(HttpContext.Current.Request, SessionVariables.User, "Attempted to edit/create module in admin locked page" + _page.Id + " '" + _page.Name + "'");
                return false;
            }

            if (Id != 0)
            {
                _module = ModuleBoxes.Query.SingleOrDefault(mc => mc.Id == Id);
                if (_module == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module not found", MessageTypes.Error));
                    return false;
                }

                var moduleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_BOXES];
                _pageModule = PageModule.Query.SingleOrDefault(mp => mp.ModuleId == _module.Id && mp.ModuleTypeId == moduleTypeId && mp.PageId == _page.Id);
                if (_pageModule == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module does not belong to current page", MessageTypes.Error));
                    return false;
                }
            }

            if (Boxes == null || !Boxes.Any(b => !b.IsDelete))
            {
                AddMessage(nameof(State) + GetErrorMessageSufix(), new Message("Module Boxes requires at least one box", MessageTypes.Error));
                return false;
            }

            return true;
        }

        protected override void OnSubmit()
        {
            var isNew = _module == null;
            if (isNew)
            {
                _module = new ModuleBoxes();
                _pageModule = new PageModule();
                _pageModule.ModuleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_BOXES];
                _pageModule.PageId = _page.Id;
                _pageModule.Position = _page.PageModules.Count + 1;
                Position = _pageModule.Position;
            }
            
            _module.Text = Text;
            _pageModule.Title = Title;
            _pageModule.BlueTitle = BlueTitle;
            _pageModule.Theme = Theme;
            _pageModule.TransparentBackground = TransparentBackground;
            _pageModule.LootBox = LootBox;
            _pageModule.LootBoxLeft = LootBoxLeft;
            _pageModule.LootBoxTop = LootBoxTop;
            _pageModule.State = State;

            using (var conn = new NTGDBTransactional())
            {
                _module.Save(conn);
                
                var subFormSuccess = true;
                var subForm = new CreateEditModuleBoxesBoxSubFormModel();
                var positionReduction = 0;
                foreach (var box in Boxes.OrderBy(b => b.Position)) {
                    if (box.IsDelete)
                    {
                        positionReduction++;
                    }
                    else if(positionReduction > 0)
                    {
                        box.IsModified = true;
                        box.Position -= positionReduction;
                    }

                    if (box.IsModified || box.IsDelete)
                    {
                        subForm.Id = box.Id;
                        subForm.Title = box.Title;
                        subForm.Icon = box.Icon;
                        subForm.Color = box.Color;
                        subForm.Text = box.Text;
                        subForm.Url = box.Url;
                        subForm.Position = box.Position;
                        subForm.ModuleBoxes = _module;
                        subForm.IsDelete = box.IsDelete;
                        subForm.IsNewModule = Id == 0;
                        subForm.Submit(conn, Messages);

                        subFormSuccess = subFormSuccess && subForm.Success;
                        box.Id = subForm.Id;
                        box.ModuleBoxesId = subForm.ModuleBoxes.Id;
                        box.IsModified = false;
                    }
                }

                if (subFormSuccess)
                {
                    _pageModule.ModuleId = _module.Id;
                    _pageModule.Save(conn);

                    NTGLogger.LogSiteAction(HttpContext.Current.Request, 
                        SessionVariables.User, 
                        (isNew ? "Created" : "Editted") + " Module",
                        _page.Id,
                        _page.Name,
                        _module.Id, 
                        ModuleService.MODULE_BOXES,
                        conn);
                    conn.Commit();

                    Id = _module.Id;
                    PageModuleId = _pageModule.Id;
                    Boxes.RemoveAll(b => b.IsDelete);
                    ModuleService.RefreshCacheModule(_pageModule.Id);
                    AddMessage(Message.GLOBAL, new Message("Module saved", MessageTypes.Success));
                }
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Boxes" + (Id != 0 ? Id.ToString() : string.Empty);
        }
    }
}