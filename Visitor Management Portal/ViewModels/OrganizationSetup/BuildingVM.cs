using System;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class BuildingVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string NoOfZones { get; set; }
        public string NoOfMeetingAreas { get; set; }
        public string Location { get; set; }
        public string OrganizationName { get; set; }
        public bool IsExcludeFromOfficeSelection { get; set; }
    }
}