using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public interface ISetupService
    {
        IList<ChargeCode> GetChargeCodes(bool includeDeleted);
        IList<ChargeCode> GetChargeableChargeCodes(bool includeDeleted);
        IList<ChargeCode> GetNonChargeableChargeCodes(bool includeDeleted, string servLineCode);
        IList<Unit> GetAllUnits(bool includeDeleted);

    }
}
