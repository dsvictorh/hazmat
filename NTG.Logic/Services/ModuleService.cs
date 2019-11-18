using NTG.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTG.Logic.Services
{
    public class CacheModule
    {
        public PageModule Info { get; }
        public object Details { get; }

        public CacheModule(PageModule pageModule, object module)
        {
            var moduleTypeName = module.GetType().Name.Replace("Module", string.Empty);
            if (!ModuleService.ModuleTypes.ContainsKey(moduleTypeName))
                throw new Exception("Module of unknown type " + moduleTypeName + " is not valid");

            Info = pageModule;
            Details = module;
        }
    }

    public static class ModuleService
    {
        public const string MODULE_CALLOUT = "Callout";
        public const string MODULE_BOXES = "Boxes";
        public const string MODULE_SIMPLE_CARDS = "SimpleCards";
        public const string MODULE_PROFILE_CARDS = "ProfileCards";
        public const string MODULE_FREE_TEXT = "FreeText";
        public const string MODULE_GALLERY = "Gallery";
        public const string MODULE_PROMO = "Promo";

        private static Dictionary<string, int> _moduleTypes = new Dictionary<string, int>();
        private static List<CacheModule> _modulesCache = new List<CacheModule>();
        public static Dictionary<string, int> ModuleTypes
        {
            get {
                if (_moduleTypes.Count == 0)
                {
                    ModuleType.Query.ToList().ForEach(mt => _moduleTypes.Add(mt.Name, mt.Id));
                }

                return _moduleTypes;
            }
        }

        private static object GetDbModuleByType(int moduleId, string moduleType)
        {
            switch (moduleType)
            {
                case MODULE_CALLOUT:
                    return GetModuleCallout(moduleId);
                case MODULE_BOXES:
                    return GetModuleBoxes(moduleId);
                case MODULE_SIMPLE_CARDS:
                    return GetModuleSimpleCards(moduleId);
                case MODULE_PROFILE_CARDS:
                    return GetModuleProfileCards(moduleId);
                case MODULE_FREE_TEXT:
                    return GetModuleFreeText(moduleId);
                case MODULE_GALLERY:
                    return GetModuleGallery(moduleId);
                case MODULE_PROMO:
                    return GetModulePromo(moduleId);
                default:
                    return null;
            }
        }

        public static void RefreshModulesCache(int? pageId = null) {
            _modulesCache.RemoveAll(mc => !pageId.HasValue || mc.Info.PageId == pageId);
            PageModule.Query.Include(nameof(PageModule.ModuleType)).Where(pm => !pageId.HasValue || pm.PageId == pageId).ToList().ForEach(pm =>
            {
                _modulesCache.Add(new CacheModule(pm, GetDbModuleByType(pm.ModuleId, pm.ModuleType.Name)));
            });
        }

        public static void RefreshCacheModule(int pageModuleId) {
            var pageModule = PageModule.Query.Include(nameof(PageModule.ModuleType)).FirstOrDefault(pm => pm.Id == pageModuleId);
            if (pageModule != null) {
                _modulesCache.RemoveAll(mc => mc.Info.Id == pageModule.Id);
                _modulesCache.Add(new CacheModule(pageModule, GetDbModuleByType(pageModule.ModuleId, pageModule.ModuleType.Name)));
            }
        }

        public static IReadOnlyCollection<CacheModule> GetModulesFromCache(int pageId, PageModuleStates? state = null)
        {
            bool lastModuleTheme;
            return GetModulesFromCache(pageId, out lastModuleTheme, state);
        }

        public static IReadOnlyCollection<CacheModule> GetModulesFromCache(int pageId, out bool lastModuleTheme, PageModuleStates? state = null)
        {
            var modules = _modulesCache
                                .Where(pm => pm.Info.PageId == pageId && (state != null ? pm.Info.State == state : true))
                                .OrderBy(pm => pm.Info.Position);

            lastModuleTheme = modules.Any() && modules.Last().Info.Theme;
            return modules.ToList().AsReadOnly();
        }

        public static void DeleteModule(PageModule pageModule, NTGDBTransactional conn)
        {
            if (pageModule != null)
            {
                var moduleType = ModuleType.Query
                    .SingleOrDefault(mt => mt.Id == pageModule.ModuleTypeId);
                var moduleId = pageModule.ModuleId;

                pageModule.Delete(conn);
                switch (moduleType.Name)
                {
                    case MODULE_CALLOUT:
                        DeleteModuleCallout(moduleId, conn);
                        break;
                    case MODULE_BOXES:
                        DeleteModuleBoxes(moduleId, conn);
                        break;
                    case MODULE_SIMPLE_CARDS:
                        DeleteModuleSimpleCards(moduleId, conn);
                        break;
                    case MODULE_PROFILE_CARDS:
                        DeleteModuleProfileCards(moduleId, conn);
                        break;
                    case MODULE_FREE_TEXT:
                        DeleteModuleFreeText(moduleId, conn);
                        break;
                    case MODULE_GALLERY:
                        DeleteModuleGallery(moduleId, conn);
                        break;
                    case MODULE_PROMO:
                        DeleteModulePromo(pageModule.ModuleId, conn);
                        break;
                }
            }
        }

        #region Callout
        private static ModuleCallout GetModuleCallout(int moduleId)
        {
            return ModuleCallout.Query
                .SingleOrDefault(mc => mc.Id == moduleId);
        }

        private static void DeleteModuleCallout(int moduleId, NTGDBTransactional conn)
        {
            var module = ModuleCallout.Query
                .SingleOrDefault(mc => mc.Id == moduleId);

            if (module != null)
            {
                module.Delete(conn);
            }
        }
        #endregion

        #region Boxes
        private static ModuleBoxes GetModuleBoxes(int moduleId)
        {
            return ModuleBoxes.Query
                .Include(nameof(ModuleBoxes.ModuleBoxesBox))
                .SingleOrDefault(mb => mb.Id == moduleId);
        }

        private static void DeleteModuleBoxes(int moduleId, NTGDBTransactional conn)
        {
            var module = ModuleBoxes.Query.Include(nameof(ModuleBoxes.ModuleBoxesBox)).SingleOrDefault(mb => mb.Id == moduleId);
            if (module != null)
            {
                foreach (var box in module.ModuleBoxesBox.ToList())
                {
                    box.Delete(conn);
                }

                module.Delete(conn);
            }
        }
        #endregion

        #region SimpleCards
        private static ModuleSimpleCards GetModuleSimpleCards(int moduleId)
        {
            return ModuleSimpleCards.Query.Include(nameof(ModuleSimpleCards.ModuleSimpleCardsCards)).SingleOrDefault(mb => mb.Id == moduleId);
        }

        private static void DeleteModuleSimpleCards(int moduleId, NTGDBTransactional conn)
        {
            var module = ModuleSimpleCards.Query
                .Include(nameof(ModuleSimpleCards.ModuleSimpleCardsCards))
                .SingleOrDefault(mb => mb.Id == moduleId);

            if (module != null)
            {
                foreach (var card in module.ModuleSimpleCardsCards.ToList())
                {
                    card.Delete(conn);
                }

                module.Delete(conn);
            }
        }
        #endregion

        #region ProfileCards
        private static ModuleProfileCards GetModuleProfileCards(int moduleId)
        {
            return ModuleProfileCards.Query
                .Include(nameof(ModuleProfileCards.ModuleProfileCardsCards))
                .Include(nameof(ModuleProfileCards.ModuleProfileCardsCards) + "." + nameof(ModuleProfileCardsCard.ModuleProfileCardsCardLinks))
                .SingleOrDefault(mpc => mpc.Id == moduleId);
        }

        private static void DeleteModuleProfileCards(int moduleId, NTGDBTransactional conn)
        {
            var module = ModuleProfileCards.Query
                .Include(nameof(ModuleProfileCards.ModuleProfileCardsCards))
                .Include(nameof(ModuleProfileCards.ModuleProfileCardsCards) + "." + nameof(ModuleProfileCardsCard.ModuleProfileCardsCardLinks))
                .SingleOrDefault(mb => mb.Id == moduleId);

            if(module != null)
            {
                foreach(var card in module.ModuleProfileCardsCards.ToList())
                {
                    foreach (var link in card.ModuleProfileCardsCardLinks.ToList())
                    {
                        link.Delete(conn);
                    }

                    card.Delete(conn);
                }
                module.Delete(conn);
            }
        }
        #endregion

        #region FreeText
        private static ModuleFreeText GetModuleFreeText(int moduleId)
        {
            return ModuleFreeText.Query
                .SingleOrDefault(mc => mc.Id == moduleId);
        }

        private static void DeleteModuleFreeText(int moduleId, NTGDBTransactional conn)
        {
            var module = ModuleFreeText.Query.SingleOrDefault(mc => mc.Id == moduleId);
            if (module != null)
            {
                module.Delete(conn);
            }
        }
        #endregion

        #region Gallery
        private static ModuleGallery GetModuleGallery(int moduleId)
        {
            return ModuleGallery.Query
                .Include(nameof(ModuleGallery.ModuleGalleryImages))
                .Include(nameof(ModuleGallery.ModuleGalleryImages) + "." + nameof(ModuleGalleryImage.ModuleGalleryImageLinks))
                .SingleOrDefault(mpc => mpc.Id == moduleId);
        }

        private static void DeleteModuleGallery(int moduleId, NTGDBTransactional conn)
        {
            var module = ModuleGallery.Query
                .Include(nameof(ModuleGallery.ModuleGalleryImages))
                .Include(nameof(ModuleGallery.ModuleGalleryImages) + "." + nameof(ModuleGalleryImage.ModuleGalleryImageLinks))
                .SingleOrDefault(mpc => mpc.Id == moduleId);

            if (module != null)
            {
                foreach (var image in module.ModuleGalleryImages.ToList())
                {
                    foreach (var link in image.ModuleGalleryImageLinks.ToList())
                    {
                        link.Delete(conn);
                    }

                    image.Delete(conn);
                }
                module.Delete(conn);
            }
        }
        #endregion

        #region Promo
        private static ModulePromo GetModulePromo(int moduleId)
        {
            return ModulePromo.Query
                .SingleOrDefault(mp => mp.Id == moduleId);
        }

        private static void DeleteModulePromo(int moduleId, NTGDBTransactional conn)
        {
            var module = ModulePromo.Query
                .SingleOrDefault(mp => mp.Id == moduleId);

            if (module != null)
            {
                module.Delete(conn);
            }
        }
        #endregion
    }
}
