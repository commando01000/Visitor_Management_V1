using System;
using System.Collections.Generic;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IZoneService 
    {
        OperationResult AddZone(AddZoneVM addZoneVM);
        OperationResult DeleteZone(Guid zoneId);
        OperationResult EditZone(EditZoneVM editZoneVM);
        OperationResult<ZoneWithRelatedAreasVM> GetZoneFullDetails(Guid zoneId);
        List<ZoneVM> GetZonesByBuildingId(Guid buildingId);
        List<ZoneVM> GetZonesForUser();
        OperationResult ToggleZoneExcludeFromOffice(Guid zoneId);
    }
}