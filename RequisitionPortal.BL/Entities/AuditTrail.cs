using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RequisitionPortal.BL.Entities
{
    public class AuditTrail : BaseEntity<long>
    {
        //public virtual long PersonId { get; set; }
        //public virtual PersonalInformation PersonalInformation { get; set; }
        public virtual int AuditActionId { get; set; }
        //public virtual AuditAction AuditAction { get; set; }
        public virtual string Details { get; set; }
        public virtual string UserIp { get; set; }
        public virtual DateTime TimeStamp { get; set; }
    }

    public class AuditTrailMap : ClassMapping<AuditTrail>
    {
        public AuditTrailMap()
        {
            this.Table("AuditTrails");
            this.Lazy(true);
            this.Id<long>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.UserIp, mp => { mp.Column("UserIP"); });
            this.Property<string>(x => x.Details, mp => { mp.Column("Details"); });
            this.Property<DateTime>(x => x.TimeStamp, mp => { mp.Column("DateTimeStamp"); });
            this.Property<int>(x => x.AuditActionId, mp => { mp.Column("AuditActionId"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });
            //this.Property<DateTime>(x => x.LastDateEditted, mp => { mp.Column("LastEditDate"); });
            //this.Property<long>(x => x.PersonId, mp => { mp.Column("PersonId"); });
            //  this.Property<long?>(x => x.AirlineId, mp => { mp.Column("AirlineId"); });
            // this.ManyToOne<AuditAction>(x => x.AuditAction, mp => { mp.Lazy(LazyRelation.Proxy); mp.Update(false); mp.Insert(false); mp.Column("AuditActionId"); });

            //this.ManyToOne<PersonalInformation>(x => x.PersonalInformation, mp => { mp.Lazy(LazyRelation.Proxy); mp.Update(false); mp.Insert(false); mp.Column("PersonId"); });


        }
    }
}
