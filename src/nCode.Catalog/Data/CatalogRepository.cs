using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using nCode.Catalog.UI;
using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using nCode.Search;

namespace nCode.Catalog.Data
{
    public class CatalogRepository : ICatalogRepository
    {
        public static IOrderByExpression<Item> ItemCategoryOrder
        {
            get { return new OrderByExpression<Item, int>(x => x.Index); }
        }

        public static IOrderByExpression<Item> ItemBrandOrder
        {
            get { return new OrderByExpression<Item, int>(x => x.BrandIndex); }
        }

        private string priceGroup;
        private CatalogModel dbModel;

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

        public IEnumerable<ItemListView> GetItemList(IFilterExpression<CatalogModel, Item> filter, IOrderByExpression<Item> order = null, bool? includeVat = null, int skip = 0, int? take = null)
        {
            var items = dbModel.Items.AsQueryable();

            if (filter != null)
                items = filter.ApplyFilter(dbModel, items);

            if (order != null)
                items = order.ApplyOrdering(items);

            if (includeVat == null)
                includeVat = BasketController.SalesChannel.ShowPricesIncludingVat;

            var currentCurrencyCode = CurrencyController.CurrentCurrency != null ? CurrencyController.CurrentCurrency.Code : null;
            var defaultCurrencyCode = CurrencyController.DefaultCurrency != null ? CurrencyController.DefaultCurrency.Code : null;

            var data = (from i in items
                        /* Item Localizations */                        
                        from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from g in i.Localizations.Where(x => x.Culture == null)                        
                        /* Category and Catalogy Localizations */
                        let cat = i.Category
                        from cat_l in cat.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from cat_g in cat.Localizations.Where(x => x.Culture == null).DefaultIfEmpty()
                        /* List Prices */
                        from cs in i.ListPrices.Where(x => x.CurrencyCode == currentCurrencyCode && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                        from ds in i.ListPrices.Where(x => x.CurrencyCode == defaultCurrencyCode && x.PriceGroupCode == priceGroup).DefaultIfEmpty()                        
                        from c in i.ListPrices.Where(x => x.CurrencyCode == currentCurrencyCode && x.PriceGroupCode == null).DefaultIfEmpty()
                        from d in i.ListPrices.Where(x => x.CurrencyCode == defaultCurrencyCode && x.PriceGroupCode == null).DefaultIfEmpty()                        
                        let listPrice = ds != null ? (cs ?? ds) : (c ?? d)
                        let defaultListPrice = (c ?? d)
                        /* Map */          
                        select new
                        {
                            ID = i.ID,
                            ItemNo = i.ItemNo,
                            IsActive = i.IsActive,
                            Title = (l ?? g).Title,
                            VatGroupCode = i.VatGroupCode,
                            ListPrice = listPrice,
                            DefaultListPrice = defaultListPrice,
                            OnSale = i.OnSale,
                            VariantMode = i.VariantMode,

                            CategoryID = i.CategoryID,
                            CategoryTitle = (cat_l ?? cat_g).Title,
                            CategoryIndex = i.Index,

                            BrandID = i.BrandID,
                            BrandName = (i.Brand != null ? i.Brand.Name : null),
                            BrandIndex = i.BrandIndex,

                            ImageFile = (from img in i.Images orderby img.DisplayIndex select img.ImageFile).FirstOrDefault(),

                            /* Stock Management */
                            UseStockControl = i.UseStockControl,
                            StockQuantity = i.StockQuantity,
                            ReservedQuantity = i.ReservedQuantity,
                            IsAvailable = i.IsAvailable,
                        }).ToList();

            /* Map data to View Model */
            var viewData = new List<ItemListView>(data.Count());
                       
            foreach (var item in data)
            {
                var itemListView = new ItemListView {
                    ID = item.ID,
                    ItemNo = item.ItemNo,
                    IsActive = item.IsActive,

                    Title = item.Title,

                    OnSale = item.OnSale,
                    VariantMode = item.VariantMode,

                    CategoryID = item.CategoryID,
                    CategoryTitle = item.CategoryTitle,
                    CategoryIndex = item.CategoryIndex,

                    BrandID = item.BrandID,
                    BrandName = item.BrandName,
                    BrandIndex = item.BrandIndex,

                    ImageFile = item.ImageFile,

                    /* Stock Management */
                    StockQuantity = item.UseStockControl ? (int?)item.StockQuantity : null,
                    ReservedQuantity = item.UseStockControl ? (int?)item.ReservedQuantity : null,
                    IsAvailable = item.IsAvailable,
                };

                if (item.ListPrice != null)
                {
                    /* Correct the currency of the list price. */
                    var price = CurrencyController.ConvertAmount(item.ListPrice.Price, item.ListPrice.CurrencyCode, currentCurrencyCode);

                    /* Currect VAT, if needed. */
                    price = VatUtilities.GetDisplayPrice(price, item.VatGroupCode, item.ListPrice.PriceGroupCode, includeVat.Value);

                    /* 2013-08-13: Implementing advanced rounding options. */
                    price = CurrencyController.ApplyRoundingRule(price, CurrencyController.CurrentCurrency);

                    itemListView.ListPrice = new ItemListPriceView() 
                    {
                        Price = price,
                        CurrencyCode = currentCurrencyCode
                    };               
                }

                if (item.DefaultListPrice != null)
                {
                    /* Correct the currency of the list price. */
                    var price = CurrencyController.ConvertAmount(item.DefaultListPrice.Price, item.ListPrice.CurrencyCode, currentCurrencyCode);

                    /* Currect VAT, if needed. */
                    price = VatUtilities.GetDisplayPrice(price, item.VatGroupCode, item.ListPrice.PriceGroupCode, includeVat.Value);

                    /* 2013-08-13: Implementing advanced rounding options. */
                    price = CurrencyController.ApplyRoundingRule(price, CurrencyController.CurrentCurrency);

                    itemListView.DefaultListPrice = new ItemListPriceView()
                    {
                        Price = price,
                        CurrencyCode = currentCurrencyCode
                    };        
                }

                viewData.Add(itemListView);
            }

            return viewData;
        }

        public IEnumerable<ItemListView> SearchItems(string query, bool includeInActive = false, bool? includeVat = null, int skip = 0, int? take = null)
        {
            using (CatalogModel model = new CatalogModel())
            {
                var results = SearchHandler.Engine.Search(query, new string[] { "itemno" });

                IEnumerable<Guid> resultIds = null;

                if (results != null && results.Any())
                {
                    resultIds = results.Select(x => x.ID).Distinct().ToList();
                }

                if (resultIds == null || !resultIds.Any())
                {
                    return null;
                }

                var resultIdsArray = resultIds.ToArray();

                var viewData = GetItemList(new FilterExpression<CatalogModel, Item>(x => (includeInActive || x.IsActive) && resultIdsArray.Contains(x.ID)), includeVat: includeVat);
                return viewData.OrderByDescending(x => results.Single(y => y.ID == x.ID).Score);
            }
        }

        public ItemDetailView GetItem(string itemNo)
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

        public ItemDetailView GetItem(ItemViewRequest itemViewRequest)
        {
            return GetItem(itemViewRequest.ItemNo);
        }

        public IEnumerable<ItemListView> ListRelatedItems(Guid itemID)
        {
            var data = (from i in dbModel.Items
                        from g in dbModel.ItemLocalizations.Where(x => x.ItemID == i.ID && x.Culture == null)
                        from l in dbModel.ItemLocalizations.Where(x => x.ItemID == i.ID && x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from ct in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.CurrentCurrency.Code && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                        from dt in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.DefaultCurrency.Code && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                        from c in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.CurrentCurrency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                        from d in i.ListPrices.Where(x => x.CurrencyCode == CurrencyController.DefaultCurrency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                        where i.IsActive && dbModel.ItemRelations.Any(x => x.ItemID == itemID && x.RelatedItemID == i.ID)
                        select new
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
                        }).ToList();

            /* Map data to View Model */
            var viewData = data.Select(x => new ItemListView
            {
                ID = x.ID,
                ItemNo = x.ItemNo,
                Title = x.Title,
                ListPrice = x.ListPrice != null ? new ItemListPriceView()
                {
                    CurrencyCode = x.ListPrice.CurrencyCode,
                    PriceGroupCode = x.ListPrice.PriceGroupCode,
                    MultiplePrices = x.ListPrice.MultiplePrices,
                    Price = x.ListPrice.Price
                } : null,
                DefaultListPrice = x.DefaultListPrice != x.ListPrice ? new ItemListPriceView()
                {
                    CurrencyCode = x.DefaultListPrice.CurrencyCode,
                    PriceGroupCode = x.DefaultListPrice.PriceGroupCode,
                    MultiplePrices = x.DefaultListPrice.MultiplePrices,
                    Price = x.DefaultListPrice.Price
                } : null,
                OnSale = x.OnSale,
                IsAvailable = x.IsAvailable,
                VariantMode = x.VariantMode,

                //CategoryID = x.CategoryID,
                //CategoryTitle = x.CategoryTitle,
                //CategoryIndex = x.CategoryIndex,

                //BrandID = x.BrandID,
                BrandName = x.BrandName,
                //BrandIndex = x.BrandIndex,

                ImageFile = x.ImageFile
            });

            return viewData.ToList();
        }

        public void Dispose()
        {
            dbModel.Dispose();
        }
    }
}
