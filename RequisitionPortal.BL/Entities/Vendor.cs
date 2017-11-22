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
        public virtual string Address { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string ContactPerson { get; set; }
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
            this.Property<string>(x => x.Address, mp => { mp.Column("Address"); });
            this.Property<string>(x => x.PhoneNumber, mp => { mp.Column("PhoneNumber"); });
            this.Property<string>(x => x.ContactPerson, mp => { mp.Column("ContactPerson"); });
            this.Property<string>(x => x.Email, mp => { mp.Column("Email"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });


        }
    }
}
