using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class StoreItem: BaseEntity<int>
    {
        public virtual int ItemID { get; set; }
        //public virtual string Code { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int VendorID { get; set; }
        public virtual string PONumber { get; set; }
        public virtual string InvoiceNumber { get; set; }
        public virtual decimal Amount { get; set; }
    }

    public class StoreItemMap: ClassMapping<StoreItem>
    {
        public StoreItemMap()
        {
            this.Lazy(true);
            this.Table("StoreItems");
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<int>(x => x.ItemID, mp => { mp.Column("ItemID"); });
            this.Property<decimal>(x => x.UnitPrice, mp => { mp.Column("UnitPrice"); });
            this.Property<int>(x => x.Quantity, mp => { mp.Column("Quantity"); });
            this.Property<decimal>(x => x.Amount, mp => { mp.Column("Amount"); });
            this.Property<int>(x => x.VendorID, mp => { mp.Column("VendorID"); });
            this.Property<DateTime>(x => x.Date, mp => { mp.Column("Date"); });
            this.Property<string>(x => x.PONumber, mp => { mp.Column("PONumber"); });
            this.Property<string>(x => x.InvoiceNumber, mp => { mp.Column("InvoiceNumber"); });

        }
    }
}
