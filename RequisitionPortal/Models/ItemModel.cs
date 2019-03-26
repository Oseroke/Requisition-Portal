using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisitionPortal.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public virtual string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Code { get; set; }
        public decimal UnitPrice { get; set; }
    }
}