using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents a the Brand Tree in the Navigation Framework.
    /// </summary>
    public class BrandNavigationTree : NavigationTree<nCode.Catalog.Models.Brand, BrandNavigationItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrandNavigationTree"/> class.
        /// </summary>
        /// <param name="sourceFilter">The source filter.</param>
        /// <param name="viewFilter">The view filter.</param>
        /// <param name="traverseFilter">The traverse filter.</param>
        /// <param name="root">The root.</param>
        public BrandNavigationTree(Expression<Func<Models.Brand, bool>> sourceFilter = null, Func<BrandNavigationItem, bool> viewFilter = null, Func<BrandNavigationItem, bool> traverseFilter = null, BrandNavigationItem root = null)
            : base(
                sourceFilter: sourceFilter ?? (x => x.IsVisible),
                viewFilter: viewFilter,
                traverseFilter: traverseFilter,
                root: root)
        { }

        protected override IQueryable<BrandNavigationItem> InitializeSource()
        {
            using (var catalogContext = new Models.CatalogDbContext())
            {
                return (from b in catalogContext.Brands.Where(SourceFilter)
                        //from l in b.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        //from g in b.Localizations.Where(x => x.Culture == null)
                        orderby b.ParentID, b.DisplayIndex
                        select new BrandNavigationItem()
                        {
                            ID = b.ID,
                            Title = b.Name,
                            ParentID = b.ParentID,
                            IsVisible = b.IsVisible,
                            Depth = -1,
                            Image1 = b.Image1,
                            Image2 = b.Image2,
                            Image3 = b.Image3
                        }).ToList().AsQueryable();
            }
        }
    }
}