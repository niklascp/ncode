using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using M = nCode.Catalog.Model;

namespace nCode.Catalog.Controllers
{
    public class ItemController : ApiController
    {
        public M.CatalogItem GetItemByItemNo(string id)
        {
            using (var catalogDbContext = new M.CatalogDbContext())
            {
                catalogDbContext.Configuration.ProxyCreationEnabled = false;

                var item = catalogDbContext.Items
                    .Include(x => x.Localizations)
                    .Include(x => x.Properties)
                    .Where(x => x.ItemNo == id)
                    .SingleOrDefault();
    
                return item;
            }
        }
    }
}
