using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using System;
using XDesk.Helpers;

namespace Visitor_Management_Portal.ViewModels.VisitRequest
{
    public class VisitRequestVM
    {
        public string Serial { get; set; }
        public string RequestdBy { get; set; }
        public Guid? RequestedBy { get; set; }

        public string Organization { get; set; } = string.Empty;
        public int VisitorsCount { get; set; }
        public string Purpose { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }

        public Guid VisiteRequestID { get; set; }

        public string Duration { get; set; }

        public string Location { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string MeetingArea { get; set; }

        public string Zone { get; set; }

        public string Status { get; set; }
        public vm_VisitRequest_StatusCode StatusCode { get; set; }

        public string ApprovedBy { get; set; }

        public static VisitRequestVM MapFromEntity(vm_VisitRequest e)
        {
            return new VisitRequestVM()
            {
                Serial = e.GetAttributeValue<string>("vm_newcolumn"),
                RequestdBy = e.GetAttributeValue<EntityReference>("vm_requestedby")?.Name,
                //Organization = organizationName,
                VisiteRequestID = e.Id,
                Purpose = CustomEnumHelpers.GetEnumNameByValue<vm_VisitPurposes>(e.GetAttributeValue<OptionSetValue>("vm_visitpurpose")?.Value ?? 0),
                //Date = e.GetAttributeValue<DateTime?>("vm_visittime")?.ToString("yyyy-MM-dd"),
                Date = e.GetAttributeValue<DateTime?>("vm_visittime"),
                Time = e.GetAttributeValue<DateTime?>("vm_visittime"),
                Duration = CalculateDuration(e.GetAttributeValue<DateTime?>("vm_visittime"), e.GetAttributeValue<DateTime?>("vm_visituntil")),
                Location = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_vm_Location>(e.GetAttributeValue<OptionSetValue>("vm_location")?.Value ?? 0),
                Status = CustomEnumHelpers.GetEnumNameByValue<vm_VisitRequest_StatusCode>(e.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? 0),
                ApprovedBy = e.GetAttributeValue<EntityReference>("vm_approvedrejectedby")?.Name ?? "NA",
                StatusCode = (vm_VisitRequest_StatusCode)e.GetAttributeValue<OptionSetValue>("statuscode")?.Value,
                //Visitors = GetVisitorCount(e.GetAttributeValue<Guid>("vm_visitrequestid"))
            };
        }

        private static string CalculateDuration(DateTime? startTime, DateTime? endTime)
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
    }
}