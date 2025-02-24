using System;
using System.Web.Mvc;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;

namespace Visitor_Management_Portal.Controllers
{
    
    public class OrganizationSetupController : Controller
    {
        private readonly IBuildingService _buildingService;
        private readonly IZoneService _zoneService;
        private readonly IOrganizationUsersRepository _organizationUsersRepository;
        private readonly IMeetingAreaService _meetingAreaService;

        public OrganizationSetupController
            (IBuildingService buildingService,
            IZoneService zoneService,
            IOrganizationUsersRepository organizationUsersRepository,
            IMeetingAreaService organizationService
            )
        {
            _buildingService = buildingService;
            _zoneService = zoneService;
            _organizationUsersRepository = organizationUsersRepository;
            _meetingAreaService = organizationService;
        }

        public ActionResult Index()
        {
            var organizationId = ClaimsManager.GetOrganizationId();

            var model = _buildingService.GetAllByOrganization(organizationId);
            return View(model);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Building()
        {
            var organizationId = ClaimsManager.GetOrganizationId();

            var model = _buildingService.GetAllByOrganization(organizationId);

            return PartialView("_BuildingPartial", model);
        }

        #region Zones

        public ActionResult ZonesDetails()
        {
            var zones = _zoneService.GetZonesForUser();

            var organizationId = ClaimsManager.GetOrganizationId();
            var relatedBuildings = _buildingService.GetAllByOrganization(organizationId);
            ViewBag.RelatedBuildings = relatedBuildings;

            return PartialView("_ZonesPartial", zones);
        }

        public ActionResult MeetingAreasByBuilding(Guid buildingId)
        {
            var model = _meetingAreaService.GetAllByBuilding(buildingId);  
            ViewBag.AllZones = _zoneService.GetZonesByBuildingId(buildingId);
            ViewBag.BuildingId = buildingId;
            return PartialView("_MeetingAreasByBuilding", model);
        }

        [HttpPost]
        public ActionResult ToggleZoneExcludeFromOffice(Guid zoneId)
        {
            var result = _zoneService.ToggleZoneExcludeFromOffice(zoneId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddZone(AddZoneVM addZoneVM)
        {
            var result = _zoneService.AddZone(addZoneVM);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditZone(EditZoneVM editZoneVM)
        {
            var result = _zoneService.EditZone(editZoneVM);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteZone(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
            {
                return Json(new { Status = false, Message = "Invalid Zone ID" });
            }

            var result = _zoneService.DeleteZone(zoneId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ZoneFullDetails(Guid zoneId)
        {
            var result = _zoneService.GetZoneFullDetails(zoneId);

            var organizationId = ClaimsManager.GetOrganizationId();
            var relatedBuildings = _buildingService.GetAllByOrganization(organizationId);
            ViewBag.RelatedBuildings = relatedBuildings;

            return View(result.Data);
        }

        #endregion

        #region Meeting Areas

        public ActionResult MeetingAreasDetails()
        {
            var meetingAreas = _meetingAreaService.GetMeetingAreasForUser();

            var organizationId = ClaimsManager.GetOrganizationId();
            var relatedBuildings = _buildingService.GetAllByOrganization(organizationId);
            ViewBag.RelatedBuildings = relatedBuildings;

            return PartialView("_MeetingAreasPartial", meetingAreas);
        }

        [HttpPost]
        public ActionResult ToggleMeetingAvailability(Guid meetingAreaId)
        {
            var result = _meetingAreaService.ToggleMeetingAreaAvailability(meetingAreaId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddMeetingArea(AddMeetingAreaVM addMeetingAreaVM)
        {
            var result = _meetingAreaService.AddMeetingArea(addMeetingAreaVM);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditMeetingArea(EditMeetingAreaVM editMeetingAreaVM)
        {
            var result = _meetingAreaService.EditMeetingArea(editMeetingAreaVM);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMeetingArea(Guid areaId)
        {
            if (areaId == Guid.Empty)
            {
                return Json(new { Status = false, Message = "Invalid Meeting Area ID" });
            }

            var result = _meetingAreaService.DeleteMeetingArea(areaId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public JsonResult UpdateBuildingExcludeFromOfficeStatus(BuildingStatusUpdateVM model)
        {
            var result = _buildingService.UpdateBuildingExcludeFromOfficeStatus(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetZonesByBuildingId(Guid buildingId)
        {
            var result = _zoneService.GetZonesByBuildingId(buildingId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewBuilding()
        {
            var organizationId = ClaimsManager.GetOrganizationId();

            ViewBag.OrganizationUsers = _organizationUsersRepository.GetByOrganizationId(organizationId);

            return View();
        }

        [HttpPost]
        public JsonResult AddBuildingWithZonesAndMeetingAreas(AddBuildingWithZonesAndMeetingAreasVM model)
        {

            var result = _buildingService.AddBuildingWithZonesAndMeetingAreas(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuildingDetails(Guid id)
        {
            var organizationId = ClaimsManager.GetOrganizationId();
            ViewBag.OrganizationUsers = _organizationUsersRepository.GetByOrganizationId(organizationId);

            var model = _buildingService.Get(id);
            //TODO: check null to redirect to not found
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateBuildingDetails(UpdateBuildingDetails model)
        {

            var result = _buildingService.UpdateBuildingDetails(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}