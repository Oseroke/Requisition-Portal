using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Vendor: BaseEntity<int>
    {
        public virtual string VendorUID { get; set; }
        public virtual string Name { get; set; }
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string CitySTZip { get; set; }
        public virtual string Contact { get; set; }
        public virtual string Telephone1 { get; set; }
        public virtual string Telephone2 { get; set; }
        public virtual string FaxNumber { get; set; }
        public virtual string TaxIDNo { get; set; }
        public virtual string Terms { get; set; }
        public virtual DateTime? VendSince { get; set; }
        public virtual string Email { get; set; }
    }

    public class VendorMap: ClassMapping<Vendor>
    {
        public VendorMap()
        {
            this.Lazy(true);
            this.Table("Vendors");
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.VendorUID, mp => { mp.Column("VendorUID"); });
            this.Property<string>(x => x.Name, mp => { mp.Column("Name"); });
            this.Property<string>(x => x.AddressLine1, mp => { mp.Column("AddressLine1"); });
            this.Property<string>(x => x.AddressLine2, mp => { mp.Column("AddressLine2"); });
            this.Property<string>(x => x.CitySTZip, mp => { mp.Column("CitySTZip"); });
            this.Property<string>(x => x.Telephone1, mp => { mp.Column("Telephone1"); });
            this.Property<string>(x => x.Telephone2, mp => { mp.Column("Telephone2"); });
            this.Property<string>(x => x.TaxIDNo, mp => { mp.Column("TaxIDNo"); });
            this.Property<string>(x => x.Terms, mp => { mp.Column("Terms"); });
            this.Property<string>(x => x.FaxNumber, mp => { mp.Column("FaxNumber"); });
            this.Property<DateTime?>(x => x.VendSince, mp => { mp.Column("VendSince"); });
            this.Property<string>(x => x.Contact, mp => { mp.Column("Contact"); });
            this.Property<string>(x => x.Email, mp => { mp.Column("Email"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });


        }
    }
}
