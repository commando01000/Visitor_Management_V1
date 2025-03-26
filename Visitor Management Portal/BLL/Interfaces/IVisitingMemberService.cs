using System;
using System.Web.Mvc;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.VisitingMember;
using Visitor_Management_Portal.ViewModels.VisitorsHub;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IVisitingMemberService
    {
        VisitingMemberDataVM GetVisitingMemberByShortCode(string visitingMemberId);
        OperationResult AcceptInvitation(VisitorVM visitorVM, Guid visitingMemberId);
        OperationResult RejectInvitation(Guid visitingMemberId);
        VisitorProfileVM VisitorProfileByCode(string code);
        VisitorTokenVM VisitorTokenByCode(string code);
        JsonResult GenerateQrCode(string visitorShortCode, Guid visitingMemberId);
        OperationResult RemoveVisitRequestVisitors(Guid VisitorId, Guid VisitRequestId);
    }
}
