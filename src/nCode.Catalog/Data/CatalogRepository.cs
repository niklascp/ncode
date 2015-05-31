using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using nCode.Catalog.UI;

namespace nCode.Catalog.Data
{
    public class CatalogRepository : nCode.Catalog.Data.ICatalogRepository
    {
        public static IOrderByExpression<Item> ItemCategoryOrder
        {
            get { return new OrderByExpression<Item, int>(x => x.Index); }
        }

        public static IOrderByExpression<Item> ItemBrandOrder
        {
            get { return new OrderByExpression<Item, int>(x => x.BrandIndex); }
        }

        string priceGroup;
        CatalogModel dbModel;

        public CatalogRepository()
        {
            priceGroup = "TILBUD";
            dbModel = new CatalogModel();
        }

        public CategoryView GetCategory(Guid categoryId)
        {
            var categoryView = (from c in dbModel.Categories
                                from l in c.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                                from g in c.Localizations.Where(x => x.Culture == null)
                                where c.ID == categoryId
                                select new CategoryView
                                {
                                    ID = c.ID,
                                    Title = (l ?? g).Title,
                                    Description = (l ?? g).Description,
                                    SeoDescription = (l ?? g).SeoDescription,
                                    SeoKeywords = (l ?? g).SeoKeywords
                                }).SingleOrDefault();

            return categoryView;
        }

        public BrandView GetBrand(Guid brandId)
        {
            var brandView = (from b in dbModel.Brands
                             from l in b.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                             from g in b.Localizations.Where(x => x.Culture == null)
                             where b.ID == brandId
                             select new BrandView
                             {
                                 ID = b.ID,
                                 Title = b.Name,
                                 Description = (l ?? g).Description,
                                 SeoDescription = (l ?? g).SeoDescription,
                                 SeoKeywords = (l ?? g).SeoKeywords,
                                 LogoImageFile = b.Image1
                             }).SingleOrDefault();

            return brandView;
        }

        public IEnumerable<ItemListView> GetItemList(IFilterExpression<CatalogModel, Item> filter, IOrderByExpression<Item> order = null, int skip = 0, int? take = null)
        {
            var items = dbModel.Items.AsQueryable();

            if (filter != null)
                items = filter.ApplyFilter(dbModel, items);

            items = items.Where(x => x.IsActive);

            if (order != null)
                items = order.ApplyOrdering(items);

            var currentCurrencyCode = CurrencyController.CurrentCurrency != null ? CurrencyController.CurrentCurrency.Code : null;
            var defaultCurrencyCode = CurrencyController.DefaultCurrency != null ? CurrencyController.DefaultCurrency.Code : null;

            var viewData = from i in items
                           from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                           from g in i.Localizations.Where(x => x.Culture == null)
                           from cs in i.ListPrices.Where(x => x.CurrencyCode == currentCurrencyCode && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                           from ds in i.ListPrices.Where(x => x.CurrencyCode == defaultCurrencyCode && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                           from c in i.ListPrices.Where(x => x.CurrencyCode == currentCurrencyCode && x.PriceGroupCode == null).DefaultIfEmpty()
                           from d in i.ListPrices.Where(x => x.CurrencyCode == defaultCurrencyCode && x.PriceGroupCode == null).DefaultIfEmpty()
                           select new ItemListView
                           {
                               ID = i.ID,
                               ItemNo = i.ItemNo,
                               Title = (l ?? g).Title,
                               ListPrice = ds != null ? (cs ?? ds) : (c ?? d),
                               DefaultListPrice = (c ?? d),
                               OnSale = i.OnSale,
                               IsAvailable = i.IsAvailable,
                               VariantMode = i.VariantMode,

                               CategoryID = i.CategoryID,
                               //CategoryTitle = 
                               CategoryIndex = i.Index,

                               BrandID = i.BrandID,
                               BrandName = (i.Brand != null ? i.Brand.Name : null),
                               BrandIndex = i.BrandIndex,

                               ImageFile = (from img in i.Images orderby img.DisplayIndex select img.ImageFile).FirstOrDefault()
                           };

            return viewData.ToList();
        }

        public ItemDetailView GetItemDetail(string itemNo)
        {
            var viewData = from i in dbModel.Items.Where(x => x.ItemNo == itemNo)
                           from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                           from g in i.Localizations.Where(x => x.Culture == null)
                           select new ItemDetailView
                           {
                               ID = i.ID,
                               ItemNo = i.ItemNo,
                               Title = (l ?? g).Title,
                               Description = (l ?? g).Description,

                               SeoKeywords = (l ?? g).SeoKeywords,
                               SeoDescription = (l ?? g).SeoDescription,

                               OnSale = i.OnSale,
                               IsAvailable = i.IsAvailable,
                               VariantMode = i.VariantMode,

                               CategoryID = i.CategoryID,
                               //CategoryTitle = 
                               CategoryIndex = i.Index,

                               BrandID = i.BrandID,
                               BrandName = (i.Brand != null ? i.Brand.Name : null),
                               BrandIndex = i.BrandIndex,
                           };

            return viewData.SingleOrDefault();
        }

        public ItemDetailView GetItemDetail(ItemViewRequest itemViewRequest)
        {
            return GetItemDetail(itemViewRequest.ItemNo);
        }

        public IEnumerable<ItemListView> ListRelatedItems(Guid itemID)
        {
            var viewData = from i in dbModel.Items
                           from g in dbModel.ItemLocalizations.Where(x => x.ItemID == i.ID && x.Culture == null)
                           from l in dbModel.ItemLocalizations.Where(x => x.ItemID == i.ID && x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                           from ct in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.CurrentCurrency.Code && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                           from dt in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.DefaultCurrency.Code && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                           from c in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.CurrentCurrency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                           from d in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.DefaultCurrency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                           where i.IsActive && dbModel.ItemRelations.Any(x => x.ItemID == itemID && x.RelatedItemID == i.ID)
                           select new ItemListView
                           {
                               ID = i.ID,
                               ItemNo = i.ItemNo,
                               Title = (l ?? g).Title,
                               ListPrice = dt != null ? (ct ?? dt) : (c ?? d),
                               DefaultListPrice = (c ?? d),
                               OnSale = i.OnSale,
                               IsAvailable = i.IsAvailable,
                               VariantMode = i.VariantMode,
                               BrandName = (i.Brand != null ? i.Brand.Name : null),
                               ImageFile = (from img in i.Images orderby img.DisplayIndex select img.ImageFile).FirstOrDefault()
                           };

            return viewData.ToList();
        }

        public void Dispose()
        {
            dbModel.Dispose();
        }
    }
}
