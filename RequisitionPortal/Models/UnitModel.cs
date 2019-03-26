using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisitionPortal.Models
{
    public class UnitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Grp { get; set; }
        public string Division { get; set; }
        public string ServLineCode { get; set; }
    }
}