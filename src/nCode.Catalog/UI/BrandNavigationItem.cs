using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents an Brand in the Navigation Framework.
    /// </summary>
    public class BrandNavigationItem : INavigationItem, IMetadataContext
    {
        public static BrandNavigationItem GetFromID(Guid id)
        {
            using (var model = new CatalogModel())
            {
                return (from b in model.Brands
                        //from g in b.Localizations.Where(x => x.Culture == null)
                        //from l in b.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        where b.ID == id
                        select new BrandNavigationItem
                        {
                            ID = b.ID,
                            ParentID = b.ParentID,
                            Image1 = b.Image1,
                            Image2 = b.Image2,
                            Image3 = b.Image3,
                            //IsVisible = b.IsVisible,
                            Title = b.Name
                        }).SingleOrDefault();
            }
        }        

        public Guid ID { get; set; }

        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the Title for this Navigation Item.
        /// </summary>
        public string Title { get; set; }

        public bool IsVisible { get; set; }

        public string Permalink
        {
            get
            {
                if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                    return string.Format("/catalog/Item-List?Brand={0}", ID.ToString());
                else
                    return string.Format("/{0}/catalog/Item-List?Brand={1}", CultureInfo.CurrentUICulture.Name.ToLower(), ID.ToString());
            }
        }

        public string Url
        {
            get
            {
                /*
                if (SeoUtilities.UseUrlRouting)
                {
                    return GetUrl("Catalog.Category");
                }
                else
                {
                    return Permalink;
                }
                */
                return Permalink;
            }
        }        

        public int Depth { get; set; }

        public bool IsActive
        {
            get
            {
                return Navigation.CurrentPath.Any(x => x.ID == ID);
            }
        }

        public string Image1 { get; set; }

        public string Image2 { get; set; }

        public string Image3 { get; set; }

        public INavigationItem GetParent()
        {
            if (ParentID != null)
                return GetFromID(ParentID.Value);
            else
                return null;
        }

        /*
        public string GetUrl(string routeName)
        {
            var seoTitle = SeoUtilities.GetSafeTitle(Title);

            if (string.IsNullOrWhiteSpace(seoTitle))
                seoTitle = CategoryNo.ToString();

            var parameters = new RouteValueDictionary  
                    { 
                        { "Culture", CultureInfo.CurrentUICulture.Name.ToLower() }, 
                        { "CategoryTitle", seoTitle } , 
                        { "CategoryNo", CategoryNo.ToString() }
                    };

            var vp = RouteTable.Routes.GetVirtualPath(null, routeName, parameters);

            return vp.VirtualPath;
        }
        */

        /* Metadata */

        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(Brand.ObjectTypeId, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(Brand.ObjectTypeId, ID, key, value);
        }
    }
}