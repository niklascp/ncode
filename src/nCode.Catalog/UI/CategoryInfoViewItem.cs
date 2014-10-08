/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace nCode.Catalog.UI
{
    [Obsolete("Please use the new Graph Navigation Framework.")]
    public class CategoryInfoViewItem : IHierarchyData
    {
        private IHierarchyData parent;
        private CategoryInfoNode item;
        private IHierarchicalEnumerable children;

        public CategoryInfoViewItem(IHierarchyData parent, CategoryInfoNode item)
        {
            this.parent = parent;
            this.item = item;
        }

        public IHierarchicalEnumerable GetChildren()
        {
            if (children == null)
                children = new CategoryInfoView(item.Children, this);

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
            get {
                throw new NotImplementedException(); 
            }
        }

        public string Type
        {
            get { throw new NotImplementedException(); }
        }
    }
}
