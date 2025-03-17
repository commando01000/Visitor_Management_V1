using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;
using Visitor_Management_Portal.ViewModels.VisitorsHub;
using Visitor_Management_Portal.ViewModels.VisitRequest;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IVisitorsService
    {
        List<VisitorsHubVM> GetByOrganization();
        List<OptionSet<Guid>> GetVisitors();
        List<OrganizationUsersVM> GetOrganizationUsers();
        OperationResult<Guid> Add(AddVisitorVM model);
        CurrentOfficeLocationVM GetCurrentOfficeLocation();
        OperationResult AddVisitRequest(AddVisitRequestVM model);
        VisitRequestDetailsVM GetVisitRequestDetails(Guid visitRequestId);
        List<VisitRequestVM> GetVisitRequests();
        List<OptionSet> GetVisitRequestPurposes();
        int GetVisitorCount(Guid visitRequestId);
        OperationResult UpdateVisitRequest(AddVisitRequestVM model);
        ViewModels.VisitorsHub.VisitorVM GetVisitor(Guid visitRequestId);
        OperationResult UpdateVisitor(ViewModels.VisitorsHub.VisitorVM visitorsVM);
        List<VisitingMemberWithRelatedRequestVM> GetVisitorRequestsHistory(Guid visitorId);
        OperationResult DeleteVisitorRequest(Guid visitRequestId);
        OperationResult DeleteVisitorHub(Guid visitorId);
        List<VisitRequestVM> GetVisitRequestsFiltered(VisitRequestVM visitRequestVM);
        List<VisitorsHubVM> GetVisitorsHubFiltered(VisitorsHubVM visitorsHubVM);
    }
}
