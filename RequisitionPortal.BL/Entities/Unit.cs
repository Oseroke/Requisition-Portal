using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Unit : BaseEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Grp { get; set; }
        public virtual string Division { get; set; }
        public virtual string ServLineCode { get; set; }
    }

    public class UnitMap: ClassMapping<Unit>
    {
        public UnitMap()
        {
            this.Lazy(true);
            this.Table("Units");
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Name, mp => { mp.Column("Name"); });
            this.Property<string>(x => x.Grp, mp => { mp.Column("Grp"); });
            this.Property<string>(x => x.Division, mp => { mp.Column("Division"); });
            this.Property<string>(x => x.ServLineCode, mp => { mp.Column("ServLineCode"); });

        }
    }

}
