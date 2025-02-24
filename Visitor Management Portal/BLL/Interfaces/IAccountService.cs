using CrmEarlyBound;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IAccountService
    {
        vm_organizationuser Authenticate(string email);
        // Send OTP Function that takes an email
        bool SendOTP(vm_organizationuser user);
        // Validate OTP
        bool ValidateOTP(string Email, string OTP);

        bool ResetPassword(string Email, string password);

        OrganizationUserVM FindUserByEmail(string email);
        OrganizationUserVM FindUserByEmailAndPassword(string email, string password);
    }
}
