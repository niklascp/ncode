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

        SegmentView GetSegment(Guid segmentId);


        IEnumerable<ItemListView> ListItemByCategory(Guid categoryId, bool includeDescendantCategories = false, bool includeInactive = false, bool ? includeVat = null, int skip = 0, int? take = null);

        IEnumerable<ItemListView> ListItemBySegment(Guid segmentId, bool includeInactive = false, bool? includeVat = null, int skip = 0, int? take = null);


        IEnumerable<ItemListView> ListItemByCampaign(string campaignCode, bool? includeVat = null);

        IEnumerable<ItemListView> GetItemList(IFilterExpression<CatalogModel, Item> filter, IOrderByExpression<Item> order = null, bool? includeVat = null, int skip = 0, int? take = null);

        IEnumerable<ItemListView> SearchItems(string query, bool includeInActive = false, bool? includeVat = null, int skip = 0, int? take = null);

        ItemDetailView GetItem(string itemNo);

        IEnumerable<ItemListView> ListRelatedItems(Guid itemID);
    }
}
