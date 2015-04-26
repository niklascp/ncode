using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nCode.Data.Linq;

namespace nCode.Catalog.Data
{
    public class CampaignItemFilter : IFilterExpression<CatalogModel, Item>
    {
        private Guid campaignId;

        public CampaignItemFilter(Guid campaignId)
        {
            this.campaignId = campaignId;
        }

        public IQueryable<Item> ApplyFilter(CatalogModel context, IQueryable<Item> query)
        {
            return (from i in query
                    from c in context.CampaignItems.Where(x => x.ItemID == i.ID && x.CampaignID == campaignId)
                    orderby c.DisplayIndex
                    select i);
        }
    }
}
