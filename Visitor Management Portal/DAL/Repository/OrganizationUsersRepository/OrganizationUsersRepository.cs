using CrmEarlyBound;
using D365_Add_ons.Connection;
using D365_Add_ons.Repository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;
using Visitor_Management_Portal.ViewModels.VisitRequest;
using XDesk.Helpers;

namespace Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository
{
    public class OrganizationUsersRepository : BaseRepository<vm_organizationuser>, IOrganizationUsersRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public OrganizationUsersRepository()
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }

        public List<OrganizationUserVM> GetByOrganizationId(Guid organizationId)
        {
            var result = _context.vm_organizationuserSet
                .Where(x => x.vm_Organization != null && x.vm_Organization.Id == organizationId && x.StateCode == vm_organizationuserState.Active)
                .Select(user => new OrganizationUserVM
                {
                    Id = user.Id,
                    FullName = user.vm_name,
                    RoleName = user.vm_Role != null ? CustomEnumHelpers.GetEnumNameByValueCRM<vm_organizationuser_vm_Role>(user.vm_Role.Value.ToString()) : string.Empty,

                }).ToList();

            return result;
        }

        public List<OrganizationUserVM> GetOrganizationUsers(Guid userid)
        {
            var organizationID = GetOrganizationByUserID(userid);

            var organizationUsers = _context.vm_organizationuserSet
                .Where(user => user.vm_Organization.Id == organizationID)
                .ToList();

            var organizationUserVMs = new List<OrganizationUserVM>();

            foreach (var user in organizationUsers)
            {
                organizationUserVMs.Add(OrganizationUserVM.MapFromEntity(user));
            }

            return organizationUserVMs;
        }

        public Guid GetOrganizationByUserID(Guid UserID)
        {
            var result = _context.vm_organizationuserSet.Where(u => u.Id == UserID).FirstOrDefault();
            return result.vm_Organization.Id;
        }


        // Get Visit Reques
        public OrganizationUserDetailsVM GetOrganizationUserDetails(Guid UserID)
        {
            OrganizationUserDetailsVM organizationUserDetailsVM = new OrganizationUserDetailsVM();

            var user = _context.vm_organizationuserSet.Where(u => u.Id == UserID).FirstOrDefault();
            organizationUserDetailsVM.CreateVisitsWithoutApproval = !user.vm_CreateVisitsWithoutApproval.Value;
            organizationUserDetailsVM.id = user.Id;
            organizationUserDetailsVM.Floor = user.vm_FloorNumber?.ToString() ?? "";
            organizationUserDetailsVM.Name = user.vm_name ?? "";
            organizationUserDetailsVM.JobTitle = user.vm_JobTitle ?? "";
            organizationUserDetailsVM.Email = user.vm_EmailAddress ?? "";
            organizationUserDetailsVM.Role = user.vm_Role.HasValue ? user.vm_Role.ToString() : string.Empty;
            organizationUserDetailsVM.RoleId = user.vm_Role.HasValue ? (int)user.vm_Role.Value : 0;
            organizationUserDetailsVM.Reportingto = user.vm_ReportingTo?.Name ?? "";
            organizationUserDetailsVM.ReportingtoId = user.vm_ReportingTo?.Id ?? Guid.Empty;

            organizationUserDetailsVM.Building = user.vm_Building?.Name ?? "";
            organizationUserDetailsVM.BuildingId = user.vm_Building?.Id ?? Guid.Empty;
            organizationUserDetailsVM.Password = user.vm_Password ?? "";
            organizationUserDetailsVM.Email = user.vm_EmailAddress ?? "";
            organizationUserDetailsVM.Phone = user.vm_PhoneNumber ?? "";
            organizationUserDetailsVM.Zone = user.vm_Zone?.Name ?? "";
            organizationUserDetailsVM.ZoneId = user.vm_Zone?.Id ?? Guid.Empty;
            organizationUserDetailsVM.id = user.Id;



            organizationUserDetailsVM.visitRequests = GetVisitRequests(UserID);
            return organizationUserDetailsVM;
        }
        private List<VisitRequestVM> GetVisitRequests(Guid UserID)
        {
            var result = GetOrganizationObjectByUserID(UserID);
            Guid OrganizationID = result.vm_Organization.Id;
            var organizationName = result.vm_Organization.Name;

            string fetchXml = @"
                    <fetch>
                          <entity name='vm_visitrequest'>
                            <attribute name='statuscode' />
                            <attribute name='vm_approvedrejectedby' />
                            <attribute name='vm_approvedrejectedon' />
                            <attribute name='vm_newcolumn' />
                            <attribute name='vm_requestedby' />
                            <attribute name='vm_subject' />
                            <attribute name='vm_visitpurpose' />
                            <attribute name='vm_visitorscount' />
                            <attribute name='vm_visitrequestid' />
                            <attribute name='vm_visittime' />
                            <attribute name='vm_visituntil' />
                            <attribute name='vm_location' />
                            <filter>
                             <condition  attribute='vm_requestedby' operator='eq' value='" + UserID + @"'  uitype='vm_organizationuser' />
                            </filter>
                            <link-entity name='vm_organizationuser' from='vm_organizationuserid' to='vm_requestedby' alias='requestedBy' />
                             </entity>
                    </fetch>";

            var fetchExpression = new FetchExpression(fetchXml);
            var visitRequestsResult = _service.RetrieveMultiple(fetchExpression);

            var visitRequests = visitRequestsResult.Entities.Select(e => new VisitRequestVM
            {
                Serial = e.GetAttributeValue<string>("vm_newcolumn"),
                RequestdBy = e.GetAttributeValue<EntityReference>("vm_requestedby")?.Name,
                Organization = organizationName,
                Purpose = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(e.GetAttributeValue<OptionSetValue>("vm_visitpurpose")?.Value ?? 0),
                //Date = e.GetAttributeValue<DateTime?>("vm_visittime")?.ToString("yyyy-MM-dd"),
                //Time = e.GetAttributeValue<DateTime?>("vm_visittime")?.ToString("hh:mm tt"),
                Date = e.GetAttributeValue<DateTime?>("vm_visittime").Value,
                Time = e.GetAttributeValue<DateTime?>("vm_visittime").Value,
                Duration = CalculateDuration(e.GetAttributeValue<DateTime?>("vm_visittime"), e.GetAttributeValue<DateTime?>("vm_visituntil")),
                Location = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_vm_Location>(e.GetAttributeValue<OptionSetValue>("vm_location")?.Value ?? 0),
                Status = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_StatusCode>(e.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? 0),
                ApprovedBy = e.GetAttributeValue<EntityReference>("vm_approvedrejectedby")?.Name,
                //VisitorsCount = GetVisitorCount(e.GetAttributeValue<Guid>("vm_visitrequestid"))
                VisitorsCount = e.GetAttributeValue<int>("vm_visitorscount") // Updated VisitorCount without the need of another API Call
            }).ToList();

            return visitRequests;
        }

        // Helper Functions -> 
        private int GetVisitorCount(Guid visitRequestId)
        {
            string fetchXml = @"
            <fetch>
              <entity name='vm_visitingmember'>
                <attribute name='vm_visitor' />
                <filter>
                  <condition attribute='vm_visitrequest' operator='eq' value='" + visitRequestId + @"' />
                </filter>
              </entity>
            </fetch>";

            var fetchExpression = new FetchExpression(fetchXml);
            var result = _service.RetrieveMultiple(fetchExpression);

            return result.Entities.Count;
        }


        private string CalculateDuration(DateTime? startTime, DateTime? endTime)
        {
            if (startTime == null || endTime == null)
                return "N/A";

            TimeSpan duration = (endTime.Value - startTime.Value).Duration();

            if (duration.Minutes == 0)
            {
                return $"{duration.Hours} h";
            }

            return $"{duration.Hours} h {duration.Minutes} m";
        }

        public dynamic GetOrganizationObjectByUserID(Guid UserID)
        {
            var result = _context.vm_organizationuserSet.Where(u => u.Id == UserID).FirstOrDefault();
            return result;
        }

        public OrganizationUserDetailsVM GetOrganizationUserInfo(Guid UserID)
        {
            OrganizationUserDetailsVM organizationUserDetailsVM = new OrganizationUserDetailsVM();

            var user = _context.vm_organizationuserSet.Where(u => u.Id == UserID).FirstOrDefault();
            organizationUserDetailsVM.CreateVisitsWithoutApproval = !user.vm_CreateVisitsWithoutApproval.Value;
            organizationUserDetailsVM.Floor = user.vm_FloorNumber?.ToString() ?? "";
            organizationUserDetailsVM.Name = user.vm_name ?? "";
            organizationUserDetailsVM.JobTitle = user.vm_JobTitle ?? "";
            organizationUserDetailsVM.Email = user.vm_EmailAddress ?? "";
            organizationUserDetailsVM.Role = user.vm_Role.HasValue ? user.vm_Role.ToString() : string.Empty;
            organizationUserDetailsVM.RoleId = user.vm_Role.HasValue ? (int)user.vm_Role.Value : 0;
            organizationUserDetailsVM.Reportingto = user.vm_ReportingTo?.Name ?? "";
            organizationUserDetailsVM.ReportingtoId = user.vm_ReportingTo?.Id ?? Guid.Empty;

            organizationUserDetailsVM.Building = user.vm_Building?.Name ?? "";
            organizationUserDetailsVM.BuildingId = user.vm_Building?.Id ?? Guid.Empty;
            organizationUserDetailsVM.Password = user.vm_Password ?? "";
            organizationUserDetailsVM.Email = user.vm_EmailAddress ?? "";
            organizationUserDetailsVM.Phone = user.vm_PhoneNumber ?? "";
            organizationUserDetailsVM.Zone = user.vm_Zone?.Name ?? "";
            organizationUserDetailsVM.ZoneId = user.vm_Zone?.Id ?? Guid.Empty;
            organizationUserDetailsVM.id = user.Id;


            organizationUserDetailsVM.visitRequests = new List<VisitRequestVM>();
            return organizationUserDetailsVM;
        }

        public bool UpdateOrganizationUserInfo(OrganizationUserDetailsVM organizationUserDetailsVM)
        {
            var user = _context.vm_organizationuserSet.Where(u => u.Id == organizationUserDetailsVM.id).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            user.vm_CreateVisitsWithoutApproval = !organizationUserDetailsVM.CreateVisitsWithoutApproval;
            user.vm_FloorNumber = string.IsNullOrEmpty(organizationUserDetailsVM.Floor) ? (int?)null : int.Parse(organizationUserDetailsVM.Floor);
            user.vm_name = organizationUserDetailsVM.Name;
            user.vm_JobTitle = organizationUserDetailsVM.JobTitle;
            user.vm_EmailAddress = organizationUserDetailsVM.Email;

            user.vm_Role = (CrmEarlyBound.vm_organizationuser_vm_Role?)organizationUserDetailsVM.RoleId;

            if (organizationUserDetailsVM.ReportingtoId != Guid.Empty)
            {
                user.vm_ReportingTo = new EntityReference("vm_organizationuser", organizationUserDetailsVM.ReportingtoId);
            }
            else
            {
                user.vm_ReportingTo = null;
            }

            if (organizationUserDetailsVM.BuildingId != Guid.Empty)
            {
                user.vm_Building = new EntityReference("vm_building", organizationUserDetailsVM.BuildingId);
            }
            else
            {
                user.vm_Building = null;
            }

            if (organizationUserDetailsVM.ZoneId != Guid.Empty)
            {
                user.vm_Zone = new EntityReference("vm_zone", organizationUserDetailsVM.ZoneId);
            }
            else
            {
                user.vm_Zone = null;
            }

            user.vm_PhoneNumber = organizationUserDetailsVM.Phone;

            _context.UpdateObject(user);
            _context.SaveChanges();

            return true;
        }

        public bool DeleteOrganizationUser(Guid UserID)
        {
            try
            {
                var user = _context.vm_organizationuserSet.Where(u => u.Id == UserID).FirstOrDefault();
                _context.DeleteObject(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}