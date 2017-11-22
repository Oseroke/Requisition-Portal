﻿using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RequisitionPortal.BL.Entities
{
    public class Requisition: BaseEntity<long>
    {
        public virtual DateTime ReqDate { get; set; }
        public virtual string Requestor { get; set; }
        public virtual string Manager { get; set; }
        //public virtual Status Status { get; set; }
        public virtual int StatusID { get; set; }
        public virtual DateTime StatusDate { get; set; }
        public virtual int UnitID { get; set; }
        public virtual bool SentToAccounts { get; set; }
        public virtual bool PostedByAccounts { get; set; }
        public virtual IList<Req_Item> Items { get; set; }
    }

    public class RequisitionMap: ClassMapping<Requisition>
    {
        public RequisitionMap()
        {
            this.Lazy(true);
            this.Table("Requisitions");
            this.Id<long>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<DateTime>(x => x.ReqDate, mp => { mp.Column("ReqDate"); });
            this.Property<string>(x => x.Requestor, mp => { mp.Column("Requestor"); });
            this.Property<string>(x => x.Manager, mp => { mp.Column("Manager"); });
            this.Property<int>(x => x.UnitID, mp => { mp.Column("UnitID"); });
            this.Property<int>(x => x.StatusID, mp => { mp.Column("StatusID"); });
            this.Property<DateTime>(x => x.StatusDate, mp => { mp.Column("StatusDate"); });
            this.Bag(x => x.Items, mp => { mp.Cascade(Cascade.All); mp.Key(k => k.Column("RequisitionID")); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });
            this.Property<bool>(x => x.SentToAccounts, mp => { mp.Column("SentToAccounts"); });
            this.Property<bool>(x => x.PostedByAccounts, mp => { mp.Column("PostedByAccounts"); });
        }
    }
}
