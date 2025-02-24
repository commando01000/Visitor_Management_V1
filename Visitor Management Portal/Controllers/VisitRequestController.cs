using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.VisitRequestRepository;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.VisitRequest;

namespace Visitor_Management_Portal.Controllers
{
    public class VisitRequestController : Controller
    {
        private readonly IVisitRequestRepository visitRequestRepository;

        private readonly IVisitorsService _visitorsService;

        public VisitRequestController(IVisitRequestRepository visitRequesRepository, IVisitorsService visitorsService)
        {
            this.visitRequestRepository = visitRequesRepository;
            _visitorsService = visitorsService;
        }
        public ActionResult Index()
        {
            var result = _visitorsService.GetVisitRequests().Result;
            return View(result);
        }

        [HttpGet]
        public ActionResult VistRequestDetails(Guid visitRequestId)
        {
            var result = _visitorsService.GetVisitRequestDetails(visitRequestId);

            ViewBag.Purposes = _visitorsService.GetVisitRequestPurposes();
            ViewBag.RequestedByUsers = _visitorsService.GetOrganizationUsers();

            return View(result);
        }

        [HttpPost]
        public async Task<JsonResult> AddVisitRequest(AddVisitRequestVM addVisitRequestVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new OperationResult
                    {
                        Id = Guid.Empty,
                        Status = false,
                        Message = "Invalid data.",
                        RedirectURL = ""
                    });
                }

                // Call the service method asynchronously and wait for the result
                var result = await _visitorsService.AddVisitRequest(addVisitRequestVM);

                // Return the result as a JSON response
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new OperationResult
                {
                    Id = Guid.Empty,
                    Status = false,
                    Message = "An error occurred. Please try again.",
                    RedirectURL = ""
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateVisitRequest(AddVisitRequestVM updateVisitRequestVM)
        {
            var result = await _visitorsService.UpdateVisitRequest(updateVisitRequestVM);
            return Json(new { Status = result.Status, Message = result.Message });
        }

        [HttpGet]
        public async Task<ActionResult> AddNewVisit()
        {
            var locationResult = await _visitorsService.GetCurrentOfficeLocation();

            ViewBag.Location = locationResult;

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetVisitors()
        {
            var result = _visitorsService.GetByOrganization();
            return Json(result, JsonRequestBehavior.AllowGet); // Ensures JSON response
        }

        // Not used

        public ActionResult PartialVisitDetails()
        {
            return PartialView("_VisitDetailsPartial");
        }

        public ActionResult PartialAddVisitors()
        {
            return PartialView("_AddVisitorPartial");
        }

        // Helpers  

        [HttpGet]
        public JsonResult GetOrganizationUsers()
        {
            try
            {
                var result = _visitorsService.GetOrganizationUsers();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "An error occurred while fetching organization users." });
            }
        }

    }
}