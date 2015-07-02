using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace nCode.UI
{
    /// <summary>
    /// Represents a node i the navigation graph.
    /// </summary>
    public class NavigationViewItem : IHierarchyData, INavigationItem
    {
        private NavigationViewItem parent;
        private INavigationGraph graph;
        private NavigationView children;
        private INavigationItem item;

        /// <summary>
        /// Ensures that the associated INavigationGraph is expanded for the current node.
        /// </summary>
        private void EnsureChildren()
        {
            if (children == null)
            {
                children = new NavigationView(graph.Expand(item), graph, this);
            }
        }

        public NavigationViewItem(NavigationViewItem parent, INavigationGraph graph, INavigationItem item)
        {
            this.parent = parent;
            this.graph = graph;
            this.item = item;
        }

        /// <summary>
        /// Gets an enumeration object that represents all the child nodes of the current hierarchical node.
        /// </summary>
        public IHierarchicalEnumerable GetChildren()
        {
            EnsureChildren();
            return children;
        }

        /// <summary>
        /// Gets an IHierarchyData object that represents the parent node of the current hierarchical node.
        /// </summary>
        public IHierarchyData GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Gets a valie that indicates wheather this node has decendents.
        /// </summary>
        public bool HasChildren
        {
            get {
                EnsureChildren();

                if (children != null)
                    return children.Count > 0;

                return false;
            }
        }

        /// <summary>
        /// Gets the hierarchical data node that the object represents.
        /// </summary>
        public object Item
        {
            get { return item; }
        }

        /// <summary>
        /// Gets the hierarchical path of the node.
        /// </summary>
        public string Path
        {
            get
            {
                return item.Url;
            }
        }

        /// <summary>
        /// Gets a string that identifies the type of the node.
        /// </summary>
        public string Type
        {
            get
            {
                return item.GetType().ToString();
            }
        }

        /// <summary>
        /// Gets the unique identifier of the Navigation Item.
        /// </summary>
        public Guid ID
        {
            get { return item.ID; }
        }

        /// <summary>
        /// Gets the title of the Navigation Item.
        /// </summary>
        public string Title
        {
            get { return item.Title; }
        }

        /// <summary>
        /// Gets a possible frindly URL that can be used for navigation to the Navigation Item.
        /// </summary>
        public string Url
        {
            get { return item.Url; }
        }

        /// <summary>
        /// Gets a possibly context-specific logical Parent of the Navigation Item in the Navigation Graph. This is used to automatically calculate the breadcrumb path available in <see cref="Navigation.CurrentPath"/>.
        /// </summary>
        INavigationItem INavigationItem.GetParent()
        {
            return parent;
        }
    }
}
