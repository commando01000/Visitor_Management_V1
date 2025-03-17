using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = _visitorsService.GetVisitRequests();
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
        public JsonResult AddVisitRequest(AddVisitRequestVM addVisitRequestVM)
        {
            try
            {
                // Call the service method asynchronously and wait for the result
                var result = _visitorsService.AddVisitRequest(addVisitRequestVM);

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
        public JsonResult UpdateVisitRequest(AddVisitRequestVM updateVisitRequestVM)
        {
            var result = _visitorsService.UpdateVisitRequest(updateVisitRequestVM);
            return Json(new { Status = result.Status, Message = result.Message });
        }

        [HttpPost]
        public ActionResult GetVisitRequestsFiltered(VisitRequestVM visitRequestVM)
        {
            try
            {
                // check if visitRequestVM fields are null or empty
                if (visitRequestVM.Date == null && visitRequestVM.Time == null && visitRequestVM.StatusCode == 0 && visitRequestVM.RequestedBy == null)
                {
                    var response = _visitorsService.GetVisitRequests();

                    return Json(new { success = true, data = response });
                }

                var result = _visitorsService.GetVisitRequestsFiltered(visitRequestVM);

                if (result == null || !result.Any())
                {
                    return Json(new { success = true, data = new List<VisitRequestVM>() }); // Return empty array
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching visit requests", error = ex.Message });
            }
        }


        [HttpGet]
        public ActionResult AddNewVisit(Guid? visitorId)
        {
            var locationResult = _visitorsService.GetCurrentOfficeLocation();

            if (visitorId != null)
            {
                var user = _visitorsService.GetVisitor((Guid)visitorId);
                ViewBag.VisitorId = visitorId;
                ViewBag.VisitorName = user.FullName;
                ViewBag.VisitorEmail = user.EmailAddress;
            }

            ViewBag.Location = locationResult;
            return View();
        }

        [HttpPost]
        public JsonResult DeleteVisitRequest(Guid id)
        {
            var response = _visitorsService.DeleteVisitorRequest(id);

            if (response.Status)
            {
                return Json(new { Status = response.Status, Message = response.Message, RedirectUrl = response.RedirectURL });
            }
            else
            {
                return Json(new { Status = response.Status, Message = response.Message });
            }
        }

        [HttpGet]
        public ActionResult GetVisitors()
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