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

        private readonly IVisitingMemberService _visitingMemberService;

        public VisitRequestController(IVisitRequestRepository visitRequesRepository, IVisitingMemberService visitingMemberService, IVisitorsService visitorsService)
        {
            this.visitRequestRepository = visitRequesRepository;
            _visitingMemberService = visitingMemberService;
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

        [HttpGet]
        public ActionResult EditVisitRequest(string visitRequestId, string visitorIds)
        {
            // You can now use visitRequestId and visitorIdsList as needed in your logic.
            return RedirectToAction("AddNewVisit", new { visitRequestId = visitRequestId, visitorsIds = visitorIds });
        }

        [HttpPost]
        public JsonResult UpdateVisitRequest(AddVisitRequestVM updateVisitRequestVM)
        {
            var result = _visitorsService.UpdateVisitRequest(updateVisitRequestVM);
            return Json(new { Status = result.Status, Message = result.Message });
        }

        [HttpPost]
        public JsonResult UpdateVisitRequestVisitors(AddVisitRequestVM updateVisitRequestVM)
        {
            var result = _visitorsService.UpdateVisitRequestVisitors(updateVisitRequestVM);
            return Json(new { Status = result.Status, Message = result.Message });
        }

        [HttpPost]
        public JsonResult RemoveVisitRequestVisitors(Guid VisitorId, Guid VisitRequestId)
        {
            var result = _visitingMemberService.RemoveVisitRequestVisitors(VisitorId, VisitRequestId);
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
        public ActionResult AddNewVisit(Guid? visitorId, Guid? visitRequestId, string visitorsIds)
        {
            var locationResult = _visitorsService.GetCurrentOfficeLocation();

            if (visitorsIds != null && visitorsIds != "")
            {
                // Convert the comma-separated visitorsIds string into a List<Guid?>
                var visitorsIdsList = visitorsIds?.Split(',')
                                                  .Select(id => (Guid?)Guid.Parse(id))
                                                  .ToList();

                ViewBag.VisitRequestId = visitRequestId;

                // Store users' information in a list in ViewBag
                var visitorsList = new List<VisitorVM>();
                foreach (var id in visitorsIdsList)
                {
                    var user = _visitorsService.GetVisitor((Guid.Parse(id.ToString())));

                    // Store the user details in the list (can be a dictionary or anonymous object)
                    visitorsList.Add(new VisitorVM()
                    {
                        VisitorId = user.Id,
                        VisitorName = user.FullName,
                        Email = user.EmailAddress
                    });
                }
                ViewBag.Visitors = visitorsList;
            }

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