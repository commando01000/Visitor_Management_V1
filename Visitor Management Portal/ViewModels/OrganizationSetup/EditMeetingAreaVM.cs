using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using System;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class EditMeetingAreaVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool AppearInVisitRequests { get; set; }
        public Guid BuildingId { get; set; }
        public Guid ZoneId { get; set; }

        public static void MapToEntity(vm_MeetingArea entity, EditMeetingAreaVM model)
        {
            entity.vm_MeetingAreaName = model.Name;
            entity.vm_MeetingAreaCode = model.Code;
            entity.vm_AppearInVisitRequests = model.AppearInVisitRequests;
            entity.vm_Building = new EntityReference(vm_Building.EntityLogicalName, model.BuildingId);
            entity.vm_Zone = new EntityReference(vm_Zone.EntityLogicalName, model.ZoneId);
        }
    }
}
