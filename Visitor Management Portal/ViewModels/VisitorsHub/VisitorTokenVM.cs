using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDesk.Helpers;

namespace Visitor_Management_Portal.ViewModels.VisitorsHub
{
    public class VisitorTokenVM
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

        public static VisitorTokenVM MapFromEntity(vm_visitortoken token)
        {
            var location = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_vm_Location>(token.GetAliasedValue<OptionSetValue>("visitrequest.vm_location")?.Value ?? 0);

            var visitorToken = new VisitorTokenVM()
            {
                IdNumber = token.GetAliasedValue<string>("visitor.vm_idnumber"),
                JobTitle = token.GetAliasedValue<string>("visitor.vm_jobtitle"),
                OrganizationName = token.GetAliasedValue<string>("visitor.vm_organization"),
                VisitorFullName = token.GetAliasedValue<string>("visitor.vm_visitorfullname"),
                ShortCode = token.vm_ShortCode,
                VisitTime = token.GetAliasedValue<DateTime>("visitrequest.vm_visittime"),
                Duration = token.GetAliasedValue<int>("visitrequest.vm_duration"),
                VisitPurpose = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(token.GetAliasedValue<OptionSetValue>("visitrequest.vm_visitpurpose")?.Value ?? 0),
                RequestedBy = token.GetAliasedValue<string>("user.vm_name"),
                VisitsCount = token.GetAliasedValue<int?>("visitor.vm_visitscount") ?? 0,
                Location = location
            };

            if (location == "Office")
            {
                visitorToken.FloorNumber = token.GetAliasedValue<int?>("user.vm_floornumber")?.ToString() ?? token.GetAliasedValue<string>("user.vm_floornumber");
                visitorToken.BuildingName = token.GetAliasedValue<string>("building.vm_buildingname");
                visitorToken.ZoneName = token.GetAliasedValue<string>("zone.vm_zonename");
            }
            else
            {
                visitorToken.BuildingName = token.GetAliasedValue<string>("meetingareabuilding.vm_buildingname");
                visitorToken.ZoneName = token.GetAliasedValue<string>("meetingareazone.vm_zonename");
                visitorToken.MeetingArea = token.GetAliasedValue<string>("meetingarea.vm_meetingareaname");
            }

            return visitorToken;
        }
    }
}