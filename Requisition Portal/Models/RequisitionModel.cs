using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Requisition_Portal.Models
{
    public class RequisitionModel
    {
        public RequisitionModel()
        {
            Items = new List<ReqItemModel>();
        }

        public long Id { get; set; }
        public DateTime ReqDate { get; set; }
        public string ReqDateString {  get { return ReqDate.ToShortDateString(); } }
        public string Requestor { get; set; }

        [Required(ErrorMessage="Select a manager")]
        public string Manager { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusDateString { get { return StatusDate.ToShortDateString(); } }
        public int UnitID { get; set; }
        public IList<ReqItemModel> Items { get; set; }

        public string StatusString
        {
            get
            {
                switch (StatusID)
                {
                    case 1:
                        return "Awaiting Manager Approval";
                    case 2:
                        return "Approved by Manager";
                    case 3:
                        return "Awaiting Pickup";
                    case 4:
                        return "Awaiting Acknowledgement";
                    case 5:
                        return "Completed";
                    case 6:
                        return "User Cancelled";
                    case 7:
                        return "Manager Cancelled";
                    case 8:
                        return "Out of Stock";

                    default:
                        return "Unknown";
                }
            }
        }
    }
}