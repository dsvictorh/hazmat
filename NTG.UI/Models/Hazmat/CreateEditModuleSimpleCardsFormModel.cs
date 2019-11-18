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
    public class CreateEditModuleSimpleCardsFormModel : BaseFormModel<CreateEditModuleSimpleCardsFormModel>
    {
        public int Id { get; set; }

        public int PageModuleId { get; private set; }
        public int Position { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public bool BlueTitle { get; set; }

        public List<ModuleSimpleCardsCard> Cards { get; set; }

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

        private ModuleSimpleCards _module;
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
                _module = ModuleSimpleCards.Query.SingleOrDefault(mc => mc.Id == Id);
                if (_module == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module not found", MessageTypes.Error));
                    return false;
                }

                var moduleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_SIMPLE_CARDS];
                _pageModule = PageModule.Query.SingleOrDefault(mp => mp.ModuleId == _module.Id && mp.ModuleTypeId == moduleTypeId && mp.PageId == _page.Id);
                if (_pageModule == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module does not belong to current page", MessageTypes.Error));
                    return false;
                }
            }

            if (Cards == null || !Cards.Any(b => !b.IsDelete))
            {
                AddMessage(nameof(State) + GetErrorMessageSufix(), new Message("Module Simple Cards requires at least one card", MessageTypes.Warning));
                return false;
            }

            return true;
        }

        protected override void OnSubmit()
        {
            var isNew = _module == null;
            if (isNew)
            {
                _module = new ModuleSimpleCards();
                _pageModule = new PageModule();
                _pageModule.ModuleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_SIMPLE_CARDS];
                _pageModule.PageId = _page.Id;
                _pageModule.Position = _page.PageModules.Count + 1;
                Position = _pageModule.Position;
            }
            
            _pageModule.Title = Title;
            _pageModule.BlueTitle = BlueTitle;
            _pageModule.Theme = Theme;
            _pageModule.TransparentBackground = TransparentBackground;
            _pageModule.LootBox = LootBox;
            _pageModule.LootBoxTop = LootBoxTop;
            _pageModule.LootBoxLeft = LootBoxLeft;
            _pageModule.State = State;

            using (var conn = new NTGDBTransactional())
            {
                _module.Save(conn);

                var subFormSuccess = true;
                var subForm = new CreateEditModuleSimpleCardsCardSubFormModel();
                var positionReduction = 0;
                foreach (var card in Cards.OrderBy(c => c.Position)) {
                    if (card.IsDelete)
                    {
                        positionReduction++;
                    }
                    else if(positionReduction > 0)
                    {
                        card.IsModified = true;
                        card.Position -= positionReduction;
                    }

                    if (card.IsModified || card.IsDelete)
                    {
                        subForm.Id = card.Id;
                        subForm.Title = card.Title;
                        subForm.Subtitle = card.Subtitle;
                        subForm.Image = card.Image;
                        subForm.Position = card.Position;
                        subForm.ModuleSimpleCards = _module;
                        subForm.IsDelete = card.IsDelete;
                        subForm.IsNewModule = Id == 0;
                        subForm.Submit(conn, Messages);

                        subFormSuccess = subFormSuccess && subForm.Success;
                        card.Id = subForm.Id;
                        card.ModuleSimpleCardsId = subForm.ModuleSimpleCards.Id;
                        card.IsModified = false;
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
                        ModuleService.MODULE_SIMPLE_CARDS,
                        conn);

                    conn.Commit();
                    Id = _module.Id;
                    PageModuleId = _pageModule.Id;
                    Cards.RemoveAll(c => c.IsDelete);
                    ModuleService.RefreshCacheModule(_pageModule.Id);
                    AddMessage(Message.GLOBAL, new Message("Module saved", MessageTypes.Success));
                }
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "SimpleCards" + (Id != 0 ? Id.ToString() : string.Empty);
        }
    }
}