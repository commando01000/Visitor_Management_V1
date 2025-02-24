using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Profile
{
    public class ChangePasswordVM
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}