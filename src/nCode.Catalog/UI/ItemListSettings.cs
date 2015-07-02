using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents settings for an item list view.
    /// </summary>
    public class ItemListSettings
    {
        /// <summary>
        /// Initializes a new instance of settings for an item list view. 
        /// </summary>
        public ItemListSettings()
        {
            ShowBreadcrumb = true;
            NoImageImageFile = LayoutSettings.NoImageImageFile;

            ShowTitle = true;
            ShowItemNo = false;
            ShowBrandName = false;
            ShowPrice = true;
            ShowDefaultPrice = false;

            ShowBuyButton = false;

            GroupByMode = ItemListGroupByMode.NoGrouping;
        }

        /* Overall visual settings */

        /// <summary>
        /// Gets or sets whether an breadcrumb element in the top of the list view.
        /// </summary>
        public bool ShowBreadcrumb { get; set; }

        /// <summary>
        /// Gets or sets the image file that should be used in no item image is available.
        /// </summary>
        public string NoImageImageFile { get; set; }

        /* Per item visual settings */

        /// <summary>
        /// Gets or sets whether the item title should be shown in the list view.
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets whether the item no. should be shown in the list view.
        /// </summary>
        public bool ShowItemNo { get; set; }

        /// <summary>
        /// Gets or sets whether the brand name should be shown in the list view.
        /// </summary>
        public bool ShowBrandName { get; set; }

        /// <summary>
        /// Gets or sets whether the price should be shown in the list view.
        /// </summary>
        public bool ShowPrice { get; set; }

        /// <summary>
        /// Gets or sets whether the default price should be shown in the list view, if the item is on sale to a lower price.
        /// </summary>
        public bool ShowDefaultPrice { get; set; }

        /// <summary>
        /// Gets or sets whether a buy button should be shown in the list view.
        /// </summary>
        public bool ShowBuyButton { get; set; }

        /* Grouping and sorting */

        /// <summary>
        /// Gets or sets the group by modes for the item list view.
        /// </summary>
        public ItemListGroupByMode GroupByMode { get; set; }

    }
}
