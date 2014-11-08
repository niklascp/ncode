using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;

namespace nCode.UI
{
    /// <summary>
    /// Represents a the Brand Tree in the Navigation Framework.
    /// </summary>
    public abstract class NavigationTree<E, V> : INavigationGraph, IEnumerable<INavigationItem> where V : TreeNavigationItem
    {
        private Lazy<IEnumerable<INavigationItem>> roots;
        private IQueryable<V> items { get; set; }

        /// <summary>
        /// Gets the source filter.
        /// </summary>
        public Expression<Func<E, bool>> SourceFilter { get; protected set; }

        /// <summary>
        /// Gets the view filter.
        /// </summary>
        public Func<V, bool> ViewFilter { get; protected set; }

        /// <summary>
        /// Gets the traverse filter.
        /// </summary>
        public Func<V, bool> TraverseFilter { get; protected set; }

        /// <summary>
        /// Gets the roots of the tree.
        /// </summary>
        public IEnumerable<INavigationItem> Roots { get { return roots.Value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTree"/> class.
        /// </summary>
        /// <param name="sourceFilter">The source filter.</param>
        /// <param name="viewFilter">The view filter.</param>
        /// <param name="traverseFilter">The traverse filter.</param>
        /// <param name="root">The initial root of the tree, or null the true root.</param> 
        public NavigationTree(Expression<Func<E, bool>> sourceFilter = null, Func<V, bool> viewFilter = null, Func<V, bool> traverseFilter = null, INavigationItem root = null)
        {
            SourceFilter = sourceFilter ?? (x => true); 
            ViewFilter = viewFilter;
            TraverseFilter = traverseFilter;

            items = InitializeSource();

            roots = new Lazy<IEnumerable<INavigationItem>>(() => Expand(root));
        }

        /// <summary>
        /// Initializes the source.
        /// </summary>
        /// <returns></returns>
        protected abstract IQueryable<V> InitializeSource();

        /// <summary>
        /// Expands the specified item.
        /// </summary>
        public IEnumerable<INavigationItem> Expand(INavigationItem item)
        {
            if (item != null && !(item is V))
                throw new NotSupportedException(string.Format("{0} can only expand items of type {1}.", this.GetType(), typeof(V)));

            if (item == null || TraverseFilter == null || TraverseFilter((V)item))
            {
                var parentId = item != null ? item.ID : (Guid?)null;

                var children = items.Where(c => parentId != null ? c.ParentID == parentId : c.ParentID == null);

                foreach (var child in children) {
                    child.Depth = item != null ? ((TreeNavigationItem)item).Depth + 1 : 0;
                    child.HasChildren = items.Where(x => x.ParentID == child.ID).Any();
                }

                if (ViewFilter != null)
                    children = children.Where(ViewFilter).ToList().AsQueryable();

                return children;
            }

            return null;
        }

        public IEnumerator<INavigationItem> GetEnumerator()
        {
            return Roots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Roots.GetEnumerator();
        }
    }
}