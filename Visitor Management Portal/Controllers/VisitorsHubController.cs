using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows.Documents;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.VisitorsHub;
using Visitor_Management_Portal.ViewModels.VisitRequest;

namespace Visitor_Management_Portal.Controllers
{
    
    public class VisitorsHubController : Controller
    {
        private readonly IVisitorsService _visitorsService;
        public VisitorsHubController(IVisitorsService visitorsService)
        {
            _visitorsService = visitorsService;
        }

        public ActionResult Index()
        {
            var visitors = _visitorsService.GetByOrganization();

            return View(visitors);
        }

        public ActionResult AddVisitor()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddVisitor(AddVisitorVM model)
        {
            var result = _visitorsService.Add(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditVisitor(Guid id)
        {
            ViewModels.VisitorsHub.VisitorVM visitor = _visitorsService.GetVisitor(id);
            if (visitor == null)
                return View("Error", new { Message = "Visitor not found." });

            return View(visitor);
        }

        [HttpPost]
        public ActionResult EditVisitor(ViewModels.VisitorsHub.VisitorVM model)
        {
            OperationResult result = _visitorsService.UpdateVisitor(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> VisitorDetails(Guid id)
        {
            var visitor = _visitorsService.GetVisitor(id);

            List<VisitRequestVM> RelatedVisitRequests = await _visitorsService.GetVisitorRequestsHistory(id);

            if (visitor == null)
                return RedirectToAction("NotFound", "Error");

            ViewBag.RelatedVisitRequests = RelatedVisitRequests;
            return View(visitor);
        }
    }
}