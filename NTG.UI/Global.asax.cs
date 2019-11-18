using Newtonsoft.Json;
using NTG.Logic.Models;
using NTG.Logic.Services;
using NTG.UI.Loggers;
using NTG.UI.Models;
using NTG.UI.Principal;
using NTG.UI.Resources;
using NTG.UI.Session;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace NTG.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AttributeErrorsDataAnnotationsValidator.ResourceType = typeof(AttributeErrors);

            //Attributes
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MaxLengthAttribute), typeof(AttributeErrorsDataAnnotationsValidator));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(AttributeErrorsDataAnnotationsValidator));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RegularExpressionAttribute), typeof(AttributeErrorsDataAnnotationsValidator));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailAddressAttribute), typeof(AttributeErrorsDataAnnotationsValidator));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(AttributeErrorsDataAnnotationsValidator));

            //Load the modules cache on application start
            ModuleService.RefreshModulesCache();
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported)
            {
               
                try
                {
                    var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                        return;

                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket.UserData.Contains("Hazmat"))
                    {
                        var user = Admin.Query.Include(nameof(Admin.Role)).SingleOrDefault(a => a.Email == authTicket.Name && a.Active);

                        if (user == null) {
                            NTGLogger.LogSecurityAction(Context.Request, "Attempted cookied forgery or use of non-active admin '" + (authTicket.Name ?? "Unknown") + "'", null, authTicket.Name, "Hazmat");
                            return;
                        }
                        
                        HttpContext.Current.User = new NTGPrincipal(new GenericIdentity(authTicket.Name, "Forms"), new string[] { "Hazmat", user.Role.Name }, user.Id, user.Email, true);
                    }
                    else
                    {
                        //TODO: Change from Admin to User should a user base be established
                        var user = Admin.Query.SingleOrDefault(a => a.Email == authTicket.Name && a.Active);

                        if (user == null)
                        {
                            NTGLogger.LogSecurityAction(Context.Request, "Attempted cookied forgery or use of non-active member '" + (authTicket.Name ?? "Unknown") + "'", null, authTicket.Name, "Member");
                            return;
                        }

                        HttpContext.Current.User = new NTGPrincipal(new GenericIdentity(authTicket.Name, "Forms"), new string[] { "Member" }, user.Id, user.Email, false);
                    }

                    if (authTicket.IsPersistent) {
                        authCookie.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Set(authCookie);
                    } 
                }
                catch (Exception ex)
                {
                    var request = Context.Request;
                    NTGLogger.LogError(Context.Request, ex);
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var ex = Server.GetLastError();
                Server.ClearError();
                NTGLogger.LogError(Context.Request, ex);

                var httpException = ex as HttpException;
                Response.StatusCode = httpException != null ? httpException.GetHttpCode() : (int)HttpStatusCode.InternalServerError;
                
                if (IsMaxRequestExceededException(ex))
                {
                    this.Server.ClearError();
                    Context.ClearError();
                    var errorModel = new BaseAjaxModel();
                    errorModel.AddMessage(Message.GLOBAL, new Message("Unable to upload. Max file size exceeded", MessageTypes.Error));
                    Context.Response.ContentType = "application/json";
                    Context.Response.StatusCode = 500;
                    Context.Response.Write(JsonConvert.SerializeObject(errorModel));
                }
            }
            catch (Exception){
                return;
            }
        }

        

        const int TimedOutExceptionCode = -2147467259;
        private static bool IsMaxRequestExceededException(Exception ex)
        {
            // unhandled errors = caught at global.ascx level
            // http exception = caught at page level

            Exception main;
            var unhandled = ex as HttpUnhandledException;

            if (unhandled != null && unhandled.ErrorCode == TimedOutExceptionCode)
            {
                main = unhandled.InnerException;
            }
            else
            {
                main = ex;
            }


            var http = main as HttpException;

            if (http != null && http.ErrorCode == TimedOutExceptionCode)
            {
                // hack: no real method of identifying if the error is max request exceeded as 
                // it is treated as a timeout exception
                if (http.StackTrace.Contains("GetEntireRawContent") && ex.Message.Contains("Maximum request length exceeded"))
                {
                    // MAX REQUEST HAS BEEN EXCEEDED
                    return true;
                }
            }

            return false;
        }
    }
}
