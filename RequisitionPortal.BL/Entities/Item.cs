using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RequisitionPortal.BL.Entities
{
    public class Item: BaseEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual int ReorderLevel { get; set; }
        public virtual string Code { get; set; }
        public virtual string Class { get; set; }
        public virtual string Active { get; set; }
        public virtual string Type { get; set; }
    }

    public class ItemMap: ClassMapping<Item>
    {
        public ItemMap()
        {
            this.Lazy(true);
            this.Table("Items");
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Name, mp => { mp.Column("Name"); });
            this.Property<decimal>(x => x.Quantity, mp => { mp.Column("Quantity"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });
            this.Property<decimal>(x => x.UnitPrice, mp => { mp.Column("UnitPrice"); });
            this.Property<int>(x => x.ReorderLevel, mp => { mp.Column("ReorderLevel"); });
            this.Property<string>(x => x.Code, mp => { mp.Column("Code"); });
            this.Property<string>(x => x.Class, mp => { mp.Column("Class"); });
            this.Property<string>(x => x.Active, mp => { mp.Column("Active"); });
            this.Property<string>(x => x.Type, mp => { mp.Column("Type"); });


        }
    }
}
