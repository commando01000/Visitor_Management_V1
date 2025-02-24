using CrmEarlyBound;
using D365_Add_ons.Repository;
using System;
using System.Collections.Generic;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;
using BuildingVM = Visitor_Management_Portal.ViewModels.OrganizationSetup.BuildingVM;

namespace Visitor_Management_Portal.DAL.Repository.LocationRepository
{
    public interface ILocationRepository
    {
        List<BuildingVM> GetBuildings();
        List<ZoneVM> GetZones();
        List<ZoneVM> GetZonesByBuildingId(Guid buildingId);
        List<MeetingAreaVM> GetMeetingAreasByZoneId(Guid zoneId);
    }
}
