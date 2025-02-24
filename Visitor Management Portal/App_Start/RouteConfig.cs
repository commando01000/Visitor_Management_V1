using System.Web.Mvc;
using System.Web.Routing;

namespace Visitor_Management_Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route for the login page (Account/Index)
            routes.MapRoute(
                name: "Login",
                url: "Account/Index",
                defaults: new { controller = "Account", action = "Index" }
            );

            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}