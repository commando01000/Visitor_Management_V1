using System;
using System.Collections.Generic;


namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class BuildingWithZonesAndMeetingAreasBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string LocationLink { get; set; }
        public Guid ContactPersonId { get; set; }
        public bool IsExcludeFromOfficeSelection { get; set; }
        public List<Zone> Zones { get; set; }
        public List<MeetingArea> MeetingAreas { get; set; }
    }

    public class Zone
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsExcludeFromOfficeSelection { get; set; }
    }

    public class MeetingArea
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsAvailableForVisitRequests { get; set; }
        public string ZoneName { get; set; }
        public Guid ZoneId { get; set; }
    }

    public class UpdateBuildingDetails
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string LocationLink { get; set; }
        public Guid? ContactPersonId { get; set; }
    }
}