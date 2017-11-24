using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Requisition_Portal.Models
{
    public class RequisitionModel
    {
        public RequisitionModel()
        {
            Items = new List<ReqItemModel>();
            Managers = new List<SelectListItem>();
        }

        public long Id { get; set; }
        public DateTime ReqDate { get; set; }
        public string ReqDateString {  get { return ReqDate.ToShortDateString(); } }
        public string Requestor { get; set; }

        [Required(ErrorMessage="Select a manager"), Display(Name = "Manager: ")]
        public string Manager { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusDateString { get { return StatusDate.ToShortDateString(); } }
        public int UnitID { get; set; }
        public IList<ReqItemModel> Items { get; set; }
        public List<SelectListItem> Managers { get; set; }
        

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
                    case 9:
                        return "Store Officer cancelled";

                    default:
                        return "Unknown";
                }
            }
        }
    }
}