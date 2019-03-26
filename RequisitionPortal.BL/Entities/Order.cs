using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class Order: BaseEntity<long>
    {
        public int VendorID { get; set; }
        public DateTime Date { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public List<StoreItem> Items { get; set; }
    }
}
