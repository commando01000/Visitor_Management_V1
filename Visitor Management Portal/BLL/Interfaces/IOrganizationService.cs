using D365_Add_ons.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.ViewModels.OrganizationDate;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IOrganizationService
    {
        bool UpdateOrganizationData(OrganizationDataVM organizationDataVM);
    }
}
