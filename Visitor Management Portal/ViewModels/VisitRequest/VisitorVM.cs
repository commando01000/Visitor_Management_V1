using System;

namespace Visitor_Management_Portal.ViewModels.VisitRequest
{
    public class VisitorVM
    {
        public Guid VisitorId { get; set; }
        public string VisitorName { get; set; }

        public string Email{ get; set; }
        public string Phone { get; set; }
        public string JobTitle { get; set; }
        public string IdNumber { get; set; }

        public string Status { get; set; }

        public string Organization { get; set; }

    }
}