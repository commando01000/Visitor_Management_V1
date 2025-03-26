using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
	public class VisitorAnalysis
	{
        public Guid VisitorID { get; set; }
        public vm_VisitorState StateCode { get; set; }
    }
}