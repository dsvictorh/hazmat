using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTG.UI.Models.Hazmat
{
    public class CreateEditModuleGalleryFormModel : BaseFormModel<CreateEditModuleGalleryFormModel>
    {
        public int Id { get; set; }

        public int PageModuleId { get; private set; }

        public int Position { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public bool BlueTitle { get; set; }
        public List<ModuleGalleryImage> Images { get; set; }

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

        private ModuleGallery _module;
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
                _module = ModuleGallery.Query.SingleOrDefault(mg => mg.Id == Id);
                if (_module == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module not found", MessageTypes.Error));
                    return false;
                }

                var moduleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_GALLERY];
                _pageModule = PageModule.Query.SingleOrDefault(mg => mg.ModuleId == _module.Id && mg.ModuleTypeId == moduleTypeId && mg.PageId == _page.Id);
                if (_pageModule == null)
                {
                    AddMessage(Message.GLOBAL, new Message("Module does not belong to current page", MessageTypes.Error));
                    return false;
                }

                if (Images == null || !Images.Any(b => !b.IsDelete))
                {
                    AddMessage(nameof(State) + GetErrorMessageSufix(), new Message("Module Gallery requires at least one card", MessageTypes.Warning));
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
                _module = new ModuleGallery();
                _pageModule = new PageModule();
                _pageModule.ModuleTypeId = ModuleService.ModuleTypes[ModuleService.MODULE_GALLERY];
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
                var subForm = new CreateEditModuleGalleryImageSubFormModel();
                var positionReduction = 0;
                foreach (var image in Images.OrderBy(i => i.Position))
                {
                    if (image.IsDelete)
                    {
                        positionReduction++;
                    }
                    else if(positionReduction > 0)
                    {
                        image.IsModified = true;
                        image.Position -= positionReduction;
                    }

                    if (image.IsModified || image.IsDelete)
                    {
                        subForm.Id = image.Id;
                        subForm.Image = image.Image;
                        subForm.Position = image.Position;
                        subForm.ModuleGallery = _module;
                        subForm.IsDelete = image.IsDelete;
                        subForm.IsNewModule = Id == 0;
                        subForm.Links = image.ModuleGalleryImageLinks.ToList();
                        subForm.Submit(conn, Messages);

                        subFormSuccess = subFormSuccess && subForm.Success;
                        image.Id = subForm.Id;
                        image.ModuleGalleryId = subForm.ModuleGallery.Id;
                        image.ModuleGalleryImageLinks = subForm.Links;
                        image.IsModified = false;
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
                        ModuleService.MODULE_GALLERY,
                        conn);

                    conn.Commit();
                    Id = _module.Id;
                    PageModuleId = _pageModule.Id;
                    Images.RemoveAll(c => c.IsDelete);
                    ModuleService.RefreshCacheModule(_pageModule.Id);
                    AddMessage(Message.GLOBAL, new Message("Module saved", MessageTypes.Success));
                }
            }
        }

        protected override string GetErrorMessageSufix()
        {
            return "Gallery" + (Id != 0 ? Id.ToString() : string.Empty);
        }
    }
}