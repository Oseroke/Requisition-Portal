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
    public class RequisitionService: IRequisitionService
    {
        private readonly IRepository<Requisition, long> _reqRep;
        private readonly IRepository<Req_Item, int> _reqItemRep;
        
        public RequisitionService(IRepository<Requisition, long> reqRep, IRepository<Req_Item, int> reqItemRep)
        {
            _reqRep = reqRep;
            _reqItemRep = reqItemRep;
        }

        public Requisition SaveRequisition(Requisition requisition)
        {
            try
            {
                _reqRep.SaveOrUpdate(requisition);

                return requisition;
            }
            catch (Exception ex)
            {
                throw new RequisitionException("The requisition already exists");
            }
        }

        public Requisition GetRequisition(int requisitionId)
        {
            try
            {
                var query = _reqRep.Table.Where(x => x.IsDeleted == false);

                if (requisitionId > 0)
                    query = query.Where(x => x.Id == requisitionId);

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new RequisitionException("The requisition does not exist");
                //return null;
            }
        }

        public Requisition GetOutstandingRequisition(int requisitionId)
        {
            try
            {
                var query = _reqRep.Table.Where(x => x.IsDeleted == false) ;

                if (requisitionId > 0)
                    query = query.Where(x => x.Id == requisitionId);

                query = query.Where(x => x.StatusID == (int)SystemEnums.Status.AwaitingMgrApproval);
               // query = query.Where(x => x.IsDeleted == false);
                //qGetById(requisitionId);

                //query = query.
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new RequisitionException("The requisition does not exist");
                //return null;
            }
        }

        public IList<Requisition> GetRequisitions(int statusId)
        {
            var query = _reqRep.Table.Where(x => x.IsDeleted == false);

            if (statusId > 0)
                query = query.Where(x => x.StatusID == statusId);

            return query.OrderByDescending(x=>x.StatusDate).ToList();

        }

        public IList<Req_Item> GetRequisitionItems(long requisitionID)
        {
            var query = _reqItemRep.Table.Where(x=>x.IsDeleted == false);

            if (requisitionID > 0)
                query = query.Where(x => x.RequisitionID == requisitionID);

            return query.ToList();
        }
    }
}
