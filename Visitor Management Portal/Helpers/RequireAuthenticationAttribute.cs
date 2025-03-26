using System.Web.Mvc;

namespace Visitor_Management_Portal.Helpers
{
    public class RequireAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            // Define public actions that don't require authentication
            if (controller.ToLower() == "account" || controller.ToLower() == "visitor")
            {
                return; // Skip authentication check
            }

            // Check if user is NOT authenticated
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Redirect to the login page
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "controller", "Account" },
                    { "action", "Index" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
