/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace nCode.CMS.UI
{
    [Obsolete("Please use the new Graph Navigation Framework.")]
    public class ContentPageNodeViewItem : IHierarchyData
    {
        private IHierarchyData parent;
        private ContentPageNode item;
        private IHierarchicalEnumerable children;

        public ContentPageNodeViewItem(IHierarchyData parent, ContentPageNode item)
        {
            this.parent = parent;
            this.item = item;
        }

        public IHierarchicalEnumerable GetChildren()
        {
            if (children == null)
                children = new ContentPageNodeView(item.Children, this);

            return children;
        }

        public IHierarchyData GetParent()
        {
            return parent;
        }

        public bool HasChildren
        {
            get { return item.Children.Count > 0; }
        }

        public object Item
        {
            get { return item; }
        }

        public string Path
        {
            get { return item.Url; }
        }

        public string Type
        {
            get { throw new NotImplementedException(); }
        }
    }
}
