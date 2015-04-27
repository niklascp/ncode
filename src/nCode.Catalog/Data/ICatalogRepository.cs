using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace nCode.Catalog.Data
{
    interface ICatalogRepository : IDisposable
    {
        IEnumerable<ItemListView> GetItemList(IFilterExpression<CatalogModel, Item> sourceFilter, IOrderByExpression<Item> sourceOrder, int skip = 0, int? take = null);

        ItemDetailView GetItemDetail(string itemNo);
    }
}
