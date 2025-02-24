using CrmEarlyBound;
using System;
using XDesk.Helpers;

namespace Visitor_Management_Portal.ViewModels.OrganizationUsers
{
    public class OrganizationUserVM
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        public Guid OranizationId { get; set; } = Guid.Empty;
        public string OrganizationName { get; set; } = string.Empty;

        public static OrganizationUserVM MapFromEntity(vm_organizationuser entity)
        {
            return new OrganizationUserVM
            {
                Id = entity.Id,
                FullName = entity.vm_name ?? string.Empty,
                Email = entity.vm_EmailAddress ?? string.Empty,

                RoleName = entity.vm_Role != null ?
                CustomEnumHelpers.GetEnumNameByValueCRM<vm_organizationuser_vm_Role>(entity.vm_Role.Value.ToString()) :
                string.Empty,

                OranizationId = entity.vm_Organization?.Id ?? Guid.Empty,
                OrganizationName = entity.vm_Organization?.Name ?? string.Empty
            };
        }
    }
}