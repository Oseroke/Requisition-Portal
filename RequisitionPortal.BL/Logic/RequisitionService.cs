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
        private readonly IRepository<ChargeCode, int> _chargeCodeRep;
        private readonly IRepository<Item, int> _itemRep;
        private readonly IRepository<Unit, int> _unitRep;
        private readonly IRepository<Report, long> _rptRep;

        public RequisitionService(IRepository<Requisition, long> reqRep, IRepository<Req_Item, int> reqItemRep,IRepository<ChargeCode, int> chargeCodeRep, IRepository<Item, int> itemRep, IRepository<Unit, int> unitRep, IRepository<Report, long> rptRep)
        {
            _reqRep = reqRep;
            _reqItemRep = reqItemRep;
            _chargeCodeRep = chargeCodeRep;
            _itemRep = itemRep;
            _unitRep = unitRep;
            _rptRep = rptRep;
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

        public Requisition GetRequisition(int requisitionId, string username)
        {
            try
            {
                var query = _reqRep.Table.Where(x => x.IsDeleted == false);

                if (!string.IsNullOrEmpty(username))
                    query = query.Where(x => x.ManagerID == username);

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

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new RequisitionException("The requisition does not exist");
                //return null;
            }
        }

        public IList<Requisition> GetRequisitions(int statusId, string username)
        {
            var query = _reqRep.Table.Where(x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.ManagerID == username);

            if (statusId > 0)
                query = query.Where(x => x.StatusID == statusId);

            return query.OrderByDescending(x=>x.StatusDate).ToList();

        }

        public IList<Req_Item> GetRequisitionItems(long requisitionID)
        {
            try
            {
                var query = _reqItemRep.Table.Where(x => x.IsDeleted == false);

                if (requisitionID > 0)
                    query = query.Where(x => x.RequisitionID == requisitionID);

                return query.ToList();
            }
            catch
            {
                throw new RequisitionException("The request does not exist");
            }
        }

        public IList<Requisition> GetRequisitionsByRequestor(int statusId, string username)
        {
            try
            {
                var query = _reqRep.Table.Where(x => x.IsDeleted == false);

                if (!string.IsNullOrEmpty(username))
                    query = query.Where(x => x.RequestorID == username);

                if (statusId > 0)
                    query = query.Where(x => x.StatusID == statusId);

                return query.OrderByDescending(x => x.StatusDate).ToList();
            }
            catch
            {
                throw new RequisitionException("The request does not exist");

            }
        }

        public Requisition GetRequisitionByRequestor(int requisitionId, string username)
        {
            try
            {
                var query = _reqRep.Table.Where(x => x.IsDeleted == false);

                if (!string.IsNullOrEmpty(username))
                    query = query.Where(x => x.RequestorID == username);

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

        public IList<Req_Item> GetCompletedReqItems(bool isDeleted, string username)
        {
            var query = _reqItemRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Requisition.ManagerID == username);

            query = query.Where(x => x.Requisition.StatusID == (int)SystemEnums.Status.Completed);

            return query.ToList();
        }

        public IList<Req_Item> GetCompletedReqItems(bool isDeleted, string username, DateTime startDate, DateTime endDate)
        {
            var query = _reqItemRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Requisition.ManagerID == username);

            query = query.Where(x => x.Requisition.StatusID == (int)SystemEnums.Status.Completed);

            query = query.Where(x => x.Requisition.StatusDate >= startDate && x.Requisition.StatusDate <= endDate);

            return query.ToList();
        }

        public IQueryable GetCompletedReqItemsByUnit(bool isDeleted, string username, string servLineCode, DateTime startDate, DateTime endDate)
        {            
            var query = _reqItemRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Requisition.ManagerID == username);

            query = query.Where(x => x.Requisition.StatusID == (int)SystemEnums.Status.Completed);

            query = query.Where(x => x.Requisition.StatusDate >= startDate && x.Requisition.StatusDate <= endDate);
            
            var query3 = query.Join(_chargeCodeRep.Table, x => new { x.ChargeCode }, y => new { ChargeCode = y.Code }, (x, y) => new { x, y })
                .Join(_itemRep.Table, a => new { a.x.Item }, b => new { Item = b.Code }, (a, b) => new { a,b })
            .Join(_unitRep.Table, c => new { c.a.y.ServLineCode }, d => new { ServLineCode = d.ServLineCode }, (c, d) => new { c.a.x.Requisition.StatusDate, /*c.a.x.Item,*/ c.b.Name, c.a.x.Quantity, c.b.UnitPrice, Amount = c.a.x.Quantity * c.b.UnitPrice, ChargeCode = c.a.x.ChargeCode, ServLineCode = c.a.y.ServLineCode, Unit = d.Name,  Requestor = c.a.x.Requisition.RequestorID, Manager = c.a.x.Requisition.ManagerID });

            if (!string.IsNullOrEmpty(servLineCode))
                return query3.Where(x=>x.ServLineCode == servLineCode);

            return query3;
            
        }

        public IList<Report> GetCompletedReqItemsByUnit2(bool isDeleted, string username, string servLineCode)
        {
            var query = _rptRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Manager == username);

            query = query.Where(x => x.StatusID == (int)SystemEnums.Status.Completed);


            if (!string.IsNullOrEmpty(servLineCode))
                query = query.Where(x => x.ServLineCode == servLineCode);

            return query.OrderByDescending(x=>x.StatusDate).ToList();

        }

        public IList<Report> GetCompletedReqItemsByUnit3(bool isDeleted, string username, string servLineCode, DateTime startDate, DateTime endDate)
        {
            var query = _rptRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Manager == username);

            query = query.Where(x => x.StatusID == (int)SystemEnums.Status.Completed);

            query = query.Where(x => x.StatusDate >= startDate && x.StatusDate <= endDate);

            if (!string.IsNullOrEmpty(servLineCode))
                query = query.Where(x => x.ServLineCode == servLineCode);

            return query.OrderByDescending(x => x.Id).ToList();
        }


    }
}

//var query1 = query.Join(_reqItemRep.Table, a => a.Id, b => b.Id, (a, b) => new { a, b }).Join(_chargeCodeRep.Table, x => new { x.a.ChargeCode }, y => new { ChargeCode = y.Code }, (x, y) => new { ChargeCode = x.a.ChargeCode, servLineCode = y.ServLineCode });
//var query1 = query.Join(_chargeCodeRep.Table, x => new { x.ChargeCode }, y => new { ChargeCode = y.Code }, (x, y) => new { x.Item, x.Quantity, ChargeCode = x.ChargeCode, ServLineCode = y.ServLineCode });

//var query2 = query.Join(_chargeCodeRep.Table, x => new { x.ChargeCode }, y => new { ChargeCode = y.Code }, (x, y) => new { x, y })
//    .Join(_itemRep.Table, a => new { a.x.Item }, b => new { Item = b.Code }, (a, b) => new { a.x.Item, b.Name, a.x.Quantity, ChargeCode = a.x.ChargeCode, ServLineCode = a.y.ServLineCode, b.UnitPrice, Amount = a.x.Quantity * b.UnitPrice });
//.Join(_unitRep.Table, c => new { c.ServLineCode }, d => new { ServLineCode = d.ServLineCode }, (c, d) => new { c.Item, c.Name, c.Quantity, ChargeCode = c.ChargeCode, ServLineCode = c.ServLineCode, Unit = d.Name, c.UnitPrice, Amount = c.Amount });

//var query2 = query1.Join(_itemRep.Table, x => new { x.Item }, y => new { Item = y.Name }, (x, y) => new { x.Item, y.Name, x.Quantity, ChargeCode = x.ChargeCode, ServLineCode = x.ServLineCode, y.UnitPrice, Amount = x.Quantity*y.UnitPrice  });

//var query3 = query2.Join(_unitRep.Table, x => new { x.ServLineCode }, y => new { ServLineCode = y.ServLineCode }, (x, y) => new { x.Item, x.Name, x.Quantity, ChargeCode = x.ChargeCode, ServLineCode = x.ServLineCode, Unit = y.Name, x.UnitPrice, Amount = x.Amount });

//var count = query1.Count();