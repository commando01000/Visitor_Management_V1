using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.OrganizationDate;
using Visitor_Management_Portal.ViewModels.OrganizationDate;

namespace Visitor_Management_Portal.BLL.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationDataRepository _organizationDataRepository;

        public OrganizationService(IOrganizationDataRepository organizationDataRepository)
        {
            _organizationDataRepository = organizationDataRepository;
        }
        public bool UpdateOrganizationData(OrganizationDataVM organizationDataVM)
        {
            if (organizationDataVM == null)
            {
                throw new ArgumentNullException(nameof(organizationDataVM), "The organization data cannot be null.");
            }

            var organization = _organizationDataRepository.Get(organizationDataVM.OrganizationId);

            // check if organizationDataVM.OrganizationDomain is not occupied by another organization

            var ExistedOrgDomain = _organizationDataRepository.Get(o => o.vm_DomainName == organizationDataVM.OrganizationDomain && o.Id != organizationDataVM.OrganizationId); // returns null if not found


            if (ExistedOrgDomain == null) // which means the domain name is not occupied by another organization
            {
                if (organization == null)
                {
                    throw new KeyNotFoundException("The specified organization was not found.");
                }

                organization.vm_OrganizationName = organizationDataVM.OrganizationName;
                organization.vm_DomainName = organizationDataVM.OrganizationDomain;
                organization.vm_EmailAddress = organizationDataVM.EmailAddress;
                organization.vm_WebsiteURL = organizationDataVM.OrganizationWebsite;
                organization.vm_PhoneNumber = organizationDataVM.OrganizationPhone;
                organization.vm_OrganizationURL = organizationDataVM.OrganizationURL;
                organization.vm_ClientID = organizationDataVM.ClientID;
                organization.vm_ClientSecret = organizationDataVM.ClientSecret;
                organization.vm_EnableDataverseConnection = organizationDataVM.EnableDataverseConnection;
                organization.vm_WebAPIBaseURL = organizationDataVM.WebApi;
                organization.vm_TenantID = organizationDataVM.TenantID;

                try
                {
                    return _organizationDataRepository.Update(organization);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
