using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Logic
{
    public class StaffService : IStaffService
    {
        private readonly IRepository<Staff, int> _staffRep;

        public StaffService(IRepository<Staff, int> staffRep)
        {
            _staffRep = staffRep;
        }
        
        public Staff GetStaffByEmpCode(bool isDeleted, string empCode)
        {
            var query = _staffRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(empCode))
            {
                query = query.Where(x => x.EmpCode == empCode);
            }

            try
            {
                return query.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public Staff GetStaffByUsername(bool isDeleted, string empLogin)
        {
            var query = _staffRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(empLogin))
            {
                query = query.Where(x => x.EmpLogin == empLogin);
            }

            try
            {
                return query.FirstOrDefault();
            }
            catch
            {
                return null;
            }

        }

        public string GetEmpCodeByUsername(bool isDeleted, string username)
        {
            var query = _staffRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(x => x.EmpLogin == username);
            }

            try
            {
                return query.FirstOrDefault().EmpCode;
            }
            catch
            {
                return null;
            }
        }

        public IList<Staff> GetAllStaff(bool isDeleted)
        {
            var query = _staffRep.Table.Where(x => x.IsDeleted == isDeleted);

            return query.ToList();

        }

        public IList<Staff> GetManagers(bool isDeleted, string servLine)
        {
            var query = _staffRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(servLine))
            {
                query = query.Where(x => x.ServLineCode == servLine);
            }

            query = query.Where(x => x.EmpCat == "Manager");

            try
            {
                return query.ToList();
            }
            catch
            {
                return null;
            }

        }

        public IList<Staff> GetManagers(bool isDeleted, Staff staff)
        {
            var query = _staffRep.Table.Where(x => x.IsDeleted == isDeleted);

            query = query.Where(x => x.ServLineCode == staff.ServLineCode);

            query = query.Where(x => x.EmpCat == "Manager");

            try
            {
                return query.ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
