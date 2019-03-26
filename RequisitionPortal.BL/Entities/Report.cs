using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Report : BaseEntity<long>
    {
        public virtual DateTime StatusDate { get; set; }

        public virtual string Name { get; set; }

        public virtual int Quantity { get; set; }

        public virtual decimal UnitPrice { get; set; }

        public virtual string Unit { get; set; }

        public virtual string ChargeCode { get; set; }


        public virtual string ServLineCode { get; set; }

        public virtual string Requestor { get; set; }
        public virtual string Manager { get; set; }
        public virtual int StatusID { get; set; }
        //public virtual decimal Amount { get; set; }
        //public virtual bool IsDeleted { get; set; }
    }

    public class ReportMap : ClassMapping<Report>
    {
        public ReportMap()
        {
            this.Lazy(true);
            this.Table("vw_Report");
            this.Id<long>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Assigned); });
            this.Property<DateTime>(x => x.StatusDate, mp => { mp.Column("StatusDate"); });
            this.Property<string>(x => x.Name, mp => { mp.Column("Name"); });
            this.Property<string>(x => x.Unit, mp => { mp.Column("Unit"); });
            this.Property<int>(x => x.Quantity, mp => { mp.Column("Quantity"); });
            this.Property<decimal>(x => x.UnitPrice, mp => { mp.Column("UnitPrice"); });
            this.Property<string>(x => x.ChargeCode, mp => { mp.Column("ChargeCode"); });
            this.Property<string>(x => x.ServLineCode, mp => { mp.Column("ServLineCode"); });
            this.Property<string>(x => x.Requestor, mp => { mp.Column("Requestor"); });
            this.Property<string>(x => x.Manager, mp => { mp.Column("Manager"); });
            //this.Property<decimal>(x => x.Amount, mp => { mp.Column("Amount"); });
            this.Property<int>(x => x.StatusID, mp => { mp.Column("StatusID"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });


            // this.ManyToOne<User>(x => x.Requestor, mp => { mp.Lazy(LazyRelation.Proxy); mp.Update(false); mp.Insert(false); mp.Column("Requestor"); });
            //this.ManyToOne<User>(x => x.Manager, mp => { mp.Lazy(LazyRelation.Proxy); mp.Update(false); mp.Insert(false); mp.Column("Manager"); });
        }
    }
}
