using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RequisitionPortal.BL.Entities
{
    public class Req_Item: BaseEntity<int>
    {
        public virtual string Item { get; set; }
        public virtual string Description { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string ChargeCode { get; set; }
        public virtual long RequisitionID { get; set; }
        public virtual int ItemNo { get; set; }

        public virtual Requisition Requisition { get; set; }
        //public virtual string RequisitionName { get; set; }
        //public virtual Item Item { get; set; }
    }


    public class Req_Item_Map: ClassMapping<Req_Item>
    {
        public Req_Item_Map()
        {
            this.Lazy(true);
            this.Table("Req_Items");
            this.Id<long>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Item, mp => { mp.Column("Item"); });
            this.Property<string>(x => x.Description, mp => { mp.Column("Description"); });
            this.Property<int>(x => x.Quantity, mp => { mp.Column("Quantity"); });
            this.Property<string>(x => x.ChargeCode, mp => { mp.Column("ChargeCode"); });
            this.Property<int>(x => x.ItemNo, mp => { mp.Column("ItemNo"); });

            //this.Property<long>(x => x.RequisitionID, mp => { mp.Column("RequisitionID"); });
            this.ManyToOne<Requisition>(x => x.Requisition, mp => { mp.Lazy(LazyRelation.Proxy); mp.Update(false); mp.Insert(false); mp.Column("RequisitionID"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });

            //this.ManyToOne<Requisition>(x => x.Requisition, mp => {  mp.Column("RequisitionID"); });
            //this.ManyToOne<Item>(x => x.Item, mp => { mp.Lazy(LazyRelation.Proxy); mp.Update(false); mp.Insert(false); mp.Column("ItemID"); });


        }
    }
}
