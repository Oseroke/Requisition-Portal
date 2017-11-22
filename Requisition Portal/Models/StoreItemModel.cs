using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Requisition_Portal.Models
{
    public class StoreItemModel
    {
        public StoreItemModel()
        {
            Items = new List<SelectListItem>();
            Vendors = new List<SelectListItem>();
        }
        public long Id { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public List<SelectListItem> Items { get; set; }
        public List<SelectListItem> Vendors { get; set; }

        [Display(Name = "Vendor")]
        public int VendorID { get; set; }

        [Display(Name = "Item")]
        public int ItemID { get; set; }
        public ItemModel Item { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
    }
}