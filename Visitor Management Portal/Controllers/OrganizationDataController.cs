using System;
using System.Web.Mvc;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.DAL.Repository.OrganizationDate;
using Visitor_Management_Portal.ViewModels.OrganizationDate;
using Visitor_Management_Portal.BLL.Interfaces;

namespace Visitor_Management_Portal.Controllers
{


    public class OrganizationDataController : Controller
    {
        private readonly IOrganizationDataRepository organizationDate;
        private readonly IOrganizationService _organizationService;

        public OrganizationDataController(IOrganizationDataRepository organizationDate, IOrganizationService organizationService)
        {
            this.organizationDate = organizationDate;
            _organizationService = organizationService;
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

                OrganizationDataVM result = organizationDate.GetOrganizationDate(userId);
                if (result == null)
                {
                    ViewBag.ErrorMessage = "No organization data found for the user.";
                    return View(new OrganizationDataVM());
                }

                return View(result);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while retrieving organization data.";
                return View(new OrganizationDataVM());
            }
        }


        [HttpPost]
        public JsonResult updateOrganizationData(OrganizationDataVM organizationDataVM)
        {
            try
            {
                if (organizationDataVM == null)
                {
                    return Json(new { success = false, message = "Invalid data received" });
                }

                bool IsOrganizationUpdated = _organizationService.UpdateOrganizationData(organizationDataVM);

                if (IsOrganizationUpdated)
                {
                    return Json(new { success = true, message = "Organization data updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Organization domain name already exists" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating organization data" });
            }
        }


        public ActionResult DataverseConnection()
        {
            return View();
        }


    }
}