using System.Collections.Generic;

namespace Visitor_Management_Portal.ViewModels.OrganizationSetup
{
    public class ZoneWithRelatedAreasVM
    {
        public ZoneVM Zone { get; set; }

        public List<MeetingAreaVM> RelatedAreas { get; set; }
    }
}