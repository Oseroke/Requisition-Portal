using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RequisitionPortal.BL.Entities
{
    public class AuditAction : BaseEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual long AuditSectionId { get; set; }
        public virtual AuditSection AuditSection { get; set; }
    }

    public class AuditActionMap : ClassMapping<AuditAction>
    {
        public AuditActionMap()
        {
            this.Table("AuditActions");
            this.Lazy(true);
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Name, mp => { mp.Column("Name"); });
            this.Property<long>(x => x.AuditSectionId, mp => { mp.Column("AuditSectionId"); });
            this.ManyToOne<AuditSection>(x => x.AuditSection, mp => { mp.Column("AuditSectionId"); mp.Update(false); mp.Insert(false); mp.Lazy(LazyRelation.Proxy); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });

        }
    }
}
