using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Cryptography;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Session;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NTG.UI.Models.Hazmat
{
    public class CreateEditModulePromoFormModel : BaseFormModel<CreateEditModulePromoFormModel>
    {
        public int Id { get; set; }

        public int PageModuleId { get; private set; }
        public int Position { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public bool BlueTitle { get; set; }

        [Required]
        [MaxLength(200)]
        public string Text { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        [MaxLength(20)]
        public string ButtonText { get; set; }

        [Required]
        public string ButtonLink { get; set; }

        public bool ImageRight { get; set; }

        public bool TitleTop { get; set; }

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

        private ModulePromo _module;
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
                _module = ModulePromo.Query.SingleOrDefault(mc => mc.Id == Id);
                if (_module == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module not found", MessageTypes.Error));
                    return false;
                }

                var moduleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_PROMO];
                _pageModule = PageModule.Query.SingleOrDefault(mp => mp.ModuleId == _module.Id && mp.ModuleTypeId == moduleTypeId && mp.PageId == _page.Id);
                if (_pageModule == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module does not belong to current page", MessageTypes.Error));
                    return false;
                }
            }            

            return true;
        }

        protected override void OnSubmit()
        {
            var isNew = _module == null;
            if (isNew)
            {
                _module = new ModulePromo();
                _pageModule = new PageModule();
                _pageModule.ModuleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_PROMO];
                _pageModule.PageId = _page.Id;
                _pageModule.Position = _page.PageModules.Count + 1;
                Position = _pageModule.Position;
            }

            _module.Image = Image;
            _module.Text = Text;
            _module.ButtonText = ButtonText;
            _module.ButtonLink = !string.IsNullOrEmpty(ButtonText) ? ButtonLink : string.Empty;
            _module.ImageRight = ImageRight;
            _module.TitleTop = TitleTop;
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
                _pageModule.ModuleId = _module.Id;
                _pageModule.Save(conn);

                NTGLogger.LogSiteAction(HttpContext.Current.Request,
                         SessionVariables.User,
                         (isNew ? "Created" : "Editted") + " Module",
                         _page.Id,
                         _page.Name,
                         _module.Id,
                         ModuleService.MODULE_PROMO,
                         conn);

                conn.Commit();
                Id = _module.Id;
                PageModuleId = _pageModule.Id;
                ModuleService.RefreshCacheModule(_pageModule.Id);
                AddMessage(Message.GLOBAL, new Message("Module saved", MessageTypes.Success));
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Promo" + (Id != 0 ? Id.ToString() : string.Empty);
        }
    }
}