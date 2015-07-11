using System.Collections.Generic;
using nCode.Catalog.ViewModels;

namespace nCode.Catalog.ViewModels
{
    /// <summary>
    /// Represents an entry in an item list grouping.
    /// </summary>
    public class ItemListGroupView
    {
        /// <summary>
        /// Gets or sets the title of the group.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the items in the group.
        /// </summary>
        public IEnumerable<ItemListView> Items { get; set; }
    }
}
