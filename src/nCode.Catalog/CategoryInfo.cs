/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

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

namespace nCode.Catalog
{
    public class CategoryInfo : IMetadataContext
    {
        private Guid id;

        public CategoryInfo()
        {

        }

        public CategoryInfo(Guid id)
        {            
            this.id = id;
        }

        public Guid ID
        {
            get { return id; }
        }

        public int CategoryNo { get; set; }

        public string Title { get; set; }

        public bool IsVisible { get; set; }

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

        public string Url
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

        public string FullUrl
        {
            get
            {
                return Settings.Url + Url;
            }
        }

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

        /* Metadata */

        public T GetProperty<T>(string name, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(Category.ObjectTypeId, ID, name, defaultValue);
        }

        public void SetProperty<T>(string name, T value)
        {
            GenericMetadataHelper.SetProperty<T>(Category.ObjectTypeId, ID, name, value);
        }
    }

    public class CategoryInfoNode : CategoryInfo
    {
        private CategoryInfoNode parent;
        private List<CategoryInfoNode> children;

        public CategoryInfoNode(Guid id) : base(id)
        {
            children = new List<CategoryInfoNode>();
        }

        public int Depth { get; set; }

        public CategoryInfoNode Parent
        {
            get { return parent; }
        }

        public IList<CategoryInfoNode> Children
        {
            get { return children.AsReadOnly(); }
        }

        public bool HasChildren
        {
            get { return children.Any(); }
        }

        public void AddChild(CategoryInfoNode child)
        {
            if (child.parent != null)
                child.parent.children.Remove(child);
            child.parent = this;
            children.Add(child);
        }
    }
}