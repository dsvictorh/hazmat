using NTG.UI.Controllers.Hazmat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace NTG.UI
{

    public class CustomDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override string GetRoutePrefix(ControllerDescriptor controllerDescriptor)
        {
            var routePrefix = base.GetRoutePrefix(controllerDescriptor);
            var controllerBaseType = controllerDescriptor.ControllerType.BaseType;

            //The routes prefix attribute is not working as intended so we instead force the prefix
            switch (controllerBaseType.Name)
            {
                case "HazmatBaseController":
                    return "hazmat" + (!string.IsNullOrEmpty(routePrefix) ? "/" + routePrefix : string.Empty);
            }

            return routePrefix;
        }
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes(new CustomDirectRouteProvider());
        }
    }
    
}
