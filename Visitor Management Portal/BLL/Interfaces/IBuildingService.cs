using System;
using System.Collections.Generic;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;
using BuildingVM = Visitor_Management_Portal.ViewModels.OrganizationSetup.BuildingVM;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IBuildingService
    {
        List<BuildingVM> GetAllByOrganization(Guid organizationId);
        BuildingWithZonesAndMeetingAreasDetailsVM Get(Guid id);
        OperationResult<Guid> AddBuildingWithZonesAndMeetingAreas(AddBuildingWithZonesAndMeetingAreasVM model);
        OperationResult<Guid> UpdateBuildingExcludeFromOfficeStatus(BuildingStatusUpdateVM model);
        OperationResult<Guid> UpdateBuildingDetails(UpdateBuildingDetails model);
    }
}
