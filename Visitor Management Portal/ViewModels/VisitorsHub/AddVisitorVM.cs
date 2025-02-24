using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor_Management_Portal.ViewModels.VisitorsHub
{
    public class AddVisitorVM
    {
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string IDNumber { get; set; }
        public string OrganizationName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
