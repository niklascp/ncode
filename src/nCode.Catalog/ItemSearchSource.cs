using nCode.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace nCode.Catalog
{
    /// <summary>
    /// Represens a source of Catalog Items for the search engine. 
    /// </summary>
    public class ItemSearchSource : SearchSource
    {
        static readonly Guid sourceGuid = new Guid("d69edc3f-b8cd-4ec2-b40d-018ac0eecaa3");

        /// <summary>
        /// Gets a Globally Unique Id that identifies this Search Source.
        /// </summary>
        public override Guid SourceGuid
        {
            get
            {
                return sourceGuid;
            }
        }

        /// <summary>
        /// Updates the index with all active items in the Catelog.
        /// </summary>
        public override void UpdateIndex()
        {
            /* Initialize Search */
            using (var model = new CatalogModel())
            using (var index = GetUpdateContext())
            {
                index.FlushIndex();

                var searchEntries = (from i in model.Items
                                     from l in i.Localizations
                                     where i.IsActive
                                     select new SearchIndexEntry()
                                     {
                                         Id = i.ID,
                                         Title = l.Title,
                                         Description = l.SeoDescription,
                                         Content = l.Description,
                                         Culture = l.Culture,
                                         Keywords = l.SeoKeywords,
                                         Url = (l.Culture != null ? "/" + l.Culture : "") + "/Catalog/Item-View?ID=" + i.ID.ToString()
                                     }).ToList();

                var htmlRemovalRegex = new Regex("<.*?>", RegexOptions.Compiled);

                foreach (var en in searchEntries)
                {
                    if (en.Content != null)
                        en.Content = HttpUtility.HtmlDecode(htmlRemovalRegex.Replace(en.Content, Environment.NewLine));

                    index.IndexEntry(en);
                }

                index.CommitChanges();
            }

            Log.WriteEntry(EntryType.Information, "Catalog", "Update Item Search Index");
        }

        /// <summary>
        /// Deletes the Catelog Item, given by its ID, from the Search Index.
        /// </summary>
        public void DeleteItem(Guid ItemID)
        {
            /* Search */
            using (var index = GetUpdateContext())
            {
                index.DeleteEntry(ItemID);
                index.CommitChanges();
            }
        }

        /// <summary>
        /// Reindexes the Catelog Item, given by its ID, from the Search Index.
        /// </summary>
        public void ReindexItem(Guid ItemID)
        {
            /* Search */
            using (var model = new CatalogModel())
            using (var index = GetUpdateContext())
            {
                index.DeleteEntry(ItemID);

                var searchEntries = (from i in model.Items
                                     from l in i.Localizations
                                     where i.ID == ItemID && i.IsActive
                                     select new SearchIndexEntry()
                                     {
                                         Id = i.ID,
                                         Title = l.Title,
                                         Description = l.SeoDescription,
                                         Content = l.Description,
                                         Culture = l.Culture,
                                         /* NCP: 2014-09-13: Add ItemNo to Keyword, allowing search by ItemNo. */
                                         Keywords = i.ItemNo + " " + l.SeoKeywords,
                                         Url = (l.Culture != null ? "/" + l.Culture : "") + "/Catalog/Item-View?ID=" + i.ID.ToString()
                                     }).ToList();

                var htmlRemovalRegex = new Regex("<.*?>", RegexOptions.Compiled);

                foreach (var en in searchEntries)
                {
                    if (en.Content != null)
                        en.Content = HttpUtility.HtmlDecode(htmlRemovalRegex.Replace(en.Content, Environment.NewLine));
                    index.IndexEntry(en);
                }

                index.CommitChanges();
            }
        }
    }
}
