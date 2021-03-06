using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SVX
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{idDepartamento}/{filtro}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, idDepartamento = UrlParameter.Optional, filtro = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Chat",
               url: "{controller}/{action}/{idUser}/{idConver}",
               defaults: new { controller = "Home", action = "Chat", idUser = UrlParameter.Optional, idConver = UrlParameter.Optional }
           );
        }
    }
}
