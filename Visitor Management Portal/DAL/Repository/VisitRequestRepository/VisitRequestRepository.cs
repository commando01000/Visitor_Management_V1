using CrmEarlyBound;
using D365_Add_ons.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using D365_Add_ons.Repository;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.VisitRequest;
using XDesk.Helpers;

namespace Visitor_Management_Portal.DAL.Repository.VisitRequestRepository
{
    public class VisitRequestRepository : BaseRepository<vm_VisitRequest>, IVisitRequestRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public VisitRequestRepository() : base()
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }

        // Get Visit Requests
        public Task<EntityCollection> GetVisitRequests(Guid UserID)
        {
            var result = GetOrganizationByUserID(UserID);
            Guid OrganizationID = result.vm_Organization.Id;
            var organizationName = result.vm_Organization.Name;

            string fetchXml = @"
                <fetch top='50'>
                  <entity name='vm_visitrequest'>
                    <attribute name='statuscode' />
                    <attribute name='vm_approvedrejectedby' />
                    <attribute name='vm_approvedrejectedon' />
                    <attribute name='vm_newcolumn' />
                    <attribute name='vm_requestedby' />
                    <attribute name='vm_subject' />
                    <attribute name='vm_visitpurpose' />
                    <attribute name='vm_visitrequestid' />
                    <attribute name='vm_visittime' />
                    <attribute name='vm_visituntil' />
                    <attribute name='vm_location' />
                    <filter>
                      <condition entityname='requestedBy' attribute='vm_organization' operator='eq' value='" + OrganizationID + @"' uiname='initium' uitype='vm_organization' />
                    </filter>
                    <link-entity name='vm_organizationuser' from='vm_organizationuserid' to='vm_requestedby' alias='requestedBy' />
                  </entity>
                </fetch>";

            var fetchExpression = new FetchExpression(fetchXml);
            var visitRequestsResult = _service.RetrieveMultiple(fetchExpression);

            return Task.FromResult(visitRequestsResult);
        }

        public vm_VisitRequest GetVisitRequestDetails(Guid VisitRequestId)
        {
            var visitRequest = _context.vm_VisitRequestSet.Where(r => r.Id == VisitRequestId).FirstOrDefault();

            return visitRequest;
        }

        private List<VisitorVM> GetVisitorsDetails(Guid visitRequestId)
        {
            List<VisitorVM> visitors = new List<VisitorVM>();
            var ids = _context.vm_visitingmemberSet.Where(v => v.vm_VisitRequest.Id == visitRequestId).Select(s => s.vm_Visitor.Id).ToList();

            foreach (var id in ids)
            {

                var visitor = _context.vm_VisitorSet.Where(v => v.Id == id).FirstOrDefault();
                var visitorVM = new VisitorVM
                {
                    VisitorId = visitor.Id,
                    VisitorName = visitor.vm_FullName ?? "",
                    Email = visitor.vm_Email ?? "",
                    IdNumber = visitor.vm_IDNumber ?? "",
                    Status = visitor.StatusCode.ToString() ?? "",
                    Organization = visitor.vm_Organization

                };
                visitors.Add(visitorVM);
            }

            return visitors;
        }

        public dynamic GetOrganizationByUserID(Guid UserID)
        {
            var result = _context.vm_organizationuserSet.Where(u => u.Id == UserID).FirstOrDefault();
            return result;
        }

        public List<vm_organizationuser> GetOrganizationUsers(Guid currentUserID)
        {
            try
            {
                Guid organizationID = GetOrganizationByUserID(currentUserID).vm_Organization.Id;

                var result = _context.vm_organizationuserSet
                    .Where(u => u.vm_Organization.Id == organizationID)
                    .Select(u => new vm_organizationuser
                    {
                        Id = u.Id,
                        vm_name = u.vm_name
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {

                return new List<vm_organizationuser>();
            }
        }

        public Task<vm_organizationuser> GetCurrentOfficeLocation(Guid userID)
        {
            try
            {
                var user = _context.vm_organizationuserSet.FirstOrDefault(u => u.Id == userID);

                // Check if user exists
                if (user == null)
                {
                    return Task.FromResult(new vm_organizationuser());
                }

                // Check if necessary related data exists
                //return Task.FromResult(new CurrentOfficeLocationVM
                //{
                //    Building = user.vm_Building?.Name ?? " Building ",
                //    BuildingId = user.vm_Building?.Id ?? Guid.Empty,
                //    Zone = user.vm_Zone?.Name ?? " Zone",
                //    ZoneId = user.vm_Zone?.Id ?? Guid.Empty,
                //    Floor = user.vm_FloorNumber.HasValue ? user.vm_FloorNumber.ToString() : "",
                //});

                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new vm_organizationuser());
            }
        }

        public List<Guid> GetVisitorMembers(Guid VisitRequestId)
        {
            var result = _context.vm_visitingmemberSet.Where(v => v.vm_VisitRequest.Id == VisitRequestId).Select(s => s.vm_Visitor.Id).ToList();
            return result;
        }

        public vm_Visitor GetVisitingMember(Guid VisitorId)
        {
            var result = _context.vm_VisitorSet.Where(v => v.Id == VisitorId).FirstOrDefault();
            return result;
        }
    }
}