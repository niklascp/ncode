using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lucene.Net.Store;
using LU = Lucene.Net.Util;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using nCode.UI;
using Lucene.Net.Search;
using System.Web;

namespace nCode.Search
{
    public abstract class SearchEngine
    {
        public abstract IEnumerable<SearchResult> Search(string queryString);

        public abstract int GetEntryCount(SearchSource source);

        public abstract IIndexUpdateContext GetUpdateContext(SearchSource searchSource);
    }

    /// <summary>
    /// Represens a source of douments for the search engine. 
    /// </summary>
    public abstract class SearchSource
    {
        /// <summary>
        /// Gets a Globally Unique Id that identifies this Search Source.
        /// </summary>
        public abstract Guid SourceGuid { get; }

        /// <summary>
        /// Get a Index Update Context for this Search Source.
        /// </summary>
        public virtual IIndexUpdateContext GetUpdateContext()
        {
            return SearchHandler.Engine.GetUpdateContext(this);
        }

        public abstract void UpdateIndex();
    }

    public class SearchIndexEntry
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Keywords { get; set; }

        public string Url { get; set; }        

        public string Culture { get; set; }
    }
}
