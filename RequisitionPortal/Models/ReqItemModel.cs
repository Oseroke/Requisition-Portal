﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RequisitionPortal.Models
{
    public class ReqItemModel
    {
        public ReqItemModel()
        {
            ChargeCode = new ChargeCodeModel();
            Item = new ItemModel();

        }

        public int Id { get; set; }

       //[Required(ErrorMessage = "Select an item")]
       public string ItemName { get; set; }

       // public ItemModel ItemNew { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "How many?"),Range(1, 100)]
        public int Quantity { get; set; }

        //[Required(ErrorMessage = "Select a charge code")]
        //public string ChargeCode { get; set; }

        public string Manager { get; set; }

        public long RequisitionID { get; set; }
        //public Requisition Requisition { get; set; }
        //public string RequisitionName { get; set; }
        public int ItemID { get; set; }
        public int TemporaryID { get; set; }
        //public Item Item { get; set; }

        [UIHint("_ItemList")]
        public ItemModel Item { get; set; }

        [UIHint("_ChargeCodeList")]
        public ChargeCodeModel ChargeCode { get; set; }

        public string cc { get { return ChargeCode.Code; } }

        public decimal Amount { get { return Quantity * Item.UnitPrice; } }
    }
}