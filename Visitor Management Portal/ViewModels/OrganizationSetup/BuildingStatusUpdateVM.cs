using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class BuildingStatusUpdateVM
    {
        public Guid Id { get; set; }
        public bool IsExcludeFromOfficeSelection { get; set; }
    }
}