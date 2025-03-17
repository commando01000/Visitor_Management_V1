using System.Web.Mvc;
using Visitor_Management_Portal.BLL.Interfaces;

namespace Visitor_Management_Portal.Controllers
{
    
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public ActionResult Dashboard()
        {
            var isOrganizationSetupComplete = _dashboardService.IsOrganizationSetupComplete();

            if (!isOrganizationSetupComplete)
            {
                return View("Index");
            }

            return View();
        }
    }
}