using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public interface IStaffService
    {
        Staff GetStaffByEmpCode(bool isDeleted, string empCode);
        Staff GetStaffByUsername(bool isDeleted, string empLogin);
        string GetEmpCodeByUsername(bool isDeleted, string username);
        IList<Staff> GetAllStaff(bool isDeleted);
        IList<Staff> GetManagers(bool isDeleted, string servLine);
        IList<Staff> GetManagers(bool isDeleted, Staff staff);
    }
}
