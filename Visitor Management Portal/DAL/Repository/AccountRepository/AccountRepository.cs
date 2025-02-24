using CrmEarlyBound;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Visitor_Management_Portal.Utilities;
using D365_Add_ons.Connection;
using Visitor_Management_Portal.ViewModels.Auth;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using D365_Add_ons.Repository;

namespace Visitor_Management_Portal.DAL.Repository.AccountRepository
{
    public class AccountRepository : BaseRepository<vm_organizationuser>, IAccountRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public AccountRepository(IOrganizationUsersRepository organizationUsersRepository)
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }

        public bool Register(string fullName, string email, string password, string organizationName, string organizationDomain)
        {
            try
            {
                CrmServiceContext context = new CrmServiceContext(_service);

                QueryExpression query = new QueryExpression("vm_organization")
                {
                    ColumnSet = new ColumnSet("vm_organizationid"),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("vm_domainname", ConditionOperator.Equal, organizationDomain)
                        }
                    }
                };

                EntityCollection results = _service.RetrieveMultiple(query);

                if (results.Entities.Count > 0)
                {
                    return false;
                }

                Entity newOrganization = new Entity("vm_organization");
                newOrganization["vm_organizationname"] = organizationName;
                newOrganization["vm_domainname"] = organizationDomain;

                Guid organizationId = _service.Create(newOrganization);

                Entity newOrganizationUser = new Entity("vm_organizationuser");
                newOrganizationUser["vm_name"] = fullName;
                newOrganizationUser["vm_emailaddress"] = email;
                newOrganizationUser["vm_password"] = DataEncryptionHelper.Encryptdata(password);
                newOrganizationUser["vm_role"] = new OptionSetValue((int)vm_organizationuser_vm_Role.Administrator);
                newOrganizationUser["vm_organization"] = new EntityReference("vm_organization", organizationId);

                Guid userId = _service.Create(newOrganizationUser);

                Entity updateOrganization = new Entity("vm_organization");
                updateOrganization.Id = organizationId;
                updateOrganization["vm_organizationadmin"] = new EntityReference("vm_organizationuser", userId);

                _service.Update(updateOrganization);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public LoginSuccessInfoVM GetLoginSuccessInfo(Guid userId)
        {
            var user = _context.vm_organizationuserSet.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }
            LoginSuccessInfoVM loginSuccessInfo = new LoginSuccessInfoVM
            {
                Name = user.vm_name,
                Jobtitle = user.vm_JobTitle
            };
            return loginSuccessInfo;
        }
    }
}