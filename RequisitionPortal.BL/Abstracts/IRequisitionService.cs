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

        Requisition GetRequisition(int requisitionId, string username);

        Requisition GetOutstandingRequisition(int requisitionId);

        IList<Requisition> GetRequisitions(int statusId, string username);

        IList<Req_Item> GetRequisitionItems(long requisitionID);

    }
}
