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
    public class EditSiteSettingsFormModel : BaseFormModel<EditSiteSettingsFormModel>
    {

        #region Layout
        
        public string HeaderHomeImageUrl { get; set; }

        public string FooterHomeImageUrlLight { get; set; }

        public string FooterHomeImageUrlDark { get; set; }

        public string LoadingImageUrl { get; set; }

        public string BlogUrl { get; set; }

        public string ForumUrl { get; set; }

        public string ShopUrl { get; set; }

        public string FacebookUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string RedditUrl { get; set; }

        public string YouTubeUrl { get; set; }

        public string TwitchUrl { get; set; }

        #endregion

        #region Loot Box

        public string LootBoxImageUrl { get; set; }

        [MaxLength(50)]
        public string LootBoxPopupTitle { get; set; }

        public string LootBoxPopupText { get; set; }

        #endregion

        #region Meta Tags
        
        
        public string MetaFaviconImageUrl { get; set; }

        [MaxLength(50)]
        public string MetaSiteTitle { get; set; }

        public string MetaSiteDescription { get; set; }

        #endregion

        #region Open Graph Tags

        public string MetaOgImageUrl { get; set; }
        
        [MaxLength(20)]
        public string MetaOgImageType { get; set; }

        public int? MetaOgImageWidth { get; set; }

        public int? MetaOgImageHeight { get; set; }

        public string MetaOgUrl { get; set; }

        [MaxLength(50)]
        public string MetaOgSiteName { get; set; }

        [MaxLength(50)]
        public string MetaOgTitle { get; set; }

        public string MetaOgDescription { get; set; }

        [MaxLength(20)]
        public string MetaOgType { get; set; }

        [MaxLength(10)]
        public string MetaOgLocale { get; set; }


        #endregion

        #region Twitter Tags
        
        public string MetaTwitterImageUrl { get; set; }

        [MaxLength(20)]
        public string MetaTwitterCard { get; set; }

        [MaxLength(50)]
        public string MetaTwitterSite { get; set; }

        [MaxLength(50)]
        public string MetaTwitterTitle { get; set; }

        public string MetaTwitterDescription { get; set; }

        #endregion

        #region Error Pages
        
        public string ErrorImageIconTopUrl { get; set; }

        public string ErrorImage401Url { get; set; }

        public string ErrorImage403Url { get; set; }

        public string ErrorImage404Url { get; set; }

        public string ErrorImage500Url { get; set; }

        public string ErrorImage503Url { get; set; }

        #endregion

        private SiteSettings _siteSettings { get; set; }

        protected override void OnSubmit()
        {
            _siteSettings = Global.GlobalVariables.SiteSettings;

            #region Layout
            
            _siteSettings.HeaderHomeImageUrl = HeaderHomeImageUrl;
            _siteSettings.FooterHomeImageUrlLight = FooterHomeImageUrlLight;
            _siteSettings.FooterHomeImageUrlDark = FooterHomeImageUrlDark;
            _siteSettings.LoadingImageUrl = LoadingImageUrl;
            _siteSettings.BlogUrl = BlogUrl;
            _siteSettings.ForumUrl = ForumUrl;
            _siteSettings.ShopUrl = ShopUrl;
            _siteSettings.FacebookUrl = FacebookUrl;
            _siteSettings.TwitterUrl = TwitterUrl;
            _siteSettings.RedditUrl = RedditUrl;
            _siteSettings.YouTubeUrl = YouTubeUrl;
            _siteSettings.TwitchUrl = TwitchUrl;

            #endregion

            #region Loot Box

            _siteSettings.LootBoxImageUrl = LootBoxImageUrl;
            _siteSettings.LootBoxPopupTitle = LootBoxPopupTitle;
            _siteSettings.LootBoxPopupText = LootBoxPopupText;

            #endregion

            #region Meta Tags

            _siteSettings.MetaFaviconImageUrl = MetaFaviconImageUrl;
            _siteSettings.MetaSiteTitle = MetaSiteTitle;
            _siteSettings.MetaSiteDescription = MetaSiteDescription;

            #endregion

            #region Open Graph Tags

            _siteSettings.MetaOgImageUrl = MetaOgImageUrl;
            _siteSettings.MetaOgImageType = MetaOgImageType;
            _siteSettings.MetaOgImageWidth = MetaOgImageWidth;
            _siteSettings.MetaOgImageHeight = MetaOgImageHeight;
            _siteSettings.MetaOgUrl = MetaOgUrl;
            _siteSettings.MetaOgSiteName = MetaOgSiteName;
            _siteSettings.MetaOgTitle = MetaOgTitle;
            _siteSettings.MetaOgDescription = MetaOgDescription;
            _siteSettings.MetaOgType = MetaOgType;
            _siteSettings.MetaOgLocale = MetaOgLocale;

            #endregion

            #region Twitter Tags

            _siteSettings.MetaTwitterImageUrl = MetaTwitterImageUrl;
            _siteSettings.MetaTwitterCard = MetaTwitterCard;
            _siteSettings.MetaTwitterSite = MetaTwitterSite;
            _siteSettings.MetaTwitterTitle = MetaTwitterTitle;
            _siteSettings.MetaTwitterDescription = MetaTwitterDescription;

            #endregion

            #region Error Pages

            _siteSettings.ErrorImageIconTopUrl = ErrorImageIconTopUrl;
            _siteSettings.ErrorImage401Url = ErrorImage401Url;
            _siteSettings.ErrorImage403Url = ErrorImage403Url;
            _siteSettings.ErrorImage404Url = ErrorImage404Url;
            _siteSettings.ErrorImage500Url = ErrorImage500Url;
            _siteSettings.ErrorImage503Url = ErrorImage503Url;
            
            #endregion

            using (var conn = new NTGDBTransactional())
            {
                NTGLogger.LogSiteAction(HttpContext.Current.Request, SessionVariables.User, "Editted Site Settings", null, null, null, null, conn);

                _siteSettings.Save(conn);
                conn.Commit();
            }

            AddMessage(Message.GLOBAL, new Message("Changes successfully saved", MessageTypes.Success));
        }
    }
}