using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using System;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class AddMeetingAreaVM
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool AppearInVisitRequests { get; set; }
        public Guid BuildingId { get; set; }
        public Guid ZoneId { get; set; }

        public static vm_MeetingArea MapToEntity(AddMeetingAreaVM model)
        {
            if (model == null) return null;

            return new vm_MeetingArea()
            {
                vm_MeetingAreaName = model.Name,
                vm_MeetingAreaCode = model.Code ?? string.Empty,
                vm_AppearInVisitRequests = model.AppearInVisitRequests,

                vm_Building = new EntityReference("vm_building", model.BuildingId),
                vm_Zone = new EntityReference("vm_zone", model.ZoneId)
            };
        }
    }
}
