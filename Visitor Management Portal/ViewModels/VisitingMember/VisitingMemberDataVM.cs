using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using XDesk.Helpers;

namespace Visitor_Management_Portal.ViewModels.VisitingMember
{
    public class VisitingMemberDataVM
    {
        public Guid VisitingMemberId { get; set; }
        public Guid VisitRequestId { get; set; }

        public Guid VisitorId { get; set; }
        public string VisitorFullName { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public string JobTitle { get; set; }
        public string MobileNumber { get; set; }
        public string ShortCode { get; set; }

        public string RequestedBy { get; set; }
        public string VisitPurpose { get; set; }
        public DateTime VisitTime { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
        public int StateCode { get; set; }
        public int StatusCode { get; set; }
        public int VisitingMemebrerStatusCode { get; set; }
        public string OrganizationName { get; set; }

        public string FloorNumber { get; set; }
        public string BuildingName { get; set; }
        public string ZoneName { get; set; }
        public string MeetingArea { get; set; }
        public string QRCodeUrl { get; set; } = null;

        public static VisitingMemberDataVM MapFromEntity(Entity e)
        {
            var visitor = e.GetAttributeValue<EntityReference>("vm_visitor");
            var visitRequest = e.GetAttributeValue<EntityReference>("vm_visitrequest");
            var requestedBy = e.GetAliasedValue<EntityReference>("visitrequest.vm_requestedby");
            var location = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_vm_Location>(e.GetAliasedValue<OptionSetValue>("visitrequest.vm_location")?.Value ?? 0);

            var visitingMemberVM = new VisitingMemberDataVM()
            {
                VisitingMemberId = e.Id,
                VisitRequestId = visitRequest?.Id ?? Guid.Empty,
                VisitorId = visitor?.Id ?? Guid.Empty,

                VisitorFullName = e.GetAliasedValue<string>("visitor.vm_fullname"),
                ShortCode = e.GetAliasedValue<string>("visitor.vm_code"),
                Email = e.GetAliasedValue<string>("visitor.vm_email"),
                IdNumber = e.GetAliasedValue<string>("visitor.vm_idnumber"),
                JobTitle = e.GetAliasedValue<string>("visitor.vm_jobtitle"),
                MobileNumber = e.GetAliasedValue<string>("visitor.vm_mobilenumber"),

                VisitPurpose = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(e.GetAliasedValue<OptionSetValue>("visitrequest.vm_visitpurpose")?.Value ?? 0),
                VisitTime = e.GetAliasedValue<DateTime>("visitrequest.vm_visittime"),
                Duration = e.GetAliasedValue<int>("visitrequest.vm_duration"),
                Location = location,
                StateCode = e.GetAliasedValue<OptionSetValue>("visitrequest.statecode")?.Value ?? 0,
                StatusCode = e.GetAliasedValue<OptionSetValue>("visitrequest.statuscode")?.Value ?? 0,
                VisitingMemebrerStatusCode = e.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? 0,
                RequestedBy = e.GetAliasedValue<string>("user.vm_name"),
                OrganizationName = e.GetAliasedValue<string>("visitor.vm_organization")
            };

            if (location == "Office")
            {
                visitingMemberVM.FloorNumber = e.GetAliasedValue<int?>("user.vm_floornumber")?.ToString() ?? e.GetAliasedValue<string>("user.vm_floornumber");
                visitingMemberVM.BuildingName = e.GetAliasedValue<string>("building.vm_buildingname");
                visitingMemberVM.ZoneName = e.GetAliasedValue<string>("zone.vm_zonename");
            }
            else
            {
                visitingMemberVM.BuildingName = e.GetAliasedValue<string>("meetingareabuilding.vm_buildingname");
                visitingMemberVM.ZoneName = e.GetAliasedValue<string>("meetingareazone.vm_zonename");
                visitingMemberVM.MeetingArea = e.GetAliasedValue<string>("meetingarea.vm_meetingareaname");
            }

            return visitingMemberVM;
        }
    }
}
