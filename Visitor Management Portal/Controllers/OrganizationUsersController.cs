using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.Controllers
{
    

    public class OrganizationUsersController : Controller
    {
        private readonly IOrganizationUsersRepository organizationUsersRepository;

        public OrganizationUsersController(IOrganizationUsersRepository organizationUsersRepository)
        {
            this.organizationUsersRepository = organizationUsersRepository;
        }
        public ActionResult Index()
        {
            try
            {
                var userId = ClaimsManager.GetUserId();
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID is invalid.");
                }

                var result = organizationUsersRepository.GetOrganizationUsers(userId);
                if (result == null || !result.Any())
                {
                    ViewBag.ErrorMessage = "No organization users found.";
                    return View(new List<OrganizationUserVM>()); 
                }

                return View(result);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while retrieving organization users.";
                return View(new List<OrganizationUserVM>());
            }
        }

        public ActionResult InviteUsers()
        {
            return View(); 
        }

        public ActionResult UserDetails(Guid userId)
        {
            OrganizationUserDetailsVM result = organizationUsersRepository.GetOrganizationUserDetails(userId);
            return View(result);

        }

        public ActionResult EditUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user ID provided.");
                }

                OrganizationUserDetailsVM result = organizationUsersRepository.GetOrganizationUserInfo(userId);
                if (result == null)
                {
                    throw new KeyNotFoundException($"User with ID {userId} not found.");
                }

                return View(result);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching user details.", ex);
            }
        }

        [HttpPost]
        public JsonResult UpdateUserInfo(OrganizationUserDetailsVM organizationUserDetailsVM)
        {
            try
            {
                if (organizationUserDetailsVM == null || organizationUserDetailsVM.id== Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(organizationUserDetailsVM), "Invalid user details provided.");
                }

                bool isUpdated = organizationUsersRepository.UpdateOrganizationUserInfo(organizationUserDetailsVM);
                if (!isUpdated)
                {
                    throw new InvalidOperationException("Failed to update user information.");
                }

                return Json(new { success = true, message = "User information updated successfully" });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating user information.", ex);
            }
        }
        [HttpPost]
        public JsonResult DeleteUser(Guid userId)
        {
            try
            {
               
                bool isDeleted = organizationUsersRepository.DeleteOrganizationUser(userId);
                if (!isDeleted)
                {
                    throw new InvalidOperationException("Failed to delete user.");
                }

                return Json(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting user.", ex);   
            }
        }


    }
}