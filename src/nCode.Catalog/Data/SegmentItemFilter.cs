using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nCode.Data.Linq;

namespace nCode.Catalog.Data
{
    public class SegmentItemFilter : IFilterExpression<CatalogModel, Item>
    {
        private Guid segmentId;

        public SegmentItemFilter(Guid segmentId)
        {
            this.segmentId = segmentId;
        }

        public IQueryable<Item> ApplyFilter(CatalogModel context, IQueryable<Item> query)
        {
            return (from i in query
                    from s in context.SegmentItems.Where(x => x.ItemID == i.ID && x.SegmentID == segmentId)
                    orderby s.DisplayIndex
                    select i);
        }
    }
}
