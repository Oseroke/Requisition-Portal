using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public interface IStoreService
    {
        IList<Item> GetItems(bool includeDeleted, int itemID);

        Item GetItem(int itemID);

        Item GetItem(string item);

        Item SaveItem(Item item);

        IList<Vendor> GetVendors(bool includeDeleted);

        StoreItem SaveStoreItem(StoreItem item);

        Vendor SaveVendor(Vendor vendor);

        Vendor GetVendor(bool includeDeleted, int id);
        IList<StoreItem> GetStoreItems(bool includeDeleted, string PONumber);
    }
}
