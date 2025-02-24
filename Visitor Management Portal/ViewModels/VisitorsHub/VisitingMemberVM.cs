using CrmEarlyBound;
using System;

namespace Visitor_Management_Portal.ViewModels.VisitorsHub
{
    public class VisitingMemberVM
    {
        public Guid RequestId { get; set; }

        public Guid VisitorId { get; set; }

        public static VisitingMemberVM MapFromEntity(vm_visitingmember entity)
        {
            return new VisitingMemberVM()
            {
                RequestId = entity.vm_VisitRequest != null ? entity.vm_VisitRequest.Id : Guid.Empty,
                VisitorId = entity.vm_Visitor != null ? entity.vm_Visitor.Id : Guid.Empty
            };
        }
    }
}