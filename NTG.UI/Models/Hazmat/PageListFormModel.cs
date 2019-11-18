using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Session;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace NTG.UI.Models.Hazmat
{
    public class PageAction
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public bool Delete { get; set; }
    };

    public class PageListFormModel : BaseFormModel<PageListFormModel>
    {     
        public List<PageAction> PageActions {get; set;}

        private List<Page> _pages;

        protected override bool OnValidate()
        {
            _pages = new List<Page>();
            foreach (var pageAction in PageActions)
            {
                var page = Page.Query.SingleOrDefault(p => p.Id == pageAction.Id);
                if (page == null)
                {
                    AddMessage("PageList", new Message("Page in collection with Id " + pageAction.Id + " does not exist", MessageTypes.Error));
                    return false;
                }

                if (pageAction.Delete)
                {
                    if (pageAction.Position == 1 && pageAction.Delete)
                    {
                        AddMessage(Message.GLOBAL, new Message("Page with position 1 cannot be deleted", MessageTypes.Error));
                        NTGLogger.LogSecurityAction(HttpContext.Current.Request, SessionVariables.User, "Attempted to delete current home page " + page.Id + " '" + page.Name + "'");
                        return false;
                    }

                    if (pageAction.Delete && page.AdminLocked && !SessionVariables.User.IsInRole("Admin"))
                    {
                        AddMessage("PageList", new Message("You are not permitted to delete admin locked page " + pageAction.Id, MessageTypes.Error));
                        NTGLogger.LogSecurityAction(HttpContext.Current.Request, SessionVariables.User, "Attempted to delete admin locked page " + page.Id + " '" + page.Name + "'");
                        return false;
                    }
                }

                _pages.Add(page);
            }
            
            return true;
        }

        protected override void OnSubmit()
        {
            var deleteLog = string.Empty;
            var orderLog = string.Empty;
            using (var conn = new NTGDBTransactional())
            {
                foreach (var page in _pages.OrderBy(p => PageActions.OrderBy(pa => pa.Position).Select(pa => pa.Id).ToList().IndexOf(p.Id)))
                {
                    var action = PageActions.SingleOrDefault(pa => pa.Id == page.Id);
                    if (action != null)
                    {
                        if (action.Delete)
                        {
                            var pageModules = PageModule.Query.Where(pm => pm.PageId == page.Id);
                            foreach (var pageModule in pageModules)
                            {
                                ModuleService.DeleteModule(pageModule, conn);   
                            }

                            var cloudinaryResult = CloudinaryService.DeleteFolder(page.UploadFolder);
                            if (!string.IsNullOrEmpty(cloudinaryResult.error))
                            {
                                NTGLogger.LogError(HttpContext.Current.Request, "Cloudinary Error", cloudinaryResult.error, nameof(PageListFormModel), nameof(CloudinaryService.DeleteFolder), conn);
                            }
                            page.Delete(conn);
                            deleteLog += " - " +  action.Id;
                            foreach (var nextAction in PageActions.Where(pa => pa.Position > action.Position))
                            {
                                nextAction.Position -= 1;
                            }
                        }
                        else if(page.Position != action.Position)
                        {
                            page.Position = action.Position;
                            page.Save(conn);
                            orderLog += " - " + action.Id;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(orderLog))
                {
                    NTGLogger.LogSiteAction(HttpContext.Current.Request,
                        SessionVariables.User,
                        "Changed pages order " + orderLog,
                        null,
                        null,
                        null,
                        null,
                        conn);
                }

                if (!string.IsNullOrEmpty(deleteLog))
                {
                    NTGLogger.LogSiteAction(HttpContext.Current.Request,
                        SessionVariables.User,
                        "Deleted pages " + deleteLog,
                        null,
                        null,
                        null,
                        null,
                        conn);
                }

                conn.Commit();
                AddMessage(Message.GLOBAL, new Message("Pages successfully saved", MessageTypes.Success));
            }
        }
    }
}