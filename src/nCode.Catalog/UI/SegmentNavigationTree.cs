using nCode.Metadata;
using nCode.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents a the Brand Tree in the Navigation Framework.
    /// </summary>
    public class SegmentNavigationTree : NavigationTree<Segment, SegmentNavigationItem>
    {
        public SegmentNavigationTree(Expression<Func<Segment, bool>> sourceFilter = null, Func<SegmentNavigationItem, bool> viewFilter = null, Func<SegmentNavigationItem, bool> traverseFilter = null)
            : base(sourceFilter, viewFilter, traverseFilter)
        { }

        protected override IQueryable<SegmentNavigationItem> InitializeSource()
        {
            using (var catalogModel = new CatalogModel())
            {
                return (from s in catalogModel.Segments.Where(SourceFilter)
                        from l in s.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from g in s.Localizations.Where(x => x.Culture == null)
                        orderby s.DisplayIndex
                        select new SegmentNavigationItem()
                        {
                            ID = s.ID,
                            Title = (l ?? g).Title,
                            IsVisible = s.IsActive
                        }).ToList().AsQueryable();
            }
        }
    }
}