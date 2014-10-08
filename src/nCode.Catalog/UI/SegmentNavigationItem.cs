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
    public class SegmentNavigationItem : TreeNavigationItem //, IMetadataContext
    {
        public static SegmentNavigationItem GetFromID(Guid id)
        {
            using (var model = new CatalogModel())
            {
                return (from s in model.Segments
                        from g in s.Localizations.Where(x => x.Culture == null)
                        from l in s.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        where s.ID == id
                        select new SegmentNavigationItem
                        {
                            ID = s.ID,                          
                            IsVisible = s.IsActive,
                            Title = (l ?? g).Title
                        }).SingleOrDefault();
            }
        }        

        public override string Url
        {
            get
            {
                if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                    return string.Format("/catalog/Item-List?Segment={0}", ID.ToString());
                else
                    return string.Format("/{0}/catalog/Item-List?Segment={1}", CultureInfo.CurrentUICulture.Name.ToLower(), ID.ToString());
            }
        }        
    }
}