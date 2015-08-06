using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Search
{
    /// <summary>
    /// Represents an entry in a search index.
    /// </summary>
    public class SearchIndexEntry
    {
        private Dictionary<string, SearchEntryField> fields;

        /// <summary>
        /// Constricts a new entry with system fields defined.
        /// </summary>
        public SearchIndexEntry()
        {
            fields = new Dictionary<string, SearchEntryField>(StringComparer.OrdinalIgnoreCase);
            fields.Add("id", new SearchEntryField { Store = true, Analyse = false });
            fields.Add("title", new SearchEntryField { Store = true, Analyse = true });
        }

        /// <summary>
        /// Gets or sets the unique id for this entry.
        /// </summary>
        public Guid Id
        {
            get
            {
                return new Guid(fields["id"].Value);
            }
            set
            {
                fields["id"].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the title for this index entry.
        /// </summary>
        public string Title
        {
            get
            {
                return fields["title"].Value;
            }
            set
            {
                fields["title"].Value = value;
            }
        }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Keywords { get; set; }

        public string Url { get; set; }

        public string Culture { get; set; }

        public void AddCustomField(string name, string value, bool store = true, bool analyse = false)
        {
            fields.Add(name, new SearchEntryField
            {
                Value = value,
                Store = store,
                Analyse = analyse
            });
        }

        public IDictionary<string, SearchEntryField> Fields
        {
            get
            {
                return fields;
            }
        }
    }
}
