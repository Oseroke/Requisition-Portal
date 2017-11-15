using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
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

        public SetupService(IRepository<ChargeCode, int> chargeCodeRep)
        {
            _chargeCodeRep = chargeCodeRep;
        }

        public IList<ChargeCode> GetChargeCodes(bool includeDeleted)
        {
            var query = _chargeCodeRep.Table;

            query = query.Where(x => x.IsDeleted == false);

            return query.ToList();
        }
    }
}
