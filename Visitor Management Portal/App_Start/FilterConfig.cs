using System.Web.Mvc;
using Visitor_Management_Portal.Helpers;

namespace Visitor_Management_Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RequireAuthenticationAttribute()); // Add our custom filter
        }
    }
}