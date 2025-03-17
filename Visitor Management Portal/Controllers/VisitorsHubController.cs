using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public ActionResult GetVisitors()
        {
            var visitors = _visitorsService.GetVisitors();

            return Json(visitors, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetVisitorsHubFiltered(VisitorsHubVM visitorsHubVM)
        {
            try
            {
                // check if visitorsHubVM fields are null or empty
                if (visitorsHubVM.IDNumber == "" && (visitorsHubVM.StatusCode == 0 || visitorsHubVM.StatusCode == null) && visitorsHubVM.Id == Guid.Empty)
                {
                    var response = _visitorsService.GetByOrganization();
                    return Json(new { success = true, data = response });
                }

                var result = _visitorsService.GetVisitorsHubFiltered(visitorsHubVM);

                if (result == null || !result.Any())
                {
                    return Json(new { success = true, data = new List<VisitorsHubVM>() }); // Return empty array
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching visitors hub", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditVisitor(ViewModels.VisitorsHub.VisitorVM model)
        {
            OperationResult result = _visitorsService.UpdateVisitor(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteVisitorHub(Guid id)
        {
            var response = _visitorsService.DeleteVisitorHub(id);

            if (response.Status)
            {
                return Json(new { Status = response.Status, Message = response.Message, RedirectUrl = response.RedirectURL });
            }
            else
            {
                return Json(new { Status = response.Status, Message = response.Message });
            }

        }

        public ActionResult VisitorDetails(Guid id)
        {
            var visitor = _visitorsService.GetVisitor(id);

            List<VisitingMemberWithRelatedRequestVM> RelatedVisitRequests = _visitorsService.GetVisitorRequestsHistory(id);

            if (visitor == null)
                return RedirectToAction("NotFound", "Error");

            ViewBag.RelatedVisitRequests = RelatedVisitRequests;
            return View(visitor);
        }
    }
}