using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.Utilities
{
    public class Utilities
    {
        internal static DateTime ConvertToEgyptTimeZone(DateTime datetime)
        {
            return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById(TimeZones.EgyptTimezoneWithDST));
        }
    }
}