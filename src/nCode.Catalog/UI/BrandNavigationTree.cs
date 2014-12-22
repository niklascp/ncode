using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;
using System.Collections.Generic;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents a the Brand Tree in the Navigation Framework.
    /// </summary>
    public class BrandNavigationTree : INavigationGraph
    {
        private List<BrandNavigationItem> list;

        /// <summary>
        /// Gets the source filter.
        /// </summary>
        public Func<Brand, bool> SourceFilter { get; private set; }

        /// <summary>
        /// Gets the view filter.
        /// </summary>
        public Func<BrandNavigationItem, bool> ViewFilter { get; private set; }

        /// <summary>
        /// Gets the traverse filter.
        /// </summary>
        public Func<BrandNavigationItem, bool> TraverseFilter { get; private set; }

        /// <summary>
        /// Gets the roots of the tree.
        /// </summary>
        public IEnumerable<INavigationItem> Roots { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandNavigationTree"/> class.
        /// </summary>
        /// <param name="sourceFilter">The source filter.</param>
        /// <param name="viewFilter">The view filter.</param>
        /// <param name="traverseFilter">The traverse filter.</param>
        /// <param name="root">The root.</param> 
        public BrandNavigationTree(Func<Brand, bool> sourceFilter = null, Func<BrandNavigationItem, bool> viewFilter = null, Func<BrandNavigationItem, bool> traverseFilter = null, BrandNavigationItem root = null)
        {
            SourceFilter = sourceFilter ?? (x => true); 
            ViewFilter = viewFilter;
            TraverseFilter = traverseFilter;

            using (var catalogModel = new CatalogModel())
            {
                list = (from b in catalogModel.Brands.Where(SourceFilter)
                        //from l in b.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        //from g in b.Localizations.Where(x => x.Culture == null)
                        orderby b.ParentID, b.Index
                        select new BrandNavigationItem()
                        {
                            ID = b.ID,
                            Title = b.Name,
                            ParentID = b.ParentID,
                            Depth = -1,
                            Image1 = b.Image1,
                            Image2 = b.Image2,
                            Image3 = b.Image3
                        }).ToList();
            }

            Roots = Expand(root).Cast<BrandNavigationItem>();
        }

        /// <summary>
        /// Expands the specified item.
        /// </summary>
        public IEnumerable<INavigationItem> Expand(INavigationItem item)
        {
            if (item != null && !(item is BrandNavigationItem))
                throw new NotSupportedException("BrandNavigationTree can only expand items of type BrandNavigationItem.");

            if (item == null || TraverseFilter == null || TraverseFilter((BrandNavigationItem)item))
            {
                var parentId = item != null ? item.ID : (Guid?)null;

                var children = list.Where(c => parentId != null ? c.ParentID == parentId : c.ParentID == null);

                foreach (var child in children) {
                    child.Depth = item != null ? ((BrandNavigationItem)item).Depth + 1 : 0;
                }

                if (ViewFilter != null)
                    children = children.Where(ViewFilter).ToList();

                return children;
            }

            return null;
        }
    }
}