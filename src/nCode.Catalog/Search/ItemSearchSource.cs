using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Common.Logging;

using nCode.Search;

namespace nCode.Catalog.Search
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
            var log = LogManager.GetLogger<ItemSearchSource>();
            log.Info("Updating Catalog Item Search Index...");

            var count = 0;

            /* Initialize Search */
            using (var model = new CatalogModel())
            using (var index = GetUpdateContext())
            {
                index.FlushIndex();

                var entryData = (from i in model.Items
                                 from l in i.Localizations
                                 select new
                                 {
                                     i.ID,
                                     i.ItemNo,
                                     i.IsActive,
                                     l.Culture,
                                     l.Title,
                                     l.SeoDescription,
                                     Content = l.Description,
                                     Keywords = (i.Brand != null ? i.Brand.Name : "") + " " + l.SeoKeywords
                                 }).ToList();

                var htmlRemovalRegex = new Regex("<.*?>", RegexOptions.Compiled);

                foreach (var itemEntry in entryData)
                {
                    var en = new SearchIndexEntry() {
                        Id = itemEntry.ID,                              
                        Culture = itemEntry.Culture,
                        Title = itemEntry.Title,
                        Description =itemEntry.SeoDescription,
                        Keywords = itemEntry.Keywords,
                        Url = (itemEntry.Culture != null ? "/" + itemEntry.Culture : "") + "/Catalog/Item-View?ID=" + itemEntry.ID.ToString()
                    };

                    if (itemEntry.Content != null)
                        en.Content = HttpUtility.HtmlDecode(htmlRemovalRegex.Replace(itemEntry.Content, Environment.NewLine));

                    /* Get item variants */
                    var variants = (from ivt in model.ItemVariantTypes.Where(x => x.ItemID == en.Id)
                                    from iv in ivt.ItemVariants
                                    from g in iv.Variant.Localizations.Where(x => x.Culture == null)
                                    from l in iv.Variant.Localizations.Where(x => x.Culture == en.Culture).DefaultIfEmpty()
                                    select new
                                    {
                                        (l ?? g).DisplayName
                                    }).ToList();

                    /* Add each variant display name as a keyword. */
                    foreach (var variant in variants)
                        en.Keywords += " " + variant.DisplayName;

                    /* NCP: 2014-09-13: Add ItemNo to fields, allowing search by ItemNo. */
                    en.AddCustomField("itemno", itemEntry.ItemNo, store: true);
                    en.AddCustomField("isactive", itemEntry.IsActive ? "1" : "0", store: true);

                    index.IndexEntry(en);

                    count++;
                }

                index.CommitChanges();
            }

            log.InfoFormat("Successfully updated Catalog Item Search Index: {0} entires was indexed.", count);
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
            var count = 0;

            /* Search */
            using (var model = new CatalogModel())
            using (var index = GetUpdateContext())
            {
                index.DeleteEntry(ItemID);

                var entryData = (from i in model.Items
                                 from l in i.Localizations
                                 where i.ID == ItemID
                                 select new
                                 {
                                     i.ID,
                                     i.ItemNo,
                                     i.IsActive,
                                     l.Culture,
                                     l.Title,
                                     l.SeoDescription,
                                     Content = l.Description,
                                     Keywords = (i.Brand != null ? i.Brand.Name : "") + " " + l.SeoKeywords
                                 }).ToList();

                var htmlRemovalRegex = new Regex("<.*?>", RegexOptions.Compiled);

                foreach (var itemEntry in entryData)
                {
                    var en = new SearchIndexEntry()
                    {
                        Id = itemEntry.ID,
                        Culture = itemEntry.Culture,
                        Title = itemEntry.Title,
                        Description = itemEntry.SeoDescription,
                        Keywords = itemEntry.Keywords,
                        Url = (itemEntry.Culture != null ? "/" + itemEntry.Culture : "") + "/Catalog/Item-View?ID=" + itemEntry.ID.ToString()
                    };

                    if (itemEntry.Content != null)
                        en.Content = HttpUtility.HtmlDecode(htmlRemovalRegex.Replace(itemEntry.Content, Environment.NewLine));

                    /* Get item variants */
                    var variants = (from ivt in model.ItemVariantTypes.Where(x => x.ItemID == en.Id)
                                    from iv in ivt.ItemVariants
                                    from g in iv.Variant.Localizations.Where(x => x.Culture == null)
                                    from l in iv.Variant.Localizations.Where(x => x.Culture == en.Culture).DefaultIfEmpty()
                                    select new
                                    {
                                        (l ?? g).DisplayName
                                    }).ToList();

                    /* Add each variant display name as a keyword. */
                    foreach (var variant in variants)
                        en.Keywords += " " + variant.DisplayName;

                    /* NCP: 2014-09-13: Add ItemNo to fields, allowing search by ItemNo. */
                    en.AddCustomField("itemno", itemEntry.ItemNo, store: true);
                    en.AddCustomField("isactive", itemEntry.IsActive ? "1" : "0", store: true);

                    index.IndexEntry(en);

                    count++;
                }

                index.CommitChanges();
            }
        }
    }
}
