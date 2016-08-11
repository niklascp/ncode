using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;

namespace nCode.UI
{
    public class NavigationView : IHierarchicalEnumerable
    {
        public static NavigationView Union(NavigationView a, NavigationView b)
        {
            var v = new NavigationView(null, null, null);
            
            foreach (var i in a.list)
                v.list.Add(i);
            
            foreach (var i in b.list)
                v.list.Add(i);

            return v;
        }

        IList<NavigationViewItem> list;

        public NavigationView(IEnumerable<INavigationItem> items, INavigationGraph relation, NavigationViewItem parent)
        {
            list = new List<NavigationViewItem>();

            if (items != null)
            {
                foreach (var item in items)
                {
                    list.Add(new NavigationViewItem(parent, relation, item));
                }
            }
        }

        public NavigationView(INavigationGraph graph)
            : this(graph.Roots, graph, null)
        { }

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void AddChild(NavigationViewItem child)
        {
            list.Add(child);
        }

        public int Count { get { return list.Count; } }
    }
}
