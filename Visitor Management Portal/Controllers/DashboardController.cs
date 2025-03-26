using Microsoft.Graph;
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
            return View(_dashboardService.DashboardAnalysis());
        }

        [HttpGet]
        public JsonResult TotalVisitChartCall(int Period, int weekNumber = 0) =>
            Json(_dashboardService.TotalVisistsChart(Period, weekNumber), JsonRequestBehavior.AllowGet);
        
        [HttpGet]
        public JsonResult PurposeChartCall(int Period) =>
            Json(_dashboardService.PurposeChartAnalysis(Period), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public JsonResult PeakTimeChartCall(int Period) =>
            Json(_dashboardService.PeakTimeChartAnalysis(Period), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public JsonResult TotalVisitsNumberAnalysis(int Period) =>
             Json(_dashboardService.TotalVisitsAnalysis(Period), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public JsonResult VisitsByDepartmentChartCall(int Period) =>
            Json(_dashboardService.VisitByDepartmentAnalysis(Period), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public JsonResult VisitsByZoneChartCall(int Period) =>
            Json(_dashboardService.VisitsByZoneAnalysis(Period), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public JsonResult PendingApprovalVisitRequestsListCall() =>
           Json(_dashboardService.PendingApprovalVisitRequestsList(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public JsonResult TopVisitorsListCall(int Period) =>
         Json(_dashboardService.TopVisitorsList(Period), JsonRequestBehavior.AllowGet);
    }
}