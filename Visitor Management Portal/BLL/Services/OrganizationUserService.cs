using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using System;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.BLL.Services
{
    public class OrganizationUserService : IOrganizationUserService
    {
        private readonly IOrganizationUsersRepository _organizationUsersRepository;

        public OrganizationUserService(IOrganizationUsersRepository organizationUsersRepository)
        {
            _organizationUsersRepository = organizationUsersRepository;
        }

        public OperationResult CreateUser(OrganizationUserDetailsVM organizationUserDetailsVM)
        {

            // check email exists or not
            if (_organizationUsersRepository.Get(u => u.vm_EmailAddress == organizationUserDetailsVM.Email) != null)
            {
                return new OperationResult()
                {
                    Status = false,
                    Message = "Email already exists"
                };
            }

            var user = new vm_organizationuser()
            {
                vm_name = organizationUserDetailsVM.Name,
                vm_EmailAddress = organizationUserDetailsVM.Email,
                vm_Role = (vm_organizationuser_vm_Role)organizationUserDetailsVM.RoleId,
                vm_CreateVisitsWithoutApproval = true, // default
                vm_Building = new EntityReference(vm_Building.EntityLogicalName, organizationUserDetailsVM.BuildingId),
                vm_Zone = new EntityReference(vm_Zone.EntityLogicalName, organizationUserDetailsVM.ZoneId),
                vm_FloorNumber = int.TryParse(organizationUserDetailsVM.Floor, out int floor) ? floor : (int?)null,
                vm_JobTitle = organizationUserDetailsVM.JobTitle,
                vm_Password = DataEncryptionHelper.Encryptdata(organizationUserDetailsVM.Password),
                vm_ReportingTo = new EntityReference(vm_organizationuser.EntityLogicalName, organizationUserDetailsVM.ReportingtoId),
                vm_Organization = new EntityReference(vm_Organization.EntityLogicalName, ClaimsManager.GetOrganizationId())
            };

            try
            {
                var result = _organizationUsersRepository.Create(user);
                return new OperationResult()
                {
                    Status = true,
                    Message = "User has been created successfully",
                };
            }
            catch (Exception ex)
            {
                return new OperationResult()
                {
                    Status = false,
                    Message = "Error adding user, try again later"
                };
            }
        }

        public bool UpdateApprovalStatus(OrganizationUserDetailsVM organizationUserDetailsVM)
        {
            var user = _organizationUsersRepository.Get(u => u.Id == organizationUserDetailsVM.id);

            if (user == null)
            {
                return false;
            }

            user.vm_CreateVisitsWithoutApproval = !organizationUserDetailsVM.CreateVisitsWithoutApproval;

            return _organizationUsersRepository.Update(user);
        }

    }
}
