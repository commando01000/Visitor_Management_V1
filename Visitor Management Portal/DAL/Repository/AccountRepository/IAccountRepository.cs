using CrmEarlyBound;
using D365_Add_ons.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.ViewModels.Auth;

namespace Visitor_Management_Portal.DAL.Repository.AccountRepository
{
    public interface IAccountRepository : IBaseRepository<vm_organizationuser>
    {
        bool Register(string fullName, string email, string password, string organizationName, string organizationDomain);
        LoginSuccessInfoVM GetLoginSuccessInfo(Guid userId);
    }
}