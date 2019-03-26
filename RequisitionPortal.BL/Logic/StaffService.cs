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
        private readonly IRepository<User, int> _userRep;

        public StaffService(IRepository<Staff, int> staffRep, IRepository<User, int> userRep)
        {
            _staffRep = staffRep;
            _userRep = userRep;
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
                return query.OrderBy(x=>x.EmpSurname).ToList();
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
                return query.OrderBy(x => x.EmpSurname).ToList();
            }
            catch
            {
                return null;
            }
        }

        public User GetUserByUsername(bool isDeleted, string username)
        {
            var query = _userRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(x => x.Username == username);
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

        public IList<User> GetManagersByDepartment(bool isDeleted, string department)
        {
            var query = _userRep.Table.Where(x => x.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(x => x.Department == department);
            }

            query = query.Where(x => x.JobTitle.Contains("Manager") || x.JobTitle.Contains("Supervisor") || x.JobTitle.Contains("Director") || x.JobTitle.Contains("Partner"));

            try
            {
                return query.OrderBy(x => x.LastName).ToList();
            }
            catch
            {
                return null;
            }

        }
    }
}
