using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using System;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class EditZoneVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool ExcludeFromOfficeToggle { get; set; }
        public Guid BuildingId { get; set; }

        public static vm_Zone MapToEntity(EditZoneVM model)
        {
            if (model == null) return null;

            return new vm_Zone()
            {
                vm_ZoneName = model.Name,
                vm_ZoneCode = model.Code ?? string.Empty,
                vm_ExcludeFromOfficeSelection = model.ExcludeFromOfficeToggle,
                vm_Building = new EntityReference("vm_building", model.BuildingId),
            };
        }
    }
}