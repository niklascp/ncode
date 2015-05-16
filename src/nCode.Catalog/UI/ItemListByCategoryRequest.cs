using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nCode.Catalog.Data;
using nCode.Catalog.ViewModels;
using nCode.Data.Linq;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents a item list request filtered by category.
    /// </summary>
    public class ItemListByCategoryRequest : ItemListRequest
    {
        /// <summary>
        /// Gets the unique identifier of the category requested by this item list request.
        /// </summary>
        public Guid CategoryID { get; set; }

        /// <summary>
        /// Gets a contextual view about the item list which provide information to the end user.
        /// </summary>
        public override IItemListContextView GetContextView(ICatalogRepository catelogRepository)
        {
            return catelogRepository.GetCategory(CategoryID);
        }

        /// <summary>
        /// Gets a the item list requested.
        /// </summary>
        public override IEnumerable<ItemListView> GetItemList(ICatalogRepository catelogRepository)
        {
            return catelogRepository.GetItemList(
                    filter: new FilterExpression<CatalogModel, Item>(x => x.CategoryID == CategoryID),
                    order: CatalogRepository.ItemCategoryOrder
                );
        }
    }
}
