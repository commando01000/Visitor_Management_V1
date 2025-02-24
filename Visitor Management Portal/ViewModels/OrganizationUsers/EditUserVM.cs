using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.OrganizationUsers
{
    public class EditUserVM
    {
        public bool CreateVisitsWithoutApproval { get; set; }


        public Guid Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Reportingto { get; set; }
        public string Building { get; set; }
        public Guid BuildingId { get; set; }
        public string Zone { get; set; }
        public Guid ZoneId { get; set; }
        public string Floor { get; set; }


    }
}