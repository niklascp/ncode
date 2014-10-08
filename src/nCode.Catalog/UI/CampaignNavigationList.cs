using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;
using System.Collections.Generic;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents a the Campaings List in the Navigation Framework.
    /// </summary>
    public class CampaignNavigationList : INavigationGraph
    {
        public bool ExcludeEmpty { get; set; }

        public Func<Campaign, bool> SourceFilter { get; private set; }

        public Func<CampaignNavigationItem, bool> ViewFilter { get; private set; }

        public IEnumerable<INavigationItem> Roots { get; private set; }

        public CampaignNavigationList(Func<Campaign, bool> sourceFilter = null, Func<CampaignNavigationItem, bool> viewFilter = null)
        {
            SourceFilter = sourceFilter ?? (x => x.IsVisible);
            ViewFilter = viewFilter;
            
            Roots = Expand(null).Cast<CampaignNavigationItem>();
        }

        public IEnumerable<INavigationItem> Expand(INavigationItem item)
        {
            if (item == null)
            {
                using (var catalogModel = new CatalogModel())
                {
                    var list = (from c in catalogModel.Campaigns.Where(SourceFilter)
                                from l in c.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                                from g in c.Localizations.Where(x => x.Culture == null)
                                orderby c.DisplayIndex
                                select new CampaignNavigationItem()
                                {
                                    ID = c.ID,
                                    Title = (l ?? g).Title                                    
                                }).ToList();

                    if (ViewFilter != null)
                        list = list.Where(ViewFilter).ToList();

                    return list;
                }
            }

            return null;
        }
    }
}