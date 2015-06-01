using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nCode.Catalog.Data;
using nCode.Catalog.Models;
using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using nCode.UI;

namespace nCode.Catalog.UI
{
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
    }
}
