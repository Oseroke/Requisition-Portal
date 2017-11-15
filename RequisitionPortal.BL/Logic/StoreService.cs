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

        public StoreService(IRepository<Item, int> itemRep)
        {
            _itemRep = itemRep;
        }

        public IList<Item> GetItems(bool includeDeleted)
        {
            var query = _itemRep.Table;

            query = query.Where(x => x.IsDeleted == false);

            return query.ToList();
        }
    }
}
