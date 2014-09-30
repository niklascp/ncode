using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Search
{
    /// <summary>
    /// Represents an Index Update Context.
    /// </summary>
    public interface IIndexUpdateContext : IDisposable
    {
        /// <summary>
        /// Deletes all enties in this index.
        /// </summary>
        /// <param name="entryId"></param>
        void FlushIndex();

        /// <summary>
        /// Deletes occuerences of the given entry.
        /// </summary>
        /// <param name="entryId"></param>
        void DeleteEntry(Guid entryId);

        /// <summary>
        /// Add or updates the given Search Index Entry.
        /// </summary>
        void IndexEntry(SearchIndexEntry searchEntry);

        /// <summary>
        /// Commit the chagnes to the Search Index.
        /// </summary>
        void CommitChanges();
    }

}
