using System;
using System.Linq;
using System.Windows.Markup;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.OrganizationDate;
using Visitor_Management_Portal.Utilities;

namespace Visitor_Management_Portal.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrganizationDataRepository _organizationDataRepository;

        public DashboardService(IOrganizationDataRepository organizationDataRepository)
        {
            _organizationDataRepository = organizationDataRepository;
        }

        public IOrganizationDataRepository OrganizationDataRepository { get; }

        public bool IsOrganizationSetupComplete()
        {
            // Steps :
            // 1/  Check if the org optional data is filled (Email , website , phone)
            // 2/  Check if the org users count is greater than 1
            // 3/ check if there at least one building in the organization

            // IF Any of the checks fails return False else return True

            var organizationId = ClaimsManager.GetOrganizationId();
            var organization = _organizationDataRepository.Get(organizationId);

            if (organization.vm_EmailAddress == null ||
                     organization.vm_WebsiteURL == null ||
                         organization.vm_PhoneNumber == null) return false;


            string fetchUsersCountQuery = $@"
                        <fetch>
                          <entity name='vm_organizationuser'>
                            <attribute name='vm_organizationuserid' />
                            <filter>
                              <condition attribute='vm_organization' operator='eq' value='{organizationId}' uitype='vm_organization' />
                            </filter>
                          </entity>
                        </fetch>
                        ";

            var users = _organizationDataRepository.GetAll(fetchUsersCountQuery);
            if (users == null || users.Count() <= 1) return false;


            string fetchBuildingsCountQuery = $@"
                       <fetch>
                          <entity name='vm_building'>
                            <attribute name='vm_buildingid' />
                            <filter>
                              <condition attribute='vm_organization' operator='eq' value='{organizationId}' uitype='vm_organization' />
                            </filter>
                          </entity>
                        </fetch> ";

            var buildings = _organizationDataRepository.GetAll(fetchBuildingsCountQuery);
            if (buildings == null || buildings.Count() < 1) return false;

            return true;
        }

    }
}
