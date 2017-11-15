using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public interface IRequisitionService
    {
        Requisition SaveRequisition(Requisition requisition);

        Requisition GetRequisition(int requisitionId);

        Requisition GetOutstandingRequisition(int requisitionId);

        IList<Requisition> GetOutstandingRequisitions(int statusId);

        IList<Req_Item> GetRequisitionItems(long requisitionID);

    }
}
