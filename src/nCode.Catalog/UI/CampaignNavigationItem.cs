using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents an Campaign in the Navigation Framework.
    /// </summary>
    public class CampaignNavigationItem : INavigationItem //, IMetadataContext
    {
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the Title for this Navigation Item.
        /// </summary>
        public string Title { get; set; }

        public string Permalink
        {
            get
            {
                if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                    return string.Format("/catalog/Item-List?Campaign={0}", ID.ToString());
                else
                    return string.Format("/{0}/catalog/Item-List?Campaign={1}", CultureInfo.CurrentUICulture.Name.ToLower(), ID.ToString());
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
        
        public bool IsActive
        {
            get
            {
                return Navigation.CurrentPath.Any(x => x.ID == ID);
            }
        }

        public INavigationItem GetParent()
        {
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

        /* Metadata 

        public T GetProperty<T>(string name, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(Brand.ObjectTypeId, ID, name, defaultValue);
        }

        public void SetProperty<T>(string name, T value)
        {
            GenericMetadataHelper.SetProperty<T>(Brand.ObjectTypeId, ID, name, value);
        }
        */
    }
}