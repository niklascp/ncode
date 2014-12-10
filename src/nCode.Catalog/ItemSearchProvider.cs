/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.UI;
using System.Globalization;
using nCode.Catalog.UI;

namespace nCode.Catalog
{
    [Obsolete("Use nCode.Search Framework, with ItemSearchSource.")]
    public class ItemSearchProvider : ISearchProvider
    {
        public IEnumerable<SearchResult> Search(string searchQuery)
        {
            List<SearchResult> result = new List<SearchResult>();

            if (string.IsNullOrWhiteSpace(searchQuery))
                return result;

            using (var model = new CatalogModel())
            {
                var items = (from i in model.Items
                             from g in i.Localizations.Where(x => x.Culture == null)
                             from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                             let c = (l ?? g)
                             where i.IsActive && (c.Title.Contains(searchQuery) || c.Description.Contains(searchQuery) || c.SeoKeywords.Contains(searchQuery) || c.SeoDescription.Contains(searchQuery))
                             select new
                             {
                                 ItemNo = i.ItemNo,
                                 Title = c.Title
                             }).ToList();

                foreach (var i in items)
                    result.Add(new SearchResult
                    {
                        Title = i.Title,
                        Url = SeoUtilities.GetItemUrl(i.ItemNo, i.Title)
                    });

                return result;
            }
        }
    }
}
