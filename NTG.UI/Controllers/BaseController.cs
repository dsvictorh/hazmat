using Newtonsoft.Json;
using NTG.Logic;
using NTG.Logic.Models;
using NTG.UI.Loggers;
using NTG.UI.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NTG.UI.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string ExceptionMessage = "An error ocurred while performing this action. Please try again later.";

        protected override void OnException(ExceptionContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            NTGLogger.LogError(request, filterContext.Exception);

            var contentType = request.Headers["Content-Type"];
            var acceptsJson = request.AcceptTypes.Contains("application/json");
            contentType = contentType != null ? contentType.ToLower() : string.Empty;
            if (acceptsJson || contentType.Contains("application/json") || contentType.Contains("multipart/form-data"))
            {
                var errorModel = new BaseAjaxModel();
                errorModel.AddMessage(Message.GLOBAL, new Message(ExceptionMessage, MessageTypes.Error));
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = JsonConvert.SerializeObject(errorModel)
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnException(filterContext);
            }

        }

        /*protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var result = filterContext.Result as ViewResultBase;
            if (result != null)
            {
                var viewModel = result.Model as BaseViewModel;

                if (viewModel != null)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            viewModel.AddMessage("Errors", new Message(error.ErrorMessage, MessageTypes.Error));
                        }
                    }
                }
            }          
        }  */
    }
}