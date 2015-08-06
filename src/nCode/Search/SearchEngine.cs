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
    /// <summary>
    /// Represens a abstract search engine.
    /// </summary>
    public abstract class SearchEngine
    {
        /// <summary>
        /// Searches the search engine for the given query string.
        /// </summary>
        public abstract IEnumerable<SearchResult> Search(string queryString, string[] includeFields = null);

        /// <summary>
        /// Get entry count in the index for the given search source.
        /// </summary>
        public abstract int GetEntryCount(SearchSource source);

        /// <summary>
        /// Gets an update context for this search engine and the given search source.
        /// </summary>
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

        /// <summary>
        /// Updates the entire index for this Search Source.
        /// </summary>
        public abstract void UpdateIndex();
    }
}
