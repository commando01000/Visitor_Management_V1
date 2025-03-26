using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
	public class UserAnalysis
	{
        public Guid UserID { get; set; }
        public vm_organizationuserState StateCode { get; set; }
    }
}