


using CrmEarlyBound;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.AccountRepository;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public vm_organizationuser Authenticate(string Email)
        {
            var user = _accountRepository.Get(a => a.vm_EmailAddress == Email);

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public bool SendOTP(vm_organizationuser user)
        {
            if (user != null)
            {
                user.vm_GenerateOTP = true;
                var result = _accountRepository.Update(user);

                // to be used in testing only
                var tempUser = _accountRepository.Get(user.Id);
                var decryptedOTP = DataEncryptionHelper.Decryptdata(tempUser.vm_OTP);

                return result;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateOTP(string Email, string OTP)
        {
            var user = _accountRepository.Get(a => a.vm_EmailAddress == Email && a.vm_OTPExpiration > DateTime.Now);

            if (user != null)
            {
                var EncryptedOTP = DataEncryptionHelper.Encryptdata(OTP);

                if (user.vm_OTP == EncryptedOTP)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResetPassword(string Email, string password)
        {
            var user = _accountRepository.Get(u => u.vm_EmailAddress == Email);

            if (user != null)
            {
                if (user != null)
                {
                    // Update With his new password
                    var EncryptedPassword = DataEncryptionHelper.Encryptdata(password);
                    user.vm_Password = EncryptedPassword;

                    var result = _accountRepository.Update(user);
                    return result;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public OrganizationUserVM FindUserByEmail(string email)
        {
            try
            {
                string fetchUserByEmailQuery = $@"
                        <fetch>
                          <entity name='vm_organizationuser'>
                            <attribute name='vm_emailaddress' />
                            <attribute name='vm_name' />
                            <attribute name='vm_organization' />
                            <attribute name='vm_organizationuserid' />
                            <attribute name='vm_role' />
                            <filter>
                              <condition attribute='vm_emailaddress' operator='eq' value='{email}'/>
                            </filter>
                          </entity>
                        </fetch>";

                var user = _accountRepository.Get(fetchUserByEmailQuery);
                if (user is null) return null;

                var userVM = OrganizationUserVM.MapFromEntity(user.ToEntity<vm_organizationuser>());

                return userVM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public OrganizationUserVM FindUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var user = _accountRepository.Get(u => u.vm_EmailAddress == email && u.vm_Password == DataEncryptionHelper.Encryptdata(password));

                if (user is null) return null;

                var userVM = OrganizationUserVM.MapFromEntity(user.ToEntity<vm_organizationuser>());

                return userVM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}