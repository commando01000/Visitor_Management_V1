using CrmEarlyBound;
using System;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class ZoneVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int RelatedAreasCount { get; set; }

        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; }

        public bool? ExcludeFromOfficeSelection { get; set; }

        public static ZoneVM MapFromEntity(vm_Zone entity)
        {
            if (entity == null) return null;

            return new ZoneVM()
            {
                Id = entity.Id,
                Name = entity.vm_ZoneName != null ? entity.vm_ZoneName : "لا يوجد",
                Code = entity.vm_ZoneCode ?? "لا يوجد",
                ExcludeFromOfficeSelection = entity.vm_ExcludeFromOfficeSelection ?? false,

                BuildingId = entity.vm_Building != null ? entity.vm_Building.Id : Guid.Empty,
                BuildingName = entity.vm_Building != null ? entity.vm_Building.Name : "لا يوجد",
            };
        }
    }
}