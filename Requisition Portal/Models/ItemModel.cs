using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requisition_Portal.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public virtual string Description { get; set; }
        public int Quantity { get; set; }
        public string Code { get; set; }
    }
}