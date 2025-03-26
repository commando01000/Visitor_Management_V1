using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D365_Add_ons.Repository;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.VisitorsHub;

namespace Visitor_Management_Portal.DAL.Repository.VisitorMemberHubRepository
{
    public class VisitorMemberHubRepository : BaseRepository<vm_visitingmember>, IVisitorMemberHubRepository
    {
        public VisitorMemberHubRepository() : base() { }

    }
}
