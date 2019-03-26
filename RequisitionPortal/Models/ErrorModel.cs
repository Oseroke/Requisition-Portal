using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requisition_Portal.Models
{
    public class ErrorModel
    {
        public long Id { get; set; }
        public string Details { get; set; }
        public string TimeStamp { get; set; }
    }
}