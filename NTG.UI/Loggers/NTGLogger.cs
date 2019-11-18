using NTG.Logic;
using NTG.Logic.Models;
using NTG.UI.Principal;
using System;
using System.Diagnostics;
using System.Web;

namespace NTG.UI.Loggers
{
    public static class NTGLogger
    {
        public static void LogSecurityAction(HttpRequestBase request, NTGPrincipal user, string action, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new SecurityLog
                {
                    Browser = request.UserAgent,
                    Date = DateTime.UtcNow,
                    IPAddress = request.UserHostAddress,
                    Action = action,
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserType = string.Join(",", user.Roles)
                };
                log.Save(transaction);
            }
            catch (Exception ex)
            {
                LogError(request, ex);
            }
        }

        public static void LogSecurityAction(HttpRequest request, NTGPrincipal user, string action, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new SecurityLog
                {
                    Browser = request.UserAgent,
                    Date = DateTime.UtcNow,
                    IPAddress = request.UserHostAddress,
                    Action = action,
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserType = string.Join(",", user.Roles)
                };
                log.Save(transaction);
            }
            catch (Exception ex)
            {
                LogError(request, ex);
            }
        }

        public static void LogSecurityAction(HttpRequestBase request, string action, int? userId, string userEmail, string userType, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new SecurityLog
                {
                    Browser = request.UserAgent,
                    Date = DateTime.UtcNow,
                    IPAddress = request.UserHostAddress,
                    Action = action,
                    UserId = userId,
                    UserEmail = userEmail,
                    UserType = userType
                };
                log.Save(transaction);
            }
            catch (Exception ex)
            {
                LogError(request, ex);
            }
        }

        public static void LogSecurityAction(HttpRequest request, string action, int? userId, string userEmail, string userType, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new SecurityLog
                {
                    Browser = request.UserAgent,
                    Date = DateTime.UtcNow,
                    IPAddress = request.UserHostAddress,
                    Action = action,
                    UserId = userId,
                    UserEmail = userEmail,
                    UserType = userType
                };
                log.Save(transaction);
            }
            catch (Exception ex)
            {
                LogError(request, ex);
            }
        }

        public static void LogSiteAction(HttpRequestBase request, NTGPrincipal user, string action, int? pageId, string pageName, int? moduleId, string moduleType , NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new SiteLog
                {
                    AdminId = user.Id,
                    Date = DateTime.UtcNow,
                    Action = action,
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    Page = pageId != null ? ((!string.IsNullOrEmpty(pageName) ? "Page: " + pageName + ", " : string.Empty) + "Id:" + pageId) :  string.Empty,
                    Module = moduleId != null ? ((!string.IsNullOrEmpty(moduleType) ? "Module: " + moduleType + ", " : string.Empty) + "Id:" + moduleId) : string.Empty,
                };
                log.Save(transaction);
            }
            catch (Exception ex)
            {
                LogError(request, ex);
            }
        }

        public static void LogSiteAction(HttpRequest request, NTGPrincipal user, string action, int? pageId, string pageName, int? moduleId, string moduleType, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new SiteLog
                {
                    AdminId = user.Id,
                    Date = DateTime.UtcNow,
                    Action = action,
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    Page = pageId != null ? ((!string.IsNullOrEmpty(pageName) ? "Page: " + pageName + ", " : string.Empty) + "Id:" + pageId) : string.Empty,
                    Module = moduleId != null ? ((!string.IsNullOrEmpty(moduleType) ? "Module: " + moduleType + ", " : string.Empty) + "Id:" + moduleId) : string.Empty,
                };
                log.Save(transaction);
            }
            catch (Exception ex)
            {
                LogError(request, ex);
            }
        }

        public static void LogError(HttpRequestBase request, Exception ex, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new ErrorLog
                {
                    Browser = request.UserAgent,
                    Type = ex.GetType().ToString(),
                    Date = DateTime.UtcNow,
                    Function = ex.TargetSite.Name,
                    HResult = ex.HResult.ToString(),
                    IPAddress = request.UserHostAddress,
                    Line = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Message = ex.Message,
                    Object = ex.TargetSite.DeclaringType.FullName,
                    StackTrace = ex.StackTrace,
                    InnerMessage = ex.InnerException?.Message
                };
                log.Save(transaction);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void LogError(HttpRequest request, Exception ex, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new ErrorLog
                {
                    Browser = request.UserAgent,
                    Type = ex.GetType().ToString(),
                    Date = DateTime.UtcNow,
                    Function = ex.TargetSite.Name,
                    HResult = ex.HResult.ToString(),
                    IPAddress = request.UserHostAddress,
                    Line = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Message = ex.Message,
                    Object = ex.TargetSite.DeclaringType.FullName,
                    StackTrace = ex.StackTrace,
                    InnerMessage = ex.InnerException?.Message
                };
                log.Save(transaction);
            }
            catch (Exception)
            {
                return;
            }
        }
        
        public static void LogError(HttpRequestBase request, string type, string message, string obj, string function, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new ErrorLog
                {
                    Browser = request.UserAgent,
                    Type = type,
                    Date = DateTime.UtcNow,
                    Function = function,
                    IPAddress = request.UserHostAddress,
                    Message = message,
                    Object = obj,
                };
                log.Save(transaction);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void LogError(HttpRequest request, string type, string message, string obj, string function, NTGDBTransactional transaction = null)
        {
            try
            {
                var log = new ErrorLog
                {
                    Browser = request.UserAgent,
                    Type = type,
                    Date = DateTime.UtcNow,
                    Function = function,
                    IPAddress = request.UserHostAddress,
                    Message = message,
                    Object = obj,
                };
                log.Save(transaction);
            }
            catch (Exception)
            {
                return;
            }
        }

    }
}