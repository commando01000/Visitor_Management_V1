using CrmEarlyBound;
using D365_Add_ons.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository
{
    public interface IOrganizationUsersRepository : IBaseRepository<vm_organizationuser>
    {
        List<OrganizationUserVM> GetByOrganizationId(Guid organizationId);
        List<OrganizationUserVM> GetOrganizationUsers(Guid userid);
        OrganizationUserDetailsVM GetOrganizationUserDetails(Guid UserID);
        OrganizationUserDetailsVM GetOrganizationUserInfo(Guid UserID);
        bool UpdateOrganizationUserInfo(OrganizationUserDetailsVM organizationUserDetailsVM);

        bool DeleteOrganizationUser(Guid UserID);

    }
}
