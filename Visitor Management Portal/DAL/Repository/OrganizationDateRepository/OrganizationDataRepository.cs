using CrmEarlyBound;
using D365_Add_ons.Connection;
using D365_Add_ons.Repository;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Visitor_Management_Portal.ViewModels.OrganizationDate;

namespace Visitor_Management_Portal.DAL.Repository.OrganizationDate
{
    public class OrganizationDataRepository : BaseRepository<vm_Organization>, IOrganizationDataRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public OrganizationDataRepository()
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }

        public OrganizationDataVM GetOrganizationDate(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            try
            {
                var organizationId = GetOrganizationIDByUserID(userId);

                var organization = _context.vm_OrganizationSet.Where(o => o.Id == organizationId).FirstOrDefault();
                if (organization == null)
                {
                    return null;
                }

                return new OrganizationDataVM
                {
                    OrganizationId = organization.Id == Guid.Empty ? Guid.Empty : organization.Id,
                    OrganizationName = organization.vm_OrganizationName ?? " ",
                    OrganizationDomain = organization.vm_DomainName ?? " ",
                    EmailAddress = organization.vm_EmailAddress ?? " ",
                    OrganizationPhone = organization.vm_PhoneNumber ?? " ",
                    OrganizationWebsite = organization.vm_WebsiteURL ?? " ",
                    EnableDataverseConnection = organization.vm_EnableDataverseConnection ?? false,
                    OrganizationURL = organization.vm_OrganizationURL ?? " ",
                    ClientID = organization.vm_ClientID ?? " ",
                    ClientSecret = organization.vm_ClientSecret ?? " ",
                    WebApi = organization.vm_WebAPIBaseURL ?? " ",
                    TenantID = organization.vm_TenantID ?? " "

                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool UpdateOrganizationDate(OrganizationDataVM organizationDataVM)
        {
            if (organizationDataVM == null)
            {
                throw new ArgumentNullException(nameof(organizationDataVM), "The organization data cannot be null.");
            }
            
            var organization = _context.vm_OrganizationSet
                .Where(o => o.Id == organizationDataVM.OrganizationId) 
                .FirstOrDefault();

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
            organization.vm_TenantID=organizationDataVM.TenantID;

            try
            {
                _context.UpdateObject(organization);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Guid GetOrganizationIDByUserID(Guid UserID)
        {
            var result = _context.vm_OrganizationSet.Where(u => u.vm_OrganizationAdmin.Id == UserID).FirstOrDefault().Id;
            return result;
        }
    }
}