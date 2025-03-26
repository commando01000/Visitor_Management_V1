using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
	public class TotalVisitsChart
	{
		public string Name { get; set; }
		public int TotalVisits { get; set; }
		public bool IsItLargestVisits { get; set; } = false;
	}
}