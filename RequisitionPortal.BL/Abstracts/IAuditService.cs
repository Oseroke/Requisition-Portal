using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public interface IAuditService
    {
        void LogRequisitionActivity(Requisition requisition, SystemEnums.AuditAction auditAction);
        IList<AuditTrail> GetAuditTrails(int actionId);
    }
}
