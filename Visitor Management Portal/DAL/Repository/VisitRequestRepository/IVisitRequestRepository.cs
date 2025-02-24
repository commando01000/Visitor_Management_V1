using CrmEarlyBound;
using D365_Add_ons.Repository;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Visitor_Management_Portal.DAL.Repository.VisitRequestRepository
{
    public interface IVisitRequestRepository : IBaseRepository<vm_VisitRequest>
    {

        Task<EntityCollection> GetVisitRequests(Guid UserID);

        //Task<OperationResult> AddVisitRequest(vm_VisitRequest visitRequest);
        List<vm_organizationuser> GetOrganizationUsers(Guid userID);

        Task<vm_organizationuser> GetCurrentOfficeLocation(Guid userID);
        int GetVisitorCount(Guid visitRequestId);
        vm_VisitRequest GetVisitRequestDetails(Guid VisitRequestSerial);
        List<Guid> GetVisitorMembers(Guid VisitRequestId);
        vm_Visitor GetVisitingMember(Guid VisitorId);
    }
}
