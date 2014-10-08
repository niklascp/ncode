/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;

namespace nCode.Catalog.UI
{
    [Obsolete("Please use the new Graph Navigation Framework.")]
    public class CategoryInfoView : IHierarchicalEnumerable
    {
        IList<CategoryInfoViewItem> list;

        internal CategoryInfoView(IList<CategoryInfoNode> categories, CategoryInfoViewItem parent)
        {
            list = new List<CategoryInfoViewItem>();

            foreach (CategoryInfoNode category in categories)
            {
                list.Add(new CategoryInfoViewItem(parent, category));
            }
        }

        public CategoryInfoView(IList<CategoryInfoNode> categories)
            : this(categories, null)
        { }

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
