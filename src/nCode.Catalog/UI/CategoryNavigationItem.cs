using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using nCode.Metadata;
using nCode.Catalog.UI;
using System.Web.Routing;
using System.Web;
using nCode.UI;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents an Category in the Navigation Framework.
    /// </summary>
    public class CategoryNavigationItem : TreeNavigationItem, IMetadataContext
    {
        public static CategoryNavigationItem GetFromID(Guid id)
        {
            using (var model = new CatalogModel())
            {
                return (from c in model.Categories
                        from g in c.Localizations.Where(x => x.Culture == null)
                        from l in c.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        where c.ID == id
                        select new CategoryNavigationItem
                        {
                            ID = c.ID,
                            ParentID = c.ParentID,
                            CategoryNo = c.CategoryNo,
                            Image1 = c.Image1,
                            Image2 = c.Image2,
                            Image3 = c.Image3,
                            IsVisible = c.IsVisible,
                            Title = (l ?? g).Title
                        }).SingleOrDefault();
            }
        }

        public int CategoryNo { get; set; }

        public string Permalink
        {
            get
            {
                if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                    return string.Format("/catalog/Item-List?Category={0}", ID.ToString());
                else
                    return string.Format("/{0}/catalog/Item-List?Category={1}", CultureInfo.CurrentUICulture.Name.ToLower(), ID.ToString());
            }
        }

        public override string Url
        {
            get
            {
                if (SeoUtilities.UseUrlRouting)
                {
                    return GetUrl("Catalog.Category");
                }
                else
                {
                    return Permalink;
                }
            }
        }

        public string Image1 { get; set; }

        public string Image2 { get; set; }

        public string Image3 { get; set; }

        public string GetUrl(string routeName)
        {
            var seoTitle = SeoUtilities.GetSafeTitle(Title);

            /* Ensure a seo title */
            if (string.IsNullOrWhiteSpace(seoTitle))
                seoTitle = CategoryNo.ToString();

            var parameters = new RouteValueDictionary
                    { 
                        { "CategoryTitle", seoTitle } , 
                        { "CategoryNo", CategoryNo.ToString() }
                    };

            if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault()))
            {
                routeName += "(DefaultCulture)";
            }
            else
            {
                routeName += "(SpecificCulture)";
                parameters.Add("Culture", CultureInfo.CurrentUICulture.Name.ToLower());
            }

            var vp = RouteTable.Routes.GetVirtualPath(null, routeName, parameters);

            return vp.VirtualPath;
        }

        /// <summary>
        /// Gets a possibly context-specific logical Parent of this Navigation Item in the Navigation Graph. This is used to automatically calculate the breadcrumb path available in <see cref="Navigation.CurrentPath" />.
        /// </summary>
        /// <returns></returns>
        public override INavigationItem GetParent()
        {
            if (ParentID != null)
                return GetFromID(ParentID.Value);
            return null;
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
            return GenericMetadataHelper.GetProperty<T>(Category.ObjectTypeId, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(Category.ObjectTypeId, ID, key, value);
        }

    }

}