using CrmEarlyBound;
using D365_Add_ons.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;
using Visitor_Management_Portal.ViewModels.Profile;
using BuildingVM = Visitor_Management_Portal.ViewModels.OrganizationSetup.BuildingVM;

namespace Visitor_Management_Portal.DAL.Repository.ProfileRepository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public ProfileRepository()
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }

   

        public ProfileInfoVM GetProfileInfo(Guid userId)
        {
            var result = _context.vm_organizationuserSet
                                .Where(u => u.vm_organizationuserId == userId)
                                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }
            var profileInfoVM = new ProfileInfoVM
            {
                JobTitle = result.vm_JobTitle,
                Name = result.vm_name,
                EmailAddress = result.vm_EmailAddress,
                Password = result.vm_Password,
                PhoneNumber = result.vm_PhoneNumber,
                FloorNumber = result.vm_FloorNumber,
                Building = result.vm_Building != null ? new BuildingVM { Id = result.vm_Building.Id, Name = result.vm_Building.Name } : null,
                Zone = result.vm_Zone != null ? new ZoneVM { Id = result.vm_Zone.Id, Name = result.vm_Zone.Name } : null
            };

            return profileInfoVM;
        }
        public bool UpdateProfileInfo(ProfileInfoVM profileInfoVM, Guid userId)
        {
            var existingUser = _context.vm_organizationuserSet
                                       .Where(u => u.vm_organizationuserId == userId) 
                                       .FirstOrDefault();

            if (existingUser == null)
            {
                return false;
            }

            bool isChanged = false;

            if (existingUser.vm_JobTitle != profileInfoVM.JobTitle)
            {
                existingUser.vm_JobTitle = profileInfoVM.JobTitle;
                isChanged = true;
            }

            if (existingUser.vm_name != profileInfoVM.Name)
            {
                existingUser.vm_name = profileInfoVM.Name;
                isChanged = true;
            }

            if (existingUser.vm_EmailAddress != profileInfoVM.EmailAddress)
            {
                existingUser.vm_EmailAddress = profileInfoVM.EmailAddress;
                isChanged = true;
            }

            if (existingUser.vm_PhoneNumber != profileInfoVM.PhoneNumber)
            {
                existingUser.vm_PhoneNumber = profileInfoVM.PhoneNumber;
                isChanged = true;
            }

            if (existingUser.vm_FloorNumber != profileInfoVM.FloorNumber)
            {
                existingUser.vm_FloorNumber = profileInfoVM.FloorNumber;
                isChanged = true;
            }

            if (profileInfoVM.Building != null && (existingUser.vm_Building == null || existingUser.vm_Building.Id != profileInfoVM.Building.Id))
            {
                existingUser.vm_Building = new EntityReference("vm_building", profileInfoVM.Building.Id);
                isChanged = true;
            }
      

            if (profileInfoVM.Zone != null && (existingUser.vm_Zone == null || existingUser.vm_Zone.Id != profileInfoVM.Zone.Id))
            {
                existingUser.vm_Zone = new EntityReference("vm_zone", profileInfoVM.Zone.Id);
                isChanged = true;
            }
      

            if (!isChanged)
            {
                return false;
            }

            _context.UpdateObject(existingUser);

            try
            {
                _service.Update(existingUser);
                return true;
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }


        public bool ChangePassword(ChangePasswordVM changePasswordVM, Guid userId)
        {
            var existingUser = _context.vm_organizationuserSet
                                       .Where(u => u.vm_organizationuserId == userId)
                                       .FirstOrDefault();

            if (existingUser == null)
            {
                return false;
            }
            var oldPasswordHashed = DataEncryptionHelper.Encryptdata(changePasswordVM.OldPassword);
            if(existingUser.vm_Password != oldPasswordHashed)
            {
                return false;
            }
            existingUser.vm_Password = DataEncryptionHelper.Encryptdata(changePasswordVM.NewPassword);

            _context.UpdateObject(existingUser);

            try
            {
                _service.Update(existingUser);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
