using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using Visitor_Management_Portal.DAL.Repository.VisitorsHubRepository;
using Visitor_Management_Portal.DAL.Repository.VisitRequestRepository;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.VisitorsHub;
using Visitor_Management_Portal.ViewModels.VisitRequest;
using XDesk.Helpers;

namespace Visitor_Management_Portal.BLL.Services
{
    public class VisitorsService : IVisitorsService
    {
        private readonly IVisitorsHubRepository _visitorsHubRepository;
        private readonly IVisitRequestRepository _visitRequestRepository;
        private readonly IOrganizationUsersRepository _organizationUsersRepository;

        public VisitorsService(IVisitorsHubRepository visitorsHubRepository, IVisitRequestRepository visitRequestRepository, IOrganizationUsersRepository organizationUsersRepository)
        {
            _visitorsHubRepository = visitorsHubRepository;
            _visitRequestRepository = visitRequestRepository;
            _organizationUsersRepository = organizationUsersRepository;
        }

        public List<VisitorsHubVM> GetByOrganization()
        {
            var organizationId = ClaimsManager.GetOrganizationId();

            var query = new QueryExpression(vm_Visitor.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddCondition("vm_organizationref", ConditionOperator.Equal, organizationId);

            var entities = _visitorsHubRepository.GetAll(query);

            var visitors = entities.Select(e => new VisitorsHubVM
            {
                Id = e.Id,
                Name = e.GetAttributeValue<string>("vm_visitorfullname"),
                Email = e.GetAttributeValue<string>("vm_email"),
                IDNumber = e.GetAttributeValue<string>("vm_idnumber"),
                OrganizationName = e.Check("vm_organizationref") ? e.GetAttributeValue<EntityReference>("vm_organizationref").Name : string.Empty,

            }).ToList();

            return visitors;
        }

        public OperationResult<Guid> Add(AddVisitorVM model)
        {

            var nameParts = model.FullName?.Split(' ') ?? new string[0];

            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var middleName = nameParts.Length > 2 ? nameParts[1] : string.Empty;
            var lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : string.Empty;

            var visitor = new vm_Visitor
            {
                vm_FullName = firstName,
                vm_MiddleName = middleName,
                vm_LastName = lastName,
                vm_Email = model.EmailAddress,
                vm_JobTitle = model.JobTitle,
                vm_MobileNumber = model.PhoneNumber,
                vm_IDNumber = model.IDNumber,
                vm_Organization = model.OrganizationName,
                vm_OrganizationRef = new EntityReference(vm_Organization.EntityLogicalName, ClaimsManager.GetOrganizationId())

            };
            var createResultId = _visitorsHubRepository.Create(visitor);
            if (createResultId != Guid.Empty)
                return new OperationResult<Guid> { Status = true, Message = "Visitor has been created successfully", Data = createResultId };

            return new OperationResult<Guid> { Status = false, Message = "Failed to create visitor, Please try again" };
        }

        public async Task<CurrentOfficeLocationVM> GetCurrentOfficeLocation()
        {
            var UserId = ClaimsManager.GetUserId();

            var user = _organizationUsersRepository.Get(UserId);

            // mapping to view model
            return new CurrentOfficeLocationVM
            {
                Building = user.vm_Building.Name ?? "Building",
                BuildingId = user.vm_Building?.Id ?? Guid.Empty,
                Zone = user.vm_Zone?.Name ?? " Zone",
                ZoneId = user.vm_Zone?.Id ?? Guid.Empty,
                Floor = user.vm_FloorNumber.HasValue ? user.vm_FloorNumber.ToString() : "",
            };
        }

        public async Task<OperationResult> AddVisitRequest(AddVisitRequestVM addVisitRequestVM)
        {
            try
            {
                // Create a transaction request collection
                var transactionRequests = new OrganizationRequestCollection();

                // Create the visit request entity
                var visitRequest = new vm_VisitRequest
                {
                    Id = Guid.NewGuid(),
                    vm_Subject = addVisitRequestVM.Subject,
                    vm_VisitPurpose = (vm_VisitPurposes)addVisitRequestVM.Purpose,
                    vm_VisitTime = addVisitRequestVM.VisitTime,
                    vm_VisitUntil = addVisitRequestVM.VisitUntil,
                    vm_Location = (vm_VisitRequest_vm_Location)Enum.Parse(typeof(vm_VisitRequest_vm_Location), addVisitRequestVM.Location),
                    vm_RequestedBy = new EntityReference(vm_organizationuser.EntityLogicalName, addVisitRequestVM.RequestedBy),
                    StatusCode = (vm_VisitRequest_StatusCode)addVisitRequestVM.StatusReason
                };

                // Check if a meeting area is selected
                if (addVisitRequestVM.MeetingArea != Guid.Empty)
                {
                    visitRequest.vm_MeetingArea = new EntityReference(vm_MeetingArea.EntityLogicalName, addVisitRequestVM.MeetingArea);
                }

                // Create a request for visit request creation
                var createVisitRequest = new CreateRequest { Target = visitRequest };
                transactionRequests.Add(createVisitRequest);

                // Process each visitor and add it to the transaction
                foreach (var visitorId in addVisitRequestVM.VisitorsIds)
                {
                    var visitorMember = new vm_visitingmember
                    {
                        vm_VisitRequest = new EntityReference(vm_VisitRequest.EntityLogicalName, visitRequest.Id), // Will be linked after transaction execution
                        vm_Visitor = new EntityReference(vm_Visitor.EntityLogicalName, visitorId)
                    };

                    // Create a request for each visitor member creation
                    var createVisitorMember = new CreateRequest { Target = visitorMember };
                    transactionRequests.Add(createVisitorMember);
                }

                // Execute the transaction
                var response = _visitRequestRepository.ExecuteTransaction(transactionRequests);

                if (response != null)
                {
                    return new OperationResult
                    {
                        Id = visitRequest.Id,
                        Message = "Visit request & visitor members created successfully!",
                        Status = true
                    };
                }

                return new OperationResult
                {
                    Message = "Failed to process transaction.",
                    Status = false
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Message = "An error occurred: " + ex.Message,
                    Status = false
                };
            }
        }

        public List<OrganizationUsersVM> GetOrganizationUsers()
        {
            Guid currentUserID = ClaimsManager.GetUserId();

            var result = _visitRequestRepository.GetOrganizationUsers(currentUserID);

            // mapping to view model
            return result.Select(e => new OrganizationUsersVM
            {
                UserID = e.Id,
                UserName = e.vm_name,
            }).ToList();
        }
        private List<ViewModels.VisitRequest.VisitorVM> GetVisitorsDetails(Guid visitRequestId)
        {
            List<ViewModels.VisitRequest.VisitorVM> visitors = new List<ViewModels.VisitRequest.VisitorVM>();
            var ids = _visitRequestRepository.GetVisitorMembers(visitRequestId);

            foreach (var id in ids)
            {
                var visitor = _visitRequestRepository.GetVisitingMember(id);
                var visitorVM = new ViewModels.VisitRequest.VisitorVM
                {
                    VisitorId = visitor.Id,
                    VisitorName = visitor.vm_FullName ?? "",
                    Email = visitor.vm_Email ?? "",
                    IdNumber = visitor.vm_IDNumber ?? "",
                    Status = visitor.StatusCode.ToString() ?? "",
                    Organization = visitor.vm_Organization,
                };
                visitors.Add(visitorVM);
            }

            return visitors;
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

        public VisitRequestDetailsVM GetVisitRequestDetails(Guid visitRequestId)
        {
            var visitRequest = _visitRequestRepository.Get(visitRequestId);
            var User = _organizationUsersRepository.Get(visitRequest.vm_RequestedBy.Id);

            // mapping to view model
            VisitRequestDetailsVM visitRequestDetails = new VisitRequestDetailsVM();

            visitRequestDetails.VisiteRequestID = visitRequest.Id.ToString();
            visitRequestDetails.visitRequestInfo.Serial = visitRequest.vm_Newcolumn ?? "";
            visitRequestDetails.visitRequestInfo.RequestdBy = visitRequest.vm_RequestedBy?.Name ?? "";
            visitRequestDetails.visitRequestInfo.Organization = " ";
            visitRequestDetails.visitRequestInfo.Purpose = visitRequest.vm_VisitPurpose.ToString() ?? "";
            visitRequestDetails.visitRequestInfo.Date = visitRequest.vm_VisitTime?.ToString("yyyy-MM-dd") ?? "";
            visitRequestDetails.visitRequestInfo.Time = visitRequest.vm_VisitUntil?.ToString("hh:mm tt") ?? "";
            visitRequestDetails.visitRequestInfo.Duration = CalculateDuration(visitRequest.vm_VisitTime, visitRequest.vm_VisitUntil);
            visitRequestDetails.visitRequestInfo.Location = visitRequest.vm_Location.ToString() ?? "";
            visitRequestDetails.visitRequestInfo.Status = visitRequest.StatusCode.ToString() ?? "";
            visitRequestDetails.visitRequestInfo.ApprovedBy = visitRequest.vm_ApprovedRejectedBy?.Name ?? " ";


            // Location Info
            visitRequestDetails.visitRequestInfo.Building = User.vm_Building.Name ?? "Building";
            visitRequestDetails.visitRequestInfo.Zone = User.vm_Zone?.Name ?? " Zone";
            visitRequestDetails.visitRequestInfo.Floor = User.vm_FloorNumber.HasValue ? User.vm_FloorNumber.ToString() : "--";

            visitRequestDetails.visitorVMs = GetVisitorsDetails(visitRequest.Id);
            return visitRequestDetails;
        }

        public async Task<List<VisitRequestVM>> GetVisitRequests()
        {
            var userId = ClaimsManager.GetUserId();
            var orgName = ClaimsManager.GetOrganizationName();
            var organizationName = orgName;

            var visitReqs = await _visitRequestRepository.GetVisitRequests(userId);


            var visitRequests = visitReqs.Entities.Select(e => new VisitRequestVM
            {
                Serial = e.GetAttributeValue<string>("vm_newcolumn"),
                RequestdBy = e.GetAttributeValue<EntityReference>("vm_requestedby")?.Name,
                Organization = organizationName,
                VisiteRequestID = e.Id,
                Purpose = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(e.GetAttributeValue<OptionSetValue>("vm_visitpurpose")?.Value ?? 0),
                Date = e.GetAttributeValue<DateTime?>("vm_visittime")?.ToString("yyyy-MM-dd"),
                Time = e.GetAttributeValue<DateTime?>("vm_visittime")?.ToString("hh:mm tt"),
                Duration = CalculateDuration(e.GetAttributeValue<DateTime?>("vm_visittime"), e.GetAttributeValue<DateTime?>("vm_visituntil")),
                Location = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_vm_Location>(e.GetAttributeValue<OptionSetValue>("vm_location")?.Value ?? 0),
                Status = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_StatusCode>(e.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? 0),
                ApprovedBy = e.GetAttributeValue<EntityReference>("vm_approvedrejectedby")?.Name,
                VisitorsCount = GetVisitorCount(e.GetAttributeValue<Guid>("vm_visitrequestid"))
            }).ToList();

            return await Task.FromResult(visitRequests);
        }

        public int GetVisitorCount(Guid visitRequestId)
        {
            var result = _visitRequestRepository.GetVisitorCount(visitRequestId);
            return result;
        }

        public List<OptionSet> GetVisitRequestPurposes()
        {
            var result = CustomEnumHelpers.EnumToList<vm_VisitPurposes>();
            // cast to OptionSet
            var optionSets = result.Select(e => new OptionSet { Id = e.Value, Name = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(e.Value) }).ToList();
            return (optionSets);
        }

        public async Task<OperationResult> UpdateVisitRequest(AddVisitRequestVM model)
        {
            // implemented with transaction in case we want to update the visit request and visitors at the same time
            try
            {
                // Retrieve existing visit request
                var existingVisitRequest = _visitRequestRepository.Get(model.VisiteRequestID);
                if (existingVisitRequest == null)
                {
                    return new OperationResult
                    {
                        Message = "Visit request not found.",
                        Status = false
                    };
                }

                // Validate new VisitTime
                if (model.VisitTime >= existingVisitRequest.vm_VisitUntil)
                {
                    return new OperationResult
                    {
                        Message = "Visit Time must be earlier than the existing Visit Until time.",
                        Status = false
                    };
                }

                // Update visit request entity with the new data
                existingVisitRequest.vm_Subject = string.IsNullOrEmpty(model.Subject) ? existingVisitRequest.vm_Subject : model.Subject;
                existingVisitRequest.vm_VisitPurpose = (vm_VisitPurposes)model.Purpose != null ? (vm_VisitPurposes)model.Purpose : existingVisitRequest.vm_VisitPurpose;
                existingVisitRequest.vm_VisitTime = model.VisitTime != null ? model.VisitTime : existingVisitRequest.vm_VisitTime;
                existingVisitRequest.vm_VisitUntil = existingVisitRequest.vm_VisitUntil;
                existingVisitRequest.vm_RequestedBy = model.RequestedBy != null ? new EntityReference(vm_organizationuser.EntityLogicalName, model.RequestedBy) : new EntityReference(vm_organizationuser.EntityLogicalName, existingVisitRequest.vm_RequestedBy.Id);

                existingVisitRequest.vm_Location = (vm_VisitRequest_vm_Location)Enum.Parse(typeof(vm_VisitRequest_vm_Location), model.Location);

                if (model.MeetingArea != Guid.Empty)
                {
                    existingVisitRequest.vm_MeetingArea = new EntityReference(vm_MeetingArea.EntityLogicalName, model.MeetingArea);
                }
                // Create an update request for the visit request
                var updateVisitRequest = new UpdateRequest { Target = existingVisitRequest };

                // Execute the update
                var response = _visitRequestRepository.ExecuteTransaction(new OrganizationRequestCollection { updateVisitRequest });

                if (response != null)
                {
                    return new OperationResult
                    {
                        Id = model.VisiteRequestID,
                        Message = "Visit request updated successfully!",
                        Status = true
                    };
                }

                return new OperationResult
                {
                    Message = "Failed to update visit request.",
                    Status = false
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Message = "An error occurred: " + ex.Message,
                    Status = false
                };
            }
        }

        public async Task<List<VisitingMemberWithRelatedRequestVM>> GetVisitorRequestsHistory(Guid visitorId)
        {
            string fetchRelatedVisitRequests = $@"
                        <fetch distinct='false' useraworderby='false' no-lock='false' mapping='logical'>
                          <entity name='vm_visitingmember'>
                            <attribute name='vm_visitor' />
                            <attribute name='vm_visitrequest' />
                            <filter>
                              <condition attribute='vm_visitor' operator='eq' value='{visitorId}' uitype='vm_visitor' />
                            </filter>
                            <link-entity name='vm_visitrequest' from='vm_visitrequestid' to='vm_visitrequest' link-type='inner' alias='requests'>
                              <attribute name='vm_location' />
                              <attribute name='vm_visitrequestid' />
                              <attribute name='vm_approvedrejectedby' />
                              <attribute name='statecode' />
                              <attribute name='statuscode' />
                              <attribute name='vm_newcolumn' />
                              <attribute name='vm_requestedby' />
                              <attribute name='vm_visitpurpose' />
                              <attribute name='vm_visittime' />
                              <attribute name='vm_visituntil' />
                            </link-entity>
                          </entity>
                        </fetch>
                        ";

            var visitingMembersWithRelatedRequestes = _visitRequestRepository.GetAll(fetchRelatedVisitRequests);
            if (visitingMembersWithRelatedRequestes is null || visitingMembersWithRelatedRequestes.Count == 0)
                return new List<VisitingMemberWithRelatedRequestVM>();

            var visitingMemeberWithRelatedRequestVMS = new List<VisitingMemberWithRelatedRequestVM>();

            foreach (var req in visitingMembersWithRelatedRequestes)
            {
                var visitingMemberWithRelatedRequestVM = VisitingMemberWithRelatedRequestVM.MapFromEntity(req.ToEntity<vm_visitingmember>());
                visitingMemberWithRelatedRequestVM.VisitorsCount = GetVisitorCount(visitingMemberWithRelatedRequestVM.RequestId);
                visitingMemeberWithRelatedRequestVMS.Add(visitingMemberWithRelatedRequestVM);
            }

            return await Task.FromResult(visitingMemeberWithRelatedRequestVMS);
        }

        public ViewModels.VisitorsHub.VisitorVM GetVisitor(Guid visitRequestId)
        {
            var visitor = _visitorsHubRepository.Get(visitRequestId);

            if (visitor == null)
                return null;

            return new ViewModels.VisitorsHub.VisitorVM
            {
                Id = visitor.Id,
                FullName = visitor.Contains("vm_visitorfullname") ? visitor["vm_visitorfullname"].ToString() : "No Name",
                FirstName = visitor.Contains("vm_fullname") ? visitor["vm_fullname"].ToString() : "",
                MiddleName = visitor.Contains("vm_middlename") ? visitor["vm_middlename"].ToString() : "",
                LastName = visitor.Contains("vm_lastname") ? visitor["vm_lastname"].ToString() : "",
                JobTitle = visitor.Contains("vm_jobtitle") ? visitor["vm_jobtitle"].ToString() : "",
                IdNumber = visitor.Contains("vm_idnumber") ? visitor["vm_idnumber"].ToString() : "",
                OrganizationName = visitor.Contains("vm_organization") ? visitor["vm_organization"].ToString() : "No org",
                EmailAddress = visitor.Contains("vm_email") ? visitor["vm_email"].ToString() : "",
                PhoneNumber = visitor.Contains("vm_mobilenumber") ? visitor["vm_mobilenumber"].ToString() : ""
            };
        }

        public OperationResult UpdateVisitor(ViewModels.VisitorsHub.VisitorVM model)
        {
            try
            {
                vm_Visitor visitor = new vm_Visitor
                {
                    Id = model.Id,
                    vm_FullName = model.FirstName,
                    vm_MiddleName = model.MiddleName,
                    vm_LastName = model.LastName,
                    vm_IDNumber = model.IdNumber,
                    vm_JobTitle = model.JobTitle,
                    vm_Organization = model.OrganizationName,
                    vm_Email = model.EmailAddress,
                    vm_MobileNumber = model.PhoneNumber
                };
                bool isUpdated = _visitorsHubRepository.Update(visitor);

                if (isUpdated)
                    return new OperationResult { Status = true, Message = "Visitor updated successfully" };

                return new OperationResult { Status = false, Message = "Failed to update status" };
            }
            catch (Exception)
            {
                return new OperationResult { Status = false, Message = "Something went wrong" };
            }
        }
    }
}
