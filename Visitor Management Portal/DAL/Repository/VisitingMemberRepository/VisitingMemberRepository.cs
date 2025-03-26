using CrmEarlyBound;
using D365_Add_ons.Connection;
using D365_Add_ons.Repository;
using Microsoft.Xrm.Sdk;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;

namespace Visitor_Management_Portal.DAL.Repository.VisitingMemberRepository
{
    public class VisitingMemberRepository : BaseRepository<vm_visitingmember>, IVisitingMemberRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public VisitingMemberRepository(IOrganizationUsersRepository organizationUsersRepository)
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }
    }
}