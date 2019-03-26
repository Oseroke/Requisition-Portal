using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisitionPortal.Models
{
    public class InventoryModel
    {
        public InventoryModel()
        {
            Items = new List<SelectListItem>();
            Vendors = new List<SelectListItem>();
            InventoryItems = new List<InventoryItemModel>();
        }
        public long Id { get; set; }
        [Display(Name = "Vendor")]
        public int VendorID { get; set; }
        [Required(ErrorMessage = "Enter PO Number")]
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        [Display(Name = "PO Date")]
        public DateTime PODate { get; set; }
        [Display(Name = "Invoice Date")]
        public DateTime InvDate { get; set; }
        public IList<InventoryItemModel> InventoryItems { get; set; }
        public List<SelectListItem> Items { get; set; }
        public List<SelectListItem> Vendors { get; set; }
    }
}