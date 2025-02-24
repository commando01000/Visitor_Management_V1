using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.VisitRequest
{
    public class VisitRequestDetailsVM
    {
        public string VisiteRequestID { get; set; }
        public VisitRequestVM visitRequestInfo { get; set; } = new VisitRequestVM();
        public List<VisitorVM> visitorVMs { get; set; } = new List<VisitorVM>();
    }
}