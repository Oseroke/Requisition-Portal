using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Requisition_Portal.Models
{
    public class ReqItemModel
    {
        //public ReqItemModel()
        //{
        //    ChargeCode = new ChargeCodeModel();
        //}

        public int Id { get; set; }

       [Required(ErrorMessage = "Select an item")]
       public string Item { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "How many?")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Select a charge code")]
        public string ChargeCode { get; set; }

        public string Manager { get; set; }

        public long RequisitionID { get; set; }
        //public Requisition Requisition { get; set; }
        //public string RequisitionName { get; set; }
        public int ItemID { get; set; }
        public int TemporaryID { get; set; }
        //public Item Item { get; set; }

        //[UIHint("_ItemList")]
        //public ItemModel Item { get; set; }

        //[UIHint("_ChargeCodeList")]
        //public ChargeCodeModel ChargeCode { get; set; }
    }
}