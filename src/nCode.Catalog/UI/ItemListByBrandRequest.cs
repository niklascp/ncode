using System;
using System.Collections.Generic;
using nCode.Catalog.Data;
using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using nCode.UI;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents a item list request filtered by brand.
    /// </summary>
    public class ItemListByBrandRequest : ItemListRequest
    {
        /// <summary>
        /// Gets item list settings for the request. 
        /// </summary>
        public override ItemListSettings ListSettings
        {
            get
            {
                return LayoutSettings.DefaultBrandItemListSettings ?? base.ListSettings;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the brand requested by this item list request.
        /// </summary>
        public Guid BrandID { get; set; }

        /// <summary>
        /// Gets a contextual navigation item for the item list.
        /// </summary>
        public override INavigationItem GetNavigationItem()
        {
            return BrandNavigationItem.GetFromID(BrandID);
        }

        /// <summary>
        /// Gets a contextual view about the item list which provide information to the end user.
        /// </summary>
        public override IItemListContextView GetContextView(ICatalogRepository catelogRepository)
        {
            return catelogRepository.GetBrand(BrandID);
        }

        /// <summary>
        /// Gets a the item list requested.
        /// </summary>
        public override IEnumerable<ItemListView> GetItemList(ICatalogRepository catelogRepository)
        {
            return catelogRepository.GetItemList(
                    filter: new FilterExpression<CatalogModel, Item>(x => x.IsActive && x.BrandID == BrandID),
                    order: CatalogRepository.ItemBrandOrder
                );
        }
    }
}
