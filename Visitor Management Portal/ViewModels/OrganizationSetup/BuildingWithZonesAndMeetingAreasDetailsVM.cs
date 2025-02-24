using System;


namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class BuildingWithZonesAndMeetingAreasDetailsVM : BuildingWithZonesAndMeetingAreasBase
    {
        public Guid Id { get; set; }
        public string ContactPersonName { get; set; }
        public string NoOfZones { get; set; }
        public string NoOfMeetingAreas { get; set; }

    }
}
