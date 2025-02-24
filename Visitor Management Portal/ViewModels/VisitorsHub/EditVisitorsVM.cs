using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.VisitorsHub
{
    public class VisitorVM
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public string JobTitle { get; set; }
        public string OrganizationName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}