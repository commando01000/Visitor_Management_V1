using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.VisitRequest
{
    public class AddVisitRequestVM
    {
        public string Subject { get; set; }

        public int Purpose { get; set; }
        public Guid VisiteRequestID { get; set; }
        public DateTime VisitTime { get; set; }

        public DateTime VisitUntil { get; set; }

        public string Location { get; set; }
        public Guid MeetingArea { get; set; }

        public Guid RequestedBy { get; set; }
        public Guid[] VisitorsIds { get; set; }
        public int StatusReason { get; set; }
    }
}