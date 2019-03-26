using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisitionPortal.Models
{
    public class ReportViewModel
    {
        public ReportViewModel() { }
        public long Id { get; set; }
        public DateTime StatusDate { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Amount { get { return Quantity * UnitPrice; } }

        public string Unit { get; set; }

        public string ChargeCode { get; set; }


        public string ServLineCode { get; set; }

        public string Requestor { get; set; }
        public string Manager { get; set; }


        
    }
}