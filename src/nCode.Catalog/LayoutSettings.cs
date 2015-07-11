using nCode.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.Catalog.UI;

namespace nCode.Catalog
{
    /// <summary>
    /// Contains Settings for Catalog Layout.
    /// </summary>
    public static class LayoutSettings
    {
        private const string defaultItemListSettingsKey = "nCode.Catalog.DefaultItemListSettings";
        private const string defaultItemViewSettingsKey = "nCode.Catalog.DefaultItemViewSettings";

        private const string noImageImageFileKey = "nCode.Catalog.NoImageImage";
        private const string showImageInBasket = "nCode.Catalog.ShowOrderItemImage";

        /// <summary>
        /// Gets or sets the default settings for item list views.
        /// </summary>
        public static ItemListSettings DefaultItemListSettings
        {
            get { return Settings.GetProperty(defaultItemListSettingsKey, new ItemListSettings()); }
            set { Settings.SetProperty(defaultItemListSettingsKey, value); }
        }

        /// <summary>
        /// Gets or sets the default settings for item list views filtered by category.
        /// </summary>
        public static ItemListSettings DefaultCategoryItemListSettings
        {
            get { return Settings.GetProperty<ItemListSettings>(defaultItemListSettingsKey + "(C)", null); }
            set { Settings.SetProperty<ItemListSettings>(defaultItemListSettingsKey + "(C)", value); }
        }

        /// <summary>
        /// Gets or sets the default settings for item list views filtered by brand.
        /// </summary>
        public static ItemListSettings DefaultBrandItemListSettings
        {
            get { return Settings.GetProperty<ItemListSettings>(defaultItemListSettingsKey + "(B)", null); }
            set { Settings.SetProperty<ItemListSettings>(defaultItemListSettingsKey + "(B)", value); }
        }

        /// <summary>
        /// Gets or sets the default settings for item detail views.
        /// </summary>
        public static ItemViewSettings DefaultItemViewSettings
        {
            get { return Settings.GetProperty(defaultItemViewSettingsKey, new ItemViewSettings()); }
            set { Settings.SetProperty(defaultItemViewSettingsKey, value); }
        }

        /// <summary>
        /// Gets or sets the no image image file.
        /// </summary>
        public static string NoImageImageFile
        {
            get { return Settings.GetProperty<string>(noImageImageFileKey, null); }
            set { Settings.SetProperty<string>(noImageImageFileKey, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to item images in basket and orders.
        /// </summary>
        public static bool ShowOrderItemImage
        {
            get { return Settings.GetProperty<bool>(showImageInBasket, false); }
            set { Settings.SetProperty<bool>(showImageInBasket, value); }
        }
    }
}
