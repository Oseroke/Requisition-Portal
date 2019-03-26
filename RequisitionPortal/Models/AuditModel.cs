using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisitionPortal.Models
{
    public class AuditModel
    {
        public long Id { get; set; }
        public string AuditAction { get; set; }
        public string Details { get; set; }
        //public virtual string UserIp { get; set; }
        public string TimeStamp { get; set; }
    }
}