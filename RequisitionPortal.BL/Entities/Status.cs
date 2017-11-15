using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace RequisitionPortal.BL.Entities
{
    public class Status: BaseEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }

    public class StatusMap : ClassMapping<Status>
    {
        public StatusMap()
        {
            this.Table("Status");
            this.Lazy(true);
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Description, mp => { mp.Column("Description"); });
            this.Property<string>(x => x.Name, mp => { mp.Column("Name"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });

        }
    }
}

//Requisition
//Approvved
//Rejected
//Awaiting Acknowledgement
//Confrimed
