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
    /// Represents a the Category Tree in the Navigation Framework.
    /// </summary>
    public class CategoryNavigationTree : NavigationTree<Category, CategoryNavigationItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryNavigationTree"/> class.
        /// </summary>
        /// <param name="sourceFilter">The source filter.</param>
        /// <param name="viewFilter">The view filter.</param>
        /// <param name="traverseFilter">The traverse filter.</param>
        public CategoryNavigationTree(Expression<Func<Category, bool>> sourceFilter = null, Func<CategoryNavigationItem, bool> viewFilter = null, Func<CategoryNavigationItem, bool> traverseFilter = null)
            : base(
                sourceFilter: sourceFilter ?? (x => x.IsVisible),
                viewFilter: viewFilter,
                traverseFilter: traverseFilter)
        { }

        protected override IQueryable<CategoryNavigationItem> InitializeSource()
        {
            using (var catalogModel = new CatalogModel())
            {
                return (from c in catalogModel.Categories.Where(SourceFilter)
                        from l in c.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from g in c.Localizations.Where(x => x.Culture == null)
                        orderby c.ParentID, c.Index
                        select new CategoryNavigationItem()
                        {
                            ID = c.ID,
                            CategoryNo = c.CategoryNo,
                            Title = (l ?? g).Title,
                            IsVisible = c.IsVisible,
                            ParentID = c.ParentID,
                            Image1 = c.Image1,
                            Image2 = c.Image2,
                            Image3 = c.Image3
                        }).ToList().AsQueryable();
            }
        }
    }
}