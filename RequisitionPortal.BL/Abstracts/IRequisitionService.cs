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

        IList<Requisition> GetRequisitionsByRequestor(int statusId, string username);

        Requisition GetRequisitionByRequestor(int requisitionId, string username);

        IList<Req_Item> GetCompletedReqItems(bool isDeleted, string username);

        IList<Req_Item> GetCompletedReqItems(bool isDeleted, string username, DateTime startDate, DateTime endDate);

        IQueryable GetCompletedReqItemsByUnit(bool isDeleted, string username, string servLineCode, DateTime startDate, DateTime endDate);

        IList<Report> GetCompletedReqItemsByUnit2(bool isDeleted, string username, string servLineCode);

        IList<Report> GetCompletedReqItemsByUnit3(bool isDeleted, string username, string servLineCode, DateTime startDate, DateTime endDate);
    }
}
