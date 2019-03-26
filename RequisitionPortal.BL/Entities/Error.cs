using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Error : BaseEntity<int>
    {
        public virtual string Details { get; set; }

        public virtual string TimeStamp { get; set; }
    }

    public class ErrorMap : ClassMapping<Error>
    {
        public ErrorMap()
        {
            this.Table("Errors");
            this.Lazy(true);
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Details, mp => { mp.Column("Details"); });
            this.Property<string>(x => x.TimeStamp, mp => { mp.Column("TimeStamp"); });
        }
    }
}
