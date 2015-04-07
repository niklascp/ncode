using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog
{
    public class CatalogRepository : nCode.Catalog.ICatalogRepository
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

        public IEnumerable<ItemListView> GetItemList(IFilterExpression<CatalogModel, Item> sourceFilter, IOrderByExpression<Item> sourceOrder, int skip = 0, int? take = null)
        {
            var items = dbModel.Items.AsQueryable();

            if (sourceFilter != null)
                items = sourceFilter.ApplyFilter(dbModel, items);

            items = items.Where(x => x.IsActive);

            if (sourceOrder != null)
                items = sourceOrder.ApplyOrdering(items);

            var viewData = from i in items
                           from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                           from g in i.Localizations.Where(x => x.Culture == null)
                           from cs in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.CurrentCurrency.Code && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                           from ds in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.DefaultCurrency.Code && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                           from c in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.CurrentCurrency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                           from d in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.DefaultCurrency.Code && x.PriceGroupCode == null)
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

        public void Dispose()
        {
            dbModel.Dispose();   
        }
    }
}
