using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Visitor_Management_Portal.ViewModels.VisitingMember;
using XDesk.Helpers;

namespace Visitor_Management_Portal.ViewModels.VisitorsHub
{
    public class VisitorProfileVM
    {
        public string IdNumber { get; set; }
        public string JobTitle { get; set; }
        public string Organization { get; set; }
        public string VisitorFullName { get; set; }
        public string OrganizationName { get; set; }
        public string ShortCode { get; set; }
        public DateTime VisitTime { get; set; }
        public int Duration { get; set; }
        public string VisitPurpose { get; set; }
        public string FloorNumber { get; set; }
        public string RequestedBy { get; set; }
        public string BuildingName { get; set; }
        public string ZoneName { get; set; }
        public string MeetingArea { get; set; }
        public string Location { get; set; }
        public int VisitsCount { get; set; }

        public static VisitorProfileVM MapFromEntity(vm_Visitor visitor)
        {
            var location = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_vm_Location>(visitor.GetAliasedValue<OptionSetValue>("visitrequest.vm_location")?.Value ?? 0);

            var visitorProfile = new VisitorProfileVM()
            {
                IdNumber = visitor.vm_IDNumber,
                JobTitle = visitor.vm_JobTitle,
                OrganizationName = visitor.vm_Organization,
                VisitorFullName = visitor.vm_VisitorFullName,
                ShortCode = visitor.vm_Code,
                VisitTime = visitor.GetAliasedValue<DateTime>("visitrequest.vm_visittime").ToEgyptTime(),
                Duration = visitor.GetAliasedValue<int>("visitrequest.vm_duration"),
                VisitPurpose = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(visitor.GetAliasedValue<OptionSetValue>("visitrequest.vm_visitpurpose")?.Value ?? 0),
                VisitsCount = visitor.vm_VisitsCount ?? 0,
                RequestedBy = visitor.GetAliasedValue<string>("user.vm_name"),
                Location = location
            };

            if (location == "Office")
            {
                visitorProfile.FloorNumber = visitor.GetAliasedValue<int?>("user.vm_floornumber")?.ToString() ?? visitor.GetAliasedValue<string>("user.vm_floornumber");
                visitorProfile.BuildingName = visitor.GetAliasedValue<string>("building.vm_buildingname");
                visitorProfile.ZoneName = visitor.GetAliasedValue<string>("zone.vm_zonename");
            }
            else
            {
                visitorProfile.BuildingName = visitor.GetAliasedValue<string>("meetingareabuilding.vm_buildingname");
                visitorProfile.ZoneName = visitor.GetAliasedValue<string>("meetingareazone.vm_zonename");
                visitorProfile.MeetingArea = visitor.GetAliasedValue<string>("meetingarea.vm_meetingareaname");
            }

            return visitorProfile;
        }
    }
}


