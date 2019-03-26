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
    public class SetupService: ISetupService
    {
        private readonly IRepository<ChargeCode, int> _chargeCodeRep;
        private readonly IRepository<Unit, int> _unitRep;

        public SetupService(IRepository<ChargeCode, int> chargeCodeRep, IRepository<Unit, int> unitRep)
        {
            _chargeCodeRep = chargeCodeRep;
            _unitRep = unitRep;
        }

        public IList<ChargeCode> GetChargeCodes(bool includeDeleted)
        {
            var query = _chargeCodeRep.Table;

            query = query.Where(x => x.IsDeleted == includeDeleted);

            return query.OrderBy(x=>x.Code).ToList();
        }

        public IList<ChargeCode> GetChargeableChargeCodes(bool includeDeleted)
        {
            var query = _chargeCodeRep.Table;

            query = query.Where(x => x.IsDeleted == includeDeleted);

            
            query = query.Where(x => x.JobCodeType == SystemEnums.JobCodeType.Chargeable.ToString());
            

            return query.OrderBy(x=>x.Code).ToList();
        }

        public IList<ChargeCode> GetNonChargeableChargeCodes(bool includeDeleted, string servLineCode)
        {
            var query = _chargeCodeRep.Table;

            query = query.Where(x => x.IsDeleted == includeDeleted);

            query = query.Where(x => x.JobCodeType == SystemEnums.JobCodeType.NonChargeable.ToString());
            
            if (!string.IsNullOrEmpty(servLineCode))
            {
                query = query.Where(x => x.ServLineCode == servLineCode);
            }

            return query.OrderBy(x=>x.Code).ToList();
        }

        public IList<ChargeCode> GetNonChargeableChargeCodesByDepartment(bool includeDeleted, string servLineCode)
        {
            var query = _chargeCodeRep.Table;

            query = query.Where(x => x.IsDeleted == includeDeleted);

            query = query.Where(x => x.JobCodeType == SystemEnums.JobCodeType.NonChargeable.ToString());

            if (!string.IsNullOrEmpty(servLineCode))
            {
                query = query.Where(x => x.ServLineCode == servLineCode);
            }

            return query.OrderBy(x=>x.Code).ToList();
        }


        public IList<Unit> GetAllUnits(bool includeDeleted)
        {
            var query = _unitRep.Table.Where(x=>x.IsDeleted == includeDeleted);

            return query.OrderBy(x=>x.Name).ToList();
        }
    }
}
