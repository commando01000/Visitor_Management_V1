using System;
using System.Web.Mvc;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.DAL.Repository.OrganizationDate;
using Visitor_Management_Portal.ViewModels.OrganizationDate;

namespace Visitor_Management_Portal.Controllers
{
    

    public class OrganizationDateController : Controller
    {
        private readonly IOrganizationDataRepository organizationDate;

        public OrganizationDateController(IOrganizationDataRepository organizationDate)
        {
            this.organizationDate = organizationDate;
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
        public JsonResult updateOrganizationDate(OrganizationDataVM organizationDataVM)
        {
            try
            {
                if (organizationDataVM == null)
                {
                    return Json(new { success = false, message = "Invalid data received" });
                }

                organizationDate.UpdateOrganizationDate(organizationDataVM);

                return Json(new { success = true, message = "Organization data updated successfully" });
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