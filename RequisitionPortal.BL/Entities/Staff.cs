using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Staff: BaseEntity<int>
    {
        public virtual Guid EmpID { get; set; }
        public virtual string EmpCode { get; set; }
        public virtual string EmpName { get; set; }
        public virtual string EmpSurname { get; set; }
        public virtual string EmpFirstName { get; set; }
        public virtual string EmpCatCode { get; set; }
        public virtual string EmpCat { get; set; }
        public virtual string ServLineCode { get; set; }
        public virtual string EmpLogin { get; set; }
        public virtual string EmpEmail { get; set; }
    }

    public class StaffMapping: ClassMapping<Staff>
    {
        public StaffMapping()
        {
            this.Lazy(true);
            this.Table("Staff");
            this.Property<Guid>(x => x.EmpID, mp => { mp.Column("EmpID"); });
            this.Property<string>(x => x.EmpCode, mp => { mp.Column("EmpCode"); });
            this.Property<string>(x => x.EmpName, mp => { mp.Column("EmpName"); });
            this.Property<string>(x => x.EmpSurname, mp => { mp.Column("EmpSurname"); });
            this.Property<string>(x => x.EmpFirstName, mp => { mp.Column("EmpFirstName"); });
            this.Property<string>(x => x.EmpCatCode, mp => { mp.Column("EmpCatCode"); });
            this.Property<string>(x => x.EmpCat, mp => { mp.Column("EmpCat"); });
            this.Property<string>(x => x.ServLineCode, mp => { mp.Column("ServLineCode"); });
            this.Property<string>(x => x.EmpLogin, mp => { mp.Column("EmpLogin"); });
            this.Property<string>(x => x.EmpEmail, mp => { mp.Column("EmpEmail"); });
        }
    }
}
