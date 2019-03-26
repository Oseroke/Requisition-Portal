using System.ComponentModel.DataAnnotations;

namespace RequisitionPortal.Models
{
    public class InventoryItemModel
    {
        public InventoryItemModel()
        {
            Item = new ItemModel();
        }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int ItemID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get { return UnitPrice * Quantity; } }
        public string Description { get; set; }

        [UIHint("_ItemList")]
        public ItemModel Item { get; set; }


    }
}