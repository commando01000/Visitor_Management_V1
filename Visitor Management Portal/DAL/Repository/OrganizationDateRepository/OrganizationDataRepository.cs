﻿using CrmEarlyBound;
using D365_Add_ons.Connection;
using D365_Add_ons.Repository;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.ViewModels.OrganizationDate;
using IOrganizationService = Microsoft.Xrm.Sdk.IOrganizationService;

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

        public Guid GetOrganizationIDByUserID(Guid UserID)
        {
            var result = _context.vm_OrganizationSet.Where(u => u.vm_OrganizationAdmin.Id == UserID).FirstOrDefault().Id;
            return result;
        }
    }
}