using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Visitor_Management_Portal.DAL.Repository.LocationRepository;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;
using BuildingVM = Visitor_Management_Portal.ViewModels.OrganizationSetup.BuildingVM;

namespace Visitor_Management_Portal.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationRepository locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        public JsonResult GetAllBuildings()
        {
            try
            {
                List<BuildingVM> buildings = locationRepository.GetBuildings();
                return Json(buildings, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetZonesByBuildingId(Guid buildingId)
        {
            try
            {
                List<ZoneVM> zones = locationRepository.GetZonesByBuildingId(buildingId);
                return Json(zones, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetMeetingAreasByZoneId(Guid zoneId)
        {
            try
            {
                List<MeetingAreaVM> meetingAreas = locationRepository.GetMeetingAreasByZoneId(zoneId);
                return Json(meetingAreas, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetAllZones()
        {
            try
            {
                List<ZoneVM> zones = locationRepository.GetZones();
                return Json(zones, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
