using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class ChargeCode:BaseEntity<int>
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual string ServLineCode { get; set; }
        public virtual string GroupCode { get; set; }
        public virtual string TaskPartner { get; set; }
        public virtual string TaskManager { get; set; }
        public virtual string JobCodeType { get; set; } // chargeable or nonchargeable
        public virtual string Department { get; set; }
    }

    public class ChargeCodeMap: ClassMapping<ChargeCode>
    {
        public ChargeCodeMap()
        {
            this.Table("LedgerCodes");
            this.Lazy(true);
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Code, mp => { mp.Column("Code"); });
            this.Property<string>(x => x.Description, mp => { mp.Column("Description"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });
            this.Property<string>(x => x.ServLineCode, mp => { mp.Column("ServLineCode"); });
            this.Property<string>(x => x.GroupCode, mp => { mp.Column("GroupCode"); });
            this.Property<string>(x => x.TaskManager, mp => { mp.Column("TaskManager"); });
            this.Property<string>(x => x.TaskPartner, mp => { mp.Column("TaskPartner"); });
            this.Property<string>(x => x.JobCodeType, mp => { mp.Column("JobCodeType"); });
            this.Property<string>(x => x.Department, mp => { mp.Column("Department"); });
        }
    }
}
