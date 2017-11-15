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
    }

    public class ChargeCodeMap: ClassMapping<ChargeCode>
    {
        public ChargeCodeMap()
        {
            this.Table("ChargeCodes");
            this.Lazy(true);
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Code, mp => { mp.Column("Code"); });
            this.Property<string>(x => x.Description, mp => { mp.Column("Description"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });

        }
    }
}
