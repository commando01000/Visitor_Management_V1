using CrmEarlyBound;
using D365_Add_ons.Repository;
using System;
using Visitor_Management_Portal.ViewModels.OrganizationDate;

namespace Visitor_Management_Portal.DAL.Repository.OrganizationDate
{
    public interface IOrganizationDataRepository : IBaseRepository<vm_Organization>
    {
        OrganizationDataVM GetOrganizationDate(Guid UserId);
        bool UpdateOrganizationDate(OrganizationDataVM organizationDataVM);
    }
}
