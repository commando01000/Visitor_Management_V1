using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Visitor_Management_Portal.ViewModels.VisitorsHub
{
    public class VisitorsHubVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IDNumber { get; set; }
        public string OrganizationName { get; set; }
    }
}