using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.VisitRequest
{
    public class CurrentOfficeLocationVM
    {
        public string Building { get; set; }

        public Guid BuildingId { get; set; }
        public string Zone { get; set; }

        public Guid ZoneId { get; set; }

        public string Floor { get; set; }
    }
}