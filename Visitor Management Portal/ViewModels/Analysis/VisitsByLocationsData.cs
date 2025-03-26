using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
    public class VisitsByLocationsData
    {
        public List<VisitsByZoneData> visitsByZones { get; set; } = new List<VisitsByZoneData>();
        public List<BusiestAreasData> BusiestAreas { get; set; } = new List<BusiestAreasData>();
    }
}