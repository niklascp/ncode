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
    public class ItemListBySegmentRequest : ItemListRequest
    {
        /// <summary>
        /// Gets the unique identifier of the segment requested by this item list request.
        /// </summary>
        public Guid SegmentID { get; set; }

        /// <summary>
        /// Gets a contextual navigation item for the item list.
        /// </summary>
        public override INavigationItem GetNavigationItem()
        {
            return SegmentNavigationItem.GetFromID(SegmentID);
        }

        /// <summary>
        /// Gets a contextual view about the item list which provide information to the end user.
        /// </summary>
        public override IItemListContextView GetContextView(ICatalogRepository catelogRepository)
        {
            return catelogRepository.GetSegment(SegmentID);
        }

        /// <summary>
        /// Gets a the item list requested.
        /// </summary>
        public override IEnumerable<ItemListView> GetItemList(ICatalogRepository catelogRepository)
        {
            return catelogRepository.ListItemBySegment(SegmentID);
        }
    }
}
