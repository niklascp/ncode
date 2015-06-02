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
using System.Net.Http;

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
                IEnumerable<ItemListView> data = null;

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

                    data = catalogRepository.GetItemList(filter, order);
                }
                else if (string.Equals(parts[0], "B"))
                {
                    var brandId = (!string.IsNullOrEmpty(parts[1])) ? new Guid(parts[1]) : (Guid?)null;

                    if (brandId == Guid.Empty)
                        brandId = null;

                    filter = new FilterExpression<CatalogModel, Item>(x => brandId != null ? x.BrandID == brandId : x.BrandID == null);
                    order = new OrderByExpression<Item, int>(x => x.BrandIndex);

                    data = catalogRepository.GetItemList(filter, order);
                }
                else if (string.Equals(parts[0], "S"))
                {
                    data = catalogRepository.SearchItems(parts[1], includeInActive: true);
                }
                else
                {
                    throw new ArgumentException("Unknown prefix '" + parts[0] + "'");
                }

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.ImageFile = item.ImageFile != null ? ImageUtilities.EnsureImageSize(item.ImageFile, 100, 80) : null;
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// Returns a list of related items given an item id.
        /// </summary>
        [Route("related")]
        [HttpGet]
        public IEnumerable<ItemListView> ListRelatedItems([FromUri]Guid itemID)
        {
            using (var catalogRepository = new CatalogRepository())
            {
                var viewData = catalogRepository.ListRelatedItems(itemID);

                foreach (var itemView in viewData)
                {
                    itemView.ImageFile = itemView.ImageFile != null ? ImageUtilities.EnsureImageSize(itemView.ImageFile, 100, 80) : null;
                }

                return viewData;
            }
        }

        /// <summary>
        /// Updates the related items of the given item id with the given list of related item ids.
        /// </summary>
        [Route("related")]
        [HttpPost]
        public IHttpActionResult UpdateRelatedItems([FromUri]Guid itemID, [FromBody]Guid[] relatedItemIDs)
        {
            using (var model = new CatalogModel())
            {
                model.ItemRelations.DeleteAllOnSubmit(model.ItemRelations.Where(r => r.ItemID == itemID));

                int displayIndex = 0;
                foreach (Guid relatedItemID in relatedItemIDs)
                {
                    ItemRelation itemRelation = new ItemRelation();
                    itemRelation.ID = Guid.NewGuid();
                    itemRelation.ItemID = itemID;
                    itemRelation.DisplayIndex = displayIndex;
                    itemRelation.RelatedItemID = relatedItemID;

                    model.ItemRelations.InsertOnSubmit(itemRelation);

                    displayIndex++;
                }

                model.SubmitChanges();
            }

            return Ok();
        }
    }
}
