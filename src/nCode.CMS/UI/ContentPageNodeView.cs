/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;

namespace nCode.CMS.UI
{
    [Obsolete("Please use the new Graph Navigation Framework.")]
    public class ContentPageNodeView : IHierarchicalEnumerable, IEnumerable<ContentPageNodeViewItem>
    {
        IList<ContentPageNodeViewItem> list;

        internal ContentPageNodeView(IList<ContentPageNode> contentPages, ContentPageNodeViewItem parent)
        {
            list = new List<ContentPageNodeViewItem>();

            foreach (var contentPage in contentPages)
            {
                list.Add(new ContentPageNodeViewItem(parent, contentPage));
            }
        }

        public ContentPageNodeView(IList<ContentPageNode> contentPages)
            : this(contentPages, null)
        { }

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator<ContentPageNodeViewItem> IEnumerable<ContentPageNodeViewItem>.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
