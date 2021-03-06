﻿using System.Collections.Generic;
using System.Linq;

using nCode.Catalog.Data;
using nCode.Catalog.ViewModels;
using nCode.UI;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents information and helper methods for an Item List Request.
    /// </summary>
    public abstract class ItemListRequest
    {
        /// <summary>
        /// Gets item list settings for the request. 
        /// </summary>
        public virtual ItemListSettings ListSettings
        {
            get
            {
                return LayoutSettings.DefaultItemListSettings;
            }
        }

        /// <summary>
        /// Gets a contextual navigation item for the item list.
        /// </summary>
        public abstract INavigationItem GetNavigationItem();

        /// <summary>
        /// Gets a contextual view about the item list which provide information to the end user.
        /// </summary>
        public abstract IItemListContextView GetContextView(ICatalogRepository catelogRepository);

        /// <summary>
        /// Gets a the item list requested.
        /// </summary>
        public abstract IEnumerable<ItemListView> GetItemList(ICatalogRepository catelogRepository);

        /// <summary>
        /// Gets a the item list requested, grouped according to the GroupByMode specified in the <see cref="ItemListSettings"/> for the request.
        /// </summary>
        public virtual IEnumerable<ItemListGroupView> GetItemGroupList(ICatalogRepository catalogRepository)
        {
            var items = GetItemList(catalogRepository);
            IEnumerable<ItemListGroupView> itemGroups = null;

            if (ListSettings.GroupByMode == ItemListGroupByMode.NoGrouping)
            {
                /* Add dummy group if no grouping. */
                itemGroups = new ItemListGroupView[] { new ItemListGroupView() { 
                    Items = items
                }};
            }
            else if (ListSettings.GroupByMode == ItemListGroupByMode.GroupByCategory)
            {
                itemGroups = items
                    .GroupBy(x => new { x.CategoryTitle })
                    .OrderBy(x => x.Min(y => y.BrandIndex)).Select(x => new ItemListGroupView
                    {
                        Title = x.Key.CategoryTitle,
                        Items = x.OrderBy(y => y.BrandIndex)
                    });
            }
            else if (ListSettings.GroupByMode == ItemListGroupByMode.GroupByBrand)
            {
                itemGroups = items
                    .GroupBy(x => new { x.BrandName })
                    .OrderBy(x => x.Min(y => y.CategoryIndex) ).Select(x => new ItemListGroupView
                    {
                        Title = x.Key.BrandName,
                        Items = x.OrderBy(y => y.CategoryIndex)
                    });
            }

            return itemGroups;
        }
    }
}
