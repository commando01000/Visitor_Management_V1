using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IOrganizationUserService
    {
        bool UpdateApprovalStatus(OrganizationUserDetailsVM organizationUserDetailsVM);

        OperationResult CreateUser(OrganizationUserDetailsVM organizationUserDetailsVM);
    }
}
