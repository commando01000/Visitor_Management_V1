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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            // Steps :
            // 1/  Check if the org optional data is filled => (Email , website , phone)
            // 2/  Check if the org users count is greater than 1
            // 3/ check if there at least one building in the organization

            // IF Any of the checks fails then redirect to the Index page else redirect to the dashboard page

            var isOrganizationSetupComplete = _dashboardService.IsOrganizationSetupComplete();

            if (!isOrganizationSetupComplete)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}