
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

using nCode.Search;

using nCode.CMS.Model;

namespace nCode.CMS
{
    /// <summary>
    /// Represens a source of Catalog Items for the search engine. 
    /// </summary>
    public class ContentPageSearchSource : SearchSource
    {
        static readonly Guid sourceGuid = new Guid("58a49715-e07f-46e4-a80c-9a2f2aacb97d");

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
        /// Updates the index with all Content Pages.
        /// </summary>
        public override void UpdateIndex()
        {
            /* Search */
            using (var model = new CmsDbContext())
            using (var index = GetUpdateContext())
            {
                index.FlushIndex();

                var searchEntries = model.ContentPages.Where(x => x.ShowInMenu == true)
                    .SelectMany(x => model.ContentPartInstances.Where(y => y.RootContainerID == x.ID), (x,y) => new { ContentPage = x, ContentPartInstance = y })
                    .GroupBy(x => x.ContentPage, x => x.ContentPartInstance)
                    .ToList()
                    .Select(x => new SearchIndexEntry()
                    {
                        Id = x.Key.ID,
                        Title = x.Key.Title,
                        Description = x.Key.Description,
                        Content = string.Join(" ", x.Select(y => string.Join(" ", y.ContentPart.Properties.Select(z => z.Value)))),
                        Culture = x.Key.Culture,
                        Keywords = x.Key.Keywords,
                        Url = "/CMS/Content-View?ID=" + x.Key.ID.ToString()
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

        /// <summary>
        /// Deletes the Content Page, given by its ID, from the Search Index.
        /// </summary>
        public void DeleteItem(Guid pageID)
        {
            /* Search */
            using (var index = GetUpdateContext())
            {
                index.DeleteEntry(pageID);
                index.CommitChanges();
            }
        }

        /// <summary>
        /// Reindexes the Content Page, given by its ID, in the Search Index.
        /// </summary>
        public void ReindexPage(Guid pageID)
        {
            /* Search */
            using (var model = new CmsDbContext())
            using (var index = GetUpdateContext())
            {
                index.DeleteEntry(pageID);

                var searchEntries = model.ContentPages.Where(x => x.ShowInMenu == true && x.ID == pageID)
                    .SelectMany(x => model.ContentPartInstances.Where(y => y.RootContainerID == x.ID), (x,y) => new { ContentPage = x, ContentPartInstance = y })
                    .GroupBy(x => x.ContentPage, x => x.ContentPartInstance)
                    .ToList()
                    .Select(x => new SearchIndexEntry()
                    {
                        Id = x.Key.ID,
                        Title = x.Key.Title,
                        Description = x.Key.Description,
                        Content = string.Join(" ", x.Select(y => string.Join(" ", y.ContentPart.Properties.Select(z => z.Value)))),
                        Culture = x.Key.Culture,
                        Keywords = x.Key.Keywords,
                        Url = "/CMS/Content-View?ID=" + x.Key.ID.ToString()
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
