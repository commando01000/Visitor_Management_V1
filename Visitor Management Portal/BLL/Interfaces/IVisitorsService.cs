using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.VisitorsHub;
using Visitor_Management_Portal.ViewModels.VisitRequest;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IVisitorsService
    {
        List<VisitorsHubVM> GetByOrganization();
        List<OrganizationUsersVM> GetOrganizationUsers();
        OperationResult<Guid> Add(AddVisitorVM model);
        Task<CurrentOfficeLocationVM> GetCurrentOfficeLocation();
        Task<OperationResult> AddVisitRequest(AddVisitRequestVM model);
        VisitRequestDetailsVM GetVisitRequestDetails(Guid visitRequestId);
        Task<List<VisitRequestVM>> GetVisitRequests();
        List<OptionSet> GetVisitRequestPurposes();
        int GetVisitorCount(Guid visitRequestId);
        Task<OperationResult> UpdateVisitRequest(AddVisitRequestVM model);
        ViewModels.VisitorsHub.VisitorVM GetVisitor(Guid visitRequestId);
        OperationResult UpdateVisitor(ViewModels.VisitorsHub.VisitorVM visitorsVM);
        Task<List<VisitRequestVM>> GetVisitorRequestsHistory(Guid visitorId);
    }
}
