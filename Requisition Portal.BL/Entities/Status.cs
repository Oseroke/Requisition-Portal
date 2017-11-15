using System;
using System.Collections.Generic;
using System.Text;

namespace RequisitionPortal.BL.Entities
{
    public class Status: BaseEntity<int>
    {
        public virtual string Name { get; set; }
    }
}

//Requisition
//Approvved
//Rejected
//Awaiting Acknowledgement
//Confrimed
