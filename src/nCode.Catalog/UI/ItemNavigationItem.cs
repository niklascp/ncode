using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;
using System.Collections.Generic;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents an Item in the Navigation Framework.
    /// </summary>
    public class ItemNavigationItem : INavigationItem, IMetadataContext
    {
        /// <summary>
        /// Gets or sets the Item ID for this Item Navigation Item.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the ItemNo for this Item Navigation Item.
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// Gets or sets the Title for this Navigation Item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets an Permanent Url that allow access to this resource.
        /// </summary>
        public string Permalink
        {
            get
            {
                if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                    return string.Format("/catalog/Item-View?ID={0}", ID.ToString());
                else
                    return string.Format("/{0}/catalog/Item-View?ID={1}", CultureInfo.CurrentUICulture.Name.ToLower(), ID.ToString());
            }
        }

        /// <summary>
        /// Gets an Frindly Url that allow access to this resource.
        /// </summary>
        public string Url
        {
            get
            {
                if (SeoUtilities.UseUrlRouting)
                {
                    return SeoUtilities.GetItemUrl(ItemNo, Title);
                }
                else
                {
                    return Permalink;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Item is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Navigation.CurrentPath.Any(x => x.ID == ID);
            }
        }

        /// <summary>
        /// Gets or sets the primery parent Navigation Item.
        /// </summary>
        public INavigationItem Parent { get; set; }

        /// <summary>
        /// Gets the primery parent Navigation Item.
        /// </summary>
        public INavigationItem GetParent()
        {
            return Parent;
        }

        /* Metadata */

        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(Item.ObjectTypeId, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(Item.ObjectTypeId, ID, key, value);
        }
    }
}