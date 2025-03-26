using CrmEarlyBound;
using System;
using System.Web.Mvc;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.VisitingMember;
using Visitor_Management_Portal.ViewModels.VisitorsHub;

namespace Visitor_Management_Portal.Controllers
{
    /// <summary>
    /// This controller for public pages of visitor invitation and qr/token
    /// </summary>
    public class VisitorController : Controller
    {
        private readonly IVisitingMemberService _visitingMemberService;

        public VisitorController(IVisitingMemberService visitingMemberService)
        {
            _visitingMemberService = visitingMemberService;
        }

        [HttpGet]
        public ActionResult Invite(string id)
        {
            VisitingMemberDataVM model = _visitingMemberService.GetVisitingMemberByShortCode(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Visit request not available or incorrect url";
                return RedirectToAction("Index", "Account");
            }
            if (model.VisitingMemebrerStatusCode != (int)vm_visitingmember_StatusCode.Invited)
            {
                TempData["ErrorMessage"] = "Invitation is not available anymore!";
                return RedirectToAction("Index", "Account");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AcceptInvitation(VisitorVM model, Guid visitingMemberId)
        {
            OperationResult result = _visitingMemberService.AcceptInvitation(model, visitingMemberId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RejectInvitation(Guid visitingMemberId)
        {
            OperationResult result = _visitingMemberService.RejectInvitation(visitingMemberId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Information(string id)
        {
            VisitorTokenVM model = _visitingMemberService.VisitorTokenByCode(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Visitor token not exit";
                return RedirectToAction("Index","Account");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Profile(string id)
        {
            VisitorProfileVM model = _visitingMemberService.VisitorProfileByCode(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Visitor not exit";
                return RedirectToAction("Index", "Account");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult GenerateQRCode(string shortCode,Guid visitingMemberId)
        {
            try
            {
                JsonResult qrCodeResponse = _visitingMemberService.GenerateQrCode(shortCode, visitingMemberId);

                return qrCodeResponse;
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        Success = false,
                        Message = "Failed to generate QR Code",
                        Error = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}