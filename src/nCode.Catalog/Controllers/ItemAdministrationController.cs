using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using nCode.Data;
using nCode.Data.Linq;
using nCode.Catalog.Data;
using nCode.Catalog.Models;
using nCode.Catalog.ViewModels;
using nCode.Imaging;

namespace nCode.Catalog.Controllers
{
    /// <summary>
    /// Api Controller from Item Administration.
    /// </summary>
    [RoutePrefix("admin/catalog/items")]
    public class ItemAdministrationController : ApiController
    {
        [Route("")]
        public IEnumerable<ItemListView> GetItemListByQuery([FromUri]string path)
        {
            using (var catalogRepository = new CatalogRepository())
            {
                IFilterExpression<CatalogModel, Item> filter = null;
                IOrderByExpression<Item> order = null;

                var parts = path.Split(new char[] { ':' }, 2);
                
                if (parts.Length < 2)
                    throw new ArgumentException("Invalid path.");

                if (string.Equals(parts[0], "C"))
                {
                    var categoryId = (!string.IsNullOrEmpty(parts[1])) ? new Guid(parts[1]) : (Guid?)null;

                    if (categoryId == Guid.Empty)
                        categoryId = null;

                    filter = new FilterExpression<CatalogModel, Item>(x => categoryId != null ? x.CategoryID == categoryId : x.CategoryID == null);
                    order = new OrderByExpression<Item, int>(x => x.Index);
                }
                else if (string.Equals(parts[0], "B"))
                {
                    var brandId = (!string.IsNullOrEmpty(parts[1])) ? new Guid(parts[1]) : (Guid?)null;

                    if (brandId == Guid.Empty)
                        brandId = null;

                    if (brandId != null)
                    {
                        filter = new FilterExpression<CatalogModel, Item>(x => brandId != null ? x.BrandID == brandId : x.BrandID == null);
                        order = new OrderByExpression<Item, int>(x => x.BrandIndex);
                    }
                }
                else if (string.Equals(parts[0], "S"))
                {
                    filter = new FilterExpression<CatalogModel, Item>(x => x.ItemNo.Contains(parts[1]));
                    order = new OrderByExpression<Item, string>(x => x.ItemNo);
                }
                else
                {
                    throw new ArgumentException("Unknown prefix '" + parts[0] + "'");
                }

                var data = catalogRepository.GetItemList(filter, order);

                foreach (var item in data)
                {
                    item.ImageFile = item.ImageFile != null ? ImageUtilities.EnsureImageSize(item.ImageFile, 100, 80) : null;
                }

                return data;
            }
        }
    }
}
