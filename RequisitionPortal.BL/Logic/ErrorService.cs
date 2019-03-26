using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Logic
{
    public class ErrorService: IErrorService
    {
        private readonly IRepository<Error, int> _errorRep;

        public ErrorService(IRepository<Error, int> errorRep)
        {
            _errorRep = errorRep;
        }

        public void LogError(Error error)
        {
            try
            {
                _errorRep.SaveOrUpdate(error); ;

            }
            catch (Exception ex)
            {
                throw new RequisitionException("The error has already been logged");
            }
            
        }
    }
}
