using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Inventory: BaseEntity<int>
    {
        public virtual DateTime PODate { get; set; }
        public virtual DateTime? InvDate { get; set; }
        public virtual int VendorID { get; set; }
        public virtual string PONumber { get; set; }
        public virtual string InvoiceNumber { get; set; }
        public virtual int StatusID { get; set; }
        public virtual List<InventoryItem> Items { get; set; }
        //public virtual int ItemID { get; set; }
        //public virtual int Quantity { get; set; }
        //public virtual decimal UnitPrice { get; set; }
        //public virtual decimal Amount { get; set; }
        //public virtual string Code { get; set; }
        //public virtual string Description { get; set; }
    }

    public class InventoryMap: ClassMapping<Inventory>
    {
        public InventoryMap()
        {
            this.Lazy(true);
            this.Table("Inventories");
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<int>(x => x.VendorID, mp => { mp.Column("VendorID"); });
            this.Property<DateTime>(x => x.PODate, mp => { mp.Column("PODate"); });
            this.Property<DateTime?>(x => x.InvDate, mp => { mp.Column("InvDate"); });
            this.Property<string>(x => x.PONumber, mp => { mp.Column("PONumber"); });
            this.Property<string>(x => x.InvoiceNumber, mp => { mp.Column("InvoiceNumber"); });
            this.Property<int>(x => x.StatusID, mp => { mp.Column("StatusID"); });
            this.Bag(x => x.Items, mp => { mp.Cascade(Cascade.All); mp.Key(k => k.Column("InventoryID")); });
            //this.Property<string>(x => x.Description, mp => { mp.Column("Description"); });
            //this.Property<int>(x => x.ItemID, mp => { mp.Column("ItemID"); });
            //this.Property<decimal>(x => x.UnitPrice, mp => { mp.Column("UnitPrice"); });
            //this.Property<int>(x => x.Quantity, mp => { mp.Column("Quantity"); });
            //this.Property<decimal>(x => x.Amount, mp => { mp.Column("Amount"); });
            //this.Property<string>(x => x.Code, mp => { mp.Column("Code"); });

        }
    }
}
