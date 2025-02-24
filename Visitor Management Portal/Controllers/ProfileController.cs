using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Visitor_Management_Portal.DAL.Repository.ProfileRepository;
using Visitor_Management_Portal.ViewModels.Profile;

namespace Visitor_Management_Portal.Controllers
{
    

    public class ProfileController : Controller
    {
        private readonly IProfileRepository profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            this.profileRepository = profileRepository;
        }
        public ActionResult Index()
        {
            try
            {
                Guid userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    throw new Exception("User identity not found");
                }

                var organizationUserInfo = profileRepository.GetProfileInfo(userId);
                if (organizationUserInfo == null)
                {
                    throw new Exception("Profile information not found");
                }

                int completedFields = CalculateCompletedFields(organizationUserInfo);
                int totalFields = 8;
                double completenessPercentage = Math.Round((double)completedFields / totalFields * 100, 0);

                ViewBag.CompletenessPercentage = completenessPercentage;
                return View(organizationUserInfo);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JsonResult UpdateProfile(ProfileInfoVM profileInfoVM)
        {
            try
            {
                Guid userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Json(new { success = false, message = "Failed to update profile" });
                }

                bool isUpdated = profileRepository.UpdateProfileInfo(profileInfoVM, userId);
                if (!isUpdated)
                {
                    return Json(new { success = false, message = "Failed to update profile" });
                }

                var updatedProfileInfo = profileRepository.GetProfileInfo(userId);
                int completedFields = CalculateCompletedFields(updatedProfileInfo);
                int totalFields = 8;
                double completenessPercentage = Math.Round((double)completedFields / totalFields * 100, 0);

                return Json(new { success = true, message = "Profile updated successfully", completenessPercentage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordVM changePasswordVM)
        {
            try
            {
                Guid userID = GetCurrentUserId();
                if (userID == Guid.Empty)
                {
                    return Json(new { success = false, message = "Failed to update profile" });
                }
                bool isUpdated = profileRepository.ChangePassword(changePasswordVM, userID);
                if (isUpdated)
                {
                    return Json(new { success = true, message = "Password changed successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to change password" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private int CalculateCompletedFields(ProfileInfoVM profileInfo)
        {
            int completedFields = 0;

            if (!string.IsNullOrEmpty(profileInfo.Name)) completedFields++;
            if (!string.IsNullOrEmpty(profileInfo.EmailAddress)) completedFields++;
            if (!string.IsNullOrEmpty(profileInfo.PhoneNumber)) completedFields++;
            if (!string.IsNullOrEmpty(profileInfo.Password)) completedFields++;
            if (!string.IsNullOrEmpty(profileInfo.JobTitle)) completedFields++;
            if (profileInfo.FloorNumber.HasValue) completedFields++;
            if (profileInfo.Building != null) completedFields++;
            if (profileInfo.Zone != null) completedFields++;

            return completedFields;
        }


        private Guid GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("User identity not found");
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return new Guid(userIdClaim);
        }

    }
} 