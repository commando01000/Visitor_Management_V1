using System;
using System.Collections.Generic;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IMeetingAreaService
    {
        List<MeetingAreaVM> GetMeetingAreasForUser();
        List<MeetingAreaVM> GetAllByBuilding(Guid buildingId);

        OperationResult<List<MeetingAreaVM>> GetMeetingAreasByZone(Guid zoneId);

        OperationResult<Guid> AddMeetingArea(AddMeetingAreaVM addMeetingAreaVM);

        OperationResult ToggleMeetingAreaAvailability(Guid meetingAreaId);

        OperationResult EditMeetingArea(EditMeetingAreaVM editMeetingAreaVM);

        OperationResult DeleteMeetingArea(Guid areaId);
    }
}