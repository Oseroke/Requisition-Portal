using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Logic
{
    public class StoreService: IStoreService
    {
        private readonly IRepository<Item, int> _itemRep;
        private readonly IRepository<Vendor, int> _vendorRep;
        private readonly IRepository<StoreItem, int> _storeItemRep;

        public StoreService(IRepository<Item, int> itemRep, IRepository<Vendor, int> vendorRep, IRepository<StoreItem, int> storeItemRep)
        {
            _itemRep = itemRep;
            _vendorRep = vendorRep;
            _storeItemRep = storeItemRep;
        }

        public IList<Item> GetItems(bool includeDeleted, int itemID)
        {
            var query = _itemRep.Table;
            
            query = query.Where(x => x.IsDeleted == false);

            if (itemID > 0)
                query.Where(x => x.Id == itemID);

            return query.ToList();
        }

        public Item GetItem(int itemID)
        {
            var query = _itemRep.Table;

            query = query.Where(x => x.IsDeleted == false);

            if (itemID > 0)
                query = query.Where(x => x.Id == itemID);

            try
            {
                return query.First();
            }
            catch
            {
                return null;
            }
        }

        public Item SaveItem(Item item)
        {
            try
            {
                _itemRep.SaveOrUpdate(item);

                return item;
            }
            catch (Exception ex)
            {
                throw new RequisitionException("An error occured.");
            }
        }

        public StoreItem SaveStoreItem(StoreItem item)
        {
            try
            {
                _storeItemRep.SaveOrUpdate(item);

                return item;
            }
            catch (Exception ex)
            {
                throw new RequisitionException("Error");
            }
        }

        public Vendor SaveVendor(Vendor vendor)
        {
            try
            {
                _vendorRep.SaveOrUpdate(vendor);

                return vendor;
            }
            catch (Exception ex)
            {
                throw new RequisitionException("An error occured.");
            }
        }

        public IList<Vendor> GetVendors(bool includeDeleted)
        {
            var query = _vendorRep.Table.Where(x => x.IsDeleted == includeDeleted);

            return query.ToList();
        }

        public Vendor GetVendor(bool includeDeleted, int id)
        {
            var query = _vendorRep.Table.Where(x => x.IsDeleted == includeDeleted);

            if (id > 0)
                query = query.Where(x => x.Id == id);

            return query.FirstOrDefault();
        }
    }
}
