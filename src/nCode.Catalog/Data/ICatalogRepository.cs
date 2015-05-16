using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace nCode.Catalog.Data
{
    public interface ICatalogRepository : IDisposable
    {
        CategoryView GetCategory(Guid categoryId);

        BrandView GetBrand(Guid brandId);

        IEnumerable<ItemListView> GetItemList(IFilterExpression<CatalogModel, Item> filter, IOrderByExpression<Item> order = null, int skip = 0, int? take = null);

        ItemDetailView GetItemDetail(string itemNo);
    }
}
