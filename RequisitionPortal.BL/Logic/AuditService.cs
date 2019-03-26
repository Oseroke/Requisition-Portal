using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Logic
{
    public class AuditService: IAuditService
    {
        private readonly IRepository<AuditTrail, long> _auditRep;
        private readonly IRepository<Req_Item, int> _reqItemRep;

        public AuditService(IRepository<AuditTrail, long> auditRep, IRepository<Req_Item, int> reqItemRep)
        {
            _auditRep = auditRep;
            _reqItemRep = reqItemRep;
        }

        public void LogRequisitionActivity(Requisition requisition, SystemEnums.AuditAction auditAction)
        {
            AuditTrail trail = new AuditTrail()
            {
                AuditActionId = (int)auditAction,
                IsDeleted = false,
                TimeStamp = DateTime.Now,
                UserIp = ":1",
                //Details = requisition.Requestor + "made a requisition on " + requisition.StatusDate 
            };

            if (auditAction == SystemEnums.AuditAction.MadeRequisition)
            {
                trail.Details = requisition.RequestorID + " made a requisition " + requisition.Id;
            }
            else if(auditAction == SystemEnums.AuditAction.CancelledRequisition)
            {
                trail.Details = requisition.RequestorID + " cancelled a requisition " + requisition.Id;
            }
            else if (auditAction == SystemEnums.AuditAction.ApprovedRequisition)
            {
                trail.Details = requisition.ManagerID + " approved a requisition " + +requisition.Id;
            }
            else if (auditAction == SystemEnums.AuditAction.RejectedRequisition)
            {
                trail.Details = requisition.ManagerID + " rejected a requisition " + +requisition.Id + " made by " + requisition.RequestorID;
            }
            else if (auditAction == SystemEnums.AuditAction.GivenOutItems)
            {
                trail.Details = "Store officer gave out items requested by " + requisition.RequestorID + " on Requisition " + requisition.Id ;
            }
            else if(auditAction==SystemEnums.AuditAction.AcknowledgedReceipt)
            {
                trail.Details = requisition.RequestorID + " acknowledged receipt of items collected from the store. Requisition" + requisition.Id;
            }
            else if (auditAction == SystemEnums.AuditAction.Completed)
            {
                trail.Details = "Requisition " + requisition.Id + " completed";
            }
            else if (auditAction == SystemEnums.AuditAction.StoreCancelledRequisition)
            {
                trail.Details = "Requisition " + requisition.Id + " declined by the store.";
            }
            else
            {
                trail.Details = "An action occured for requisition " + requisition.Id;
            }

            _auditRep.SaveOrUpdate(trail);
        }

        public IList<AuditTrail> GetAuditTrails(int actionId)
        {
            var query = _auditRep.Table;

            if (actionId > 0)
                query = query.Where(x => x.AuditActionId == actionId);

            return query.ToList();
        }
        
    }
}
