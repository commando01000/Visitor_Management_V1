using CrmEarlyBound;
using System;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class MeetingAreaVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool? AppearInVisitRequests { get; set; }

        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; }

        public Guid ZoneId { get; set; }
        public string ZoneName { get; set; }

        public static MeetingAreaVM MapFromEntity(vm_MeetingArea entity)
        {
            if (entity == null) return null;

            return new MeetingAreaVM()
            {
                Id = entity.Id,

                Name = entity.vm_MeetingAreaName != null ? entity.vm_MeetingAreaName : string.Empty,
                Code = entity.vm_MeetingAreaCode ?? string.Empty,
                AppearInVisitRequests = entity.vm_AppearInVisitRequests != null ? 
                                        (bool)entity.vm_AppearInVisitRequests : false,

                BuildingId = entity.vm_Building != null ? entity.vm_Building.Id : Guid.Empty,
                BuildingName = entity.vm_Building != null ? entity.vm_Building.Name : string.Empty,

                ZoneId = entity.vm_Zone != null ? entity.vm_Zone.Id : Guid.Empty,
                ZoneName = entity.vm_Zone != null ? entity.vm_Zone.Name : string.Empty
            };
        }
    }
}