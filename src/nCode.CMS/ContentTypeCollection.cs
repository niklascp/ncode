using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace nCode.CMS
{
    public class ContentTypeCollection : IEnumerable, IEnumerable<ContentType>
    {
        private ContentTypeGroup ungrouped;
        private IList<ContentTypeGroup> groups;
        internal IDictionary<Guid, ContentType> contentTypesByID;

        public ContentTypeCollection()
        {
            ungrouped = new ContentTypeGroup(this);
            groups = new List<ContentTypeGroup>();
            contentTypesByID = new Dictionary<Guid, ContentType>();
        }

        public IList<ContentTypeGroup> Groups
        {
            get { return groups; }
        }

        public void Add(ContentType contentType)
        {
            ungrouped.Add(contentType);
        }

        public ContentType this[int index]
        {
            get
            {
                return ungrouped[index];
            }
        }

        public ContentType this[Guid id]
        {
            get
            {
                if (contentTypesByID.ContainsKey(id))
                    return (ContentType)contentTypesByID[id];
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ungrouped.GetEnumerator();
        }

        internal void Replace(Guid id, ContentType contentType)
        {
            contentTypesByID[id] = contentType;

            {
                var ct = ungrouped.SingleOrDefault(x => x.ID == id);
                if (ct != null)
                {
                    ungrouped.Insert(ungrouped.IndexOf(ct), contentType);
                    ungrouped.Remove(ct);
                }
            }

            foreach (var g in groups)
            {
                var ct = g.SingleOrDefault(x => x.ID == id);
                if (ct != null)
                {
                    g.Insert(ungrouped.IndexOf(ct), contentType);
                    g.Remove(ct);
                }
            }
        }

        IEnumerator<ContentType> IEnumerable<ContentType>.GetEnumerator()
        {
            return ungrouped.GetEnumerator();
        }
    }
}