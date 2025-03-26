using CrmEarlyBound;
using D365_Add_ons.Extensions;
using QRCoder;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.VisitingMemberRepository;
using Visitor_Management_Portal.ViewModels.VisitingMember;
using System.Configuration;
using OperationResult = Visitor_Management_Portal.Models.OperationResult;
using System.IO;
using Visitor_Management_Portal.ViewModels.VisitorsHub;
using Visitor_Management_Portal.DAL.Repository.VisitorsHubRepository;
using System.Web.Mvc;
using System.Web.Helpers;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using Visitor_Management_Portal.DAL.Repository.VisitRequestRepository;

namespace Visitor_Management_Portal.BLL.Services
{
    public class VisitingMemberService : IVisitingMemberService
    {
        private readonly IVisitingMemberRepository _visitingMemberRepository;
        private readonly IVisitorsHubRepository _visitorRepository;
        private readonly IVisitorsService _visitorsService;
        private readonly string baseUrl;

        public VisitingMemberService(IVisitingMemberRepository visitingMemberRepository, IVisitorsHubRepository visitorRepository, IVisitorsService visitorsService)
        {
            _visitingMemberRepository = visitingMemberRepository;
            _visitorRepository = visitorRepository;
            _visitorsService = visitorsService;
            baseUrl = ConfigurationManager.AppSettings["BaseURL"];
        }

        public VisitingMemberDataVM GetVisitingMemberByShortCode(string visitingMemberShortCode)
        {
            try
            {
                string fetchVisitingMember = $@"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                                                <entity name=""vm_visitingmember"">
                                                <attribute name=""vm_visitor"" />
                                                <attribute name=""vm_visitrequest"" />
                                                <attribute name=""vm_visitingmemberid"" />
                                                <attribute name=""vm_shortcode"" />
                                                <attribute name=""statuscode"" />
                                                <filter type=""and"">
                                                    <condition attribute=""vm_shortcode"" operator=""eq"" value=""{visitingMemberShortCode}"" />
                                                </filter>
                                                <link-entity name=""vm_visitor"" from=""vm_visitorid"" to=""vm_visitor"" link-type=""inner"" alias=""visitor"">
                                                    <attribute name=""vm_email"" />
                                                    <attribute name=""vm_fullname"" />
                                                    <attribute name=""vm_idnumber"" />
                                                    <attribute name=""vm_jobtitle"" />
                                                    <attribute name=""vm_lastname"" />
                                                    <attribute name=""vm_mobilenumber"" />
                                                    <attribute name=""vm_code"" />
                                                    <attribute name=""vm_organization"" />
                                                </link-entity>
                                                <link-entity name=""vm_visitrequest"" from=""vm_visitrequestid"" to=""vm_visitrequest"" link-type=""inner"" alias=""visitrequest"">
                                                    <attribute name=""statecode"" />
                                                    <attribute name=""statuscode"" />
                                                    <attribute name=""vm_duration"" />
                                                    <attribute name=""vm_location"" />
                                                    <attribute name=""vm_visitpurpose"" />
                                                    <attribute name=""vm_visittime"" />
                                                    <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""inner"" alias=""user"">
                                                    <attribute name=""vm_floornumber"" />
                                                    <attribute name=""vm_name"" />
                                                    <link-entity name=""vm_building"" from=""vm_buildingid"" to=""vm_building"" link-type=""outer"" alias=""building"">
                                                        <attribute name=""vm_buildingname"" />
                                                    </link-entity>
                                                    <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""zone"">
                                                        <attribute name=""vm_zonename"" />
                                                    </link-entity>
                                                    <link-entity name=""vm_organization"" from=""vm_organizationid"" to=""vm_organization"" link-type=""outer"" alias=""org"">
                                                        <attribute name=""vm_organizationname"" />
                                                    </link-entity>
                                                    </link-entity>
                                                    <link-entity name=""vm_meetingarea"" from=""vm_meetingareaid"" to=""vm_meetingarea"" link-type=""outer"" alias=""meetingarea"">
                                                    <attribute name=""vm_meetingareaname"" />
                                                    <link-entity name=""vm_building"" from=""vm_buildingid"" to=""vm_building"" link-type=""outer"" alias=""meetingareabuilding"">
                                                        <attribute name=""vm_buildingname"" />
                                                    </link-entity>
                                                    <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""meetingareazone"">
                                                        <attribute name=""vm_zonename"" />
                                                    </link-entity>
                                                    </link-entity>
                                                </link-entity>
                                                </entity>
                                            </fetch>";

                var visitingRequestMember = _visitingMemberRepository.Get(fetchVisitingMember);
                if (visitingRequestMember is null) return null;
                var visitingMemberVM = VisitingMemberDataVM.MapFromEntity(visitingRequestMember.ToEntity<vm_visitingmember>());
                return visitingMemberVM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public OperationResult AcceptInvitation(ViewModels.VisitorsHub.VisitorVM visitorVM, Guid visitingMemberId)
        {
            try
            {
                var visitingMember = _visitingMemberRepository.Get(visitingMemberId);

                if (visitingMember == null)
                {
                    return new OperationResult
                    {
                        Message = "Visiting Member not found.",
                        Status = false
                    };
                }
                //var newObj = visitingMember.CloneIdentity();
                var newObj = new vm_visitingmember()
                {
                    Id = visitingMemberId,
                    StatusCode = vm_visitingmember_StatusCode.Confirmed,
                    StateCode = vm_visitingmemberState.Active,
                };


                var isUpdatedMember = _visitingMemberRepository.Update(newObj);
                var result = _visitorsService.UpdateVisitor(visitorVM);
                if (isUpdatedMember && result.Status)
                {
                    return new OperationResult
                    {
                        Id = visitingMemberId,
                        Message = "Invitation Accepted",
                        Status = true
                    };

                }

                return new OperationResult
                {
                    Message = "Something went wrong",
                    Status = false
                };

            }
            catch (Exception ex)
            {
                return new OperationResult { Status = false, Message = "Something went wrong" };
            }
        }

        public OperationResult RejectInvitation(Guid visitingMemberId)
        {
            try
            {
                var visitingMember = _visitingMemberRepository.Get(visitingMemberId);

                if (visitingMember == null)
                {
                    return new OperationResult
                    {
                        Message = "Visiting Member not found.",
                        Status = false
                    };
                }
                var newObj = new vm_visitingmember()
                {
                    Id = visitingMemberId,
                    StatusCode = vm_visitingmember_StatusCode.InvitationRejected,
                    StateCode = vm_visitingmemberState.Inactive,
                };

                var isUpdated = _visitingMemberRepository.Update(newObj);


                if (isUpdated)
                {
                    return new OperationResult
                    {
                        Id = visitingMemberId,
                        Message = "Invitation rejected",
                        Status = true
                    };
                }

                return new OperationResult
                {
                    Message = "Something went wrong",
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

        public VisitorProfileVM VisitorProfileByCode(string code)
        {
            string fetchVisitorProfileData = $@"<fetch>
                                                  <entity name=""vm_visitor"">
                                                    <attribute name=""vm_idnumber"" />
                                                    <attribute name=""vm_jobtitle"" />
                                                    <attribute name=""vm_organization"" />
                                                    <attribute name=""vm_visitorfullname"" />
                                                    <attribute name=""vm_visitscount"" />
                                                    <filter>
                                                      <condition attribute=""vm_code"" operator=""eq"" value=""{code}"" />
                                                    </filter>
                                                    <link-entity name=""vm_visitingmember"" from=""vm_visitor"" to=""vm_visitorid"" link-type=""inner"" alias=""visitingmember"">
                                                      <link-entity name=""vm_visitrequest"" from=""vm_visitrequestid"" to=""vm_visitrequest"" link-type=""inner"" alias=""visitrequest"">
                                                        <attribute name=""vm_duration"" />
                                                        <attribute name=""vm_visitpurpose"" />
                                                        <attribute name=""vm_visittime"" />
                                                        <attribute name=""vm_location"" />
                                                        <filter type=""and"">
                                                            <condition attribute=""vm_visittime"" operator=""today"" />
                                                        </filter>
                                                        <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""inner"" alias=""user"">
                                                          <attribute name=""vm_floornumber"" />
                                                          <attribute name=""vm_name"" />
                                                          <link-entity name=""vm_building"" from=""vm_buildingid"" to=""vm_building"" link-type=""outer"" alias=""building"">
                                                            <attribute name=""vm_buildingname"" />
                                                          </link-entity>
                                                          <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""zone"">
                                                            <attribute name=""vm_zonename"" />
                                                          </link-entity>
                                                        </link-entity>
                                                        <link-entity name=""vm_meetingarea"" from=""vm_meetingareaid"" to=""vm_meetingarea"" link-type=""outer"" alias=""meetingarea"">
                                                          <attribute name=""vm_meetingareaname"" />
                                                          <link-entity name=""vm_building"" from=""vm_buildingid"" to=""vm_building"" link-type=""outer"" alias=""meetingareabuilding"">
                                                            <attribute name=""vm_buildingname"" />
                                                          </link-entity>
                                                          <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""meetingareazone"">
                                                            <attribute name=""vm_zonename"" />
                                                          </link-entity>
                                                        </link-entity>
                                                      </link-entity>
                                                    </link-entity>
                                                  </entity>
                                                </fetch>";
            var visitorProdfile = _visitorRepository.Get(fetchVisitorProfileData);
            if (visitorProdfile == null)
                return null;
            var visitorProfileVM = VisitorProfileVM.MapFromEntity(visitorProdfile.ToEntity<vm_Visitor>());
            return visitorProfileVM;
        }

        public VisitorTokenVM VisitorTokenByCode(string code)
        {
            string fetchVisitorProfileData = $@"<fetch>
                                                  <entity name=""vm_visitortoken"">
                                                    <filter>
                                                      <condition attribute=""vm_shortcode"" operator=""eq"" value=""{code}"" />
                                                    </filter>
                                                    <link-entity name=""vm_visitingmember"" from=""vm_visitingmemberid"" to=""vm_visitingmemeber"" link-type=""inner"" alias=""visitingmember"">
                                                      <link-entity name=""vm_visitrequest"" from=""vm_visitrequestid"" to=""vm_visitrequest"" link-type=""inner"" alias=""visitrequest"">
                                                        <attribute name=""vm_duration"" />
                                                        <attribute name=""vm_visitpurpose"" />
                                                        <attribute name=""vm_visittime"" />
                                                        <attribute name=""vm_location"" />
                                                        <filter type=""and"">
                                                            <condition attribute=""vm_visittime"" operator=""today"" />
                                                        </filter>
                                                        <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""inner"" alias=""user"">
                                                          <attribute name=""vm_floornumber"" />
                                                          <attribute name=""vm_name"" />
                                                          <link-entity name=""vm_building"" from=""vm_buildingid"" to=""vm_building"" link-type=""outer"" alias=""building"">
                                                            <attribute name=""vm_buildingname"" />
                                                          </link-entity>
                                                          <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""zone"">
                                                            <attribute name=""vm_zonename"" />
                                                          </link-entity>
                                                        </link-entity>
                                                        <link-entity name=""vm_meetingarea"" from=""vm_meetingareaid"" to=""vm_meetingarea"" link-type=""outer"" alias=""meetingarea"">
                                                          <attribute name=""vm_meetingareaname"" />
                                                          <link-entity name=""vm_building"" from=""vm_buildingid"" to=""vm_building"" link-type=""outer"" alias=""meetingareabuilding"">
                                                            <attribute name=""vm_buildingname"" />
                                                          </link-entity>
                                                          <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""meetingareazone"">
                                                            <attribute name=""vm_zonename"" />
                                                          </link-entity>
                                                        </link-entity>
                                                      </link-entity>
                                                      <link-entity name=""vm_visitor"" from=""vm_visitorid"" to=""vm_visitor"" link-type=""inner"" alias=""visitor"">
                                                        <attribute name=""vm_idnumber"" />
                                                        <attribute name=""vm_jobtitle"" />
                                                        <attribute name=""vm_organization"" />
                                                        <attribute name=""vm_visitorfullname"" />
                                                        <attribute name=""vm_visitscount"" />
                                                      </link-entity>
                                                    </link-entity>
                                                  </entity>
                                                </fetch>";
            var visitorToken = _visitorRepository.Get(fetchVisitorProfileData);
            if (visitorToken == null)
                return null;
            var visitorTokenVM = VisitorTokenVM.MapFromEntity(visitorToken.ToEntity<vm_visitortoken>());
            return visitorTokenVM;
        }

        public JsonResult GenerateQrCode(string visitorShortCode, Guid visitingMemberId)
        {
            string visitorUrl = $"{baseUrl}/Visitor/Profile/{visitorShortCode}";
            string base64Image = GenerateQrCodeBase64(visitorUrl);

            JsonResult jsonResponse = new JsonResult
            {
                Data = new
                {
                    QrCode = $"data:image/png;base64,{base64Image}",
                    Message = "QrCode generated successfully",
                    Success = true
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return jsonResponse;
        }

        private string GenerateQrCodeBase64(string url)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            using (MemoryStream stream = new MemoryStream())
            {
                qrBitmap.Save(stream, ImageFormat.Png);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public OperationResult RemoveVisitRequestVisitors(Guid VisitorId, Guid VisitRequestId)
        {
            try
            {
                // Create a transaction request collection for batch deletion
                var transactionRequests = new OrganizationRequestCollection();

                // Fetch all visiting member records that match the criteria
                var visitingMembers = _visitingMemberRepository.GetAll(vm =>
                    (vm.vm_VisitRequest != null && vm.vm_VisitRequest.Id == VisitRequestId) ||
                    (vm.vm_Visitor != null && vm.vm_Visitor.Id == VisitorId) ||
                    vm.vm_VisitRequest == null || // Records with null VisitRequest
                    vm.vm_Visitor == null // Records with null Visitor
                ).ToList();

                if (!visitingMembers.Any())
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "No matching visiting member records found to delete."
                    };
                }

                // Create a DeleteRequest for each matching record
                foreach (var visitingMember in visitingMembers)
                {
                    Console.WriteLine($"Adding DeleteRequest for visiting member ID: {visitingMember.Id}");
                    var deleteRequest = new DeleteRequest
                    {
                        Target = new EntityReference(vm_visitingmember.EntityLogicalName, visitingMember.Id)
                    };
                    transactionRequests.Add(deleteRequest);
                }

                Console.WriteLine($"Executing transaction with {transactionRequests.Count} requests.");

                // Execute the transaction to delete all matching records
                try
                {
                    var response = _visitingMemberRepository.ExecuteTransaction(transactionRequests);

                    if (response != null)
                    {
                        return new OperationResult
                        {
                            Status = true,
                            Message = "Visitor(s) removed from the visit request successfully."
                        };
                    }
                    else
                    {
                        return new OperationResult
                        {
                            Status = false,
                            Message = "Failed to execute deletion transaction."
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = $"Error removing visitor from visit request: {ex.Message}"
                    };
                }

            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = $"Error removing visitor from visit request: {ex.Message}"
                };
            }
        }
    }
}