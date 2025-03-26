using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
    public class BusiestAreasData
    {
        public string Zone_Name { get; set; }
        public string MeetingAreaName { get; set; }
        public int NumberVisitsPerZone { get; set; }
    }
}