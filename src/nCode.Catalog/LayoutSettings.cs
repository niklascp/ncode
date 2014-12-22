using nCode.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog
{
    /// <summary>
    /// Contains Settings for Catalog Layout.
    /// </summary>
    public static class LayoutSettings
    {
        private const string defaultItemListSettingsKey = "nCode.Catalog.DefaultItemListSettings";

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
        /// Gets or sets the no image image file.
        /// </summary>
        public static string NoImageImageFile
        {
            get { return Settings.GetProperty<string>(noImageImageFileKey, null); }
            set { Settings.SetProperty<string>(noImageImageFileKey, value);  }
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
