using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitor_Management_Portal.ViewModels.Profile;

namespace Visitor_Management_Portal.DAL.Repository.ProfileRepository
{
    public interface IProfileRepository
    {
        ProfileInfoVM GetProfileInfo(Guid userId);
        bool UpdateProfileInfo(ProfileInfoVM profileInfoVM, Guid userId);

        bool ChangePassword(ChangePasswordVM changePasswordVM, Guid userId);
    }
}
