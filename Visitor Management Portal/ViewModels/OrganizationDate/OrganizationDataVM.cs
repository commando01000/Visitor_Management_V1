using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.OrganizationDate
{
    public class OrganizationDataVM
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string EmailAddress{ get; set; }
        public string OrganizationPhone { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationDomain { get; set; }


        // dataverse 
        public bool EnableDataverseConnection{ get; set; }
        public string OrganizationURL  { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string WebApi { get; set; }
        public string TenantID { get; set; }

    }
}