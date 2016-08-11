using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using nCode.Catalog.UI;
using nCode.Catalog.Models;
using nCode.Catalog.ViewModels;
using nCode.Data.Linq;
using nCode.Search;

using Dapper;

namespace nCode.Catalog.Data
{
    public class CatalogRepository : ICatalogRepository
    {
        #region Queries

        private const string listItemByCategoryQuery = @"
with
[CategoryFilter] ([CategoryID])
as
(
    select @categoryId
    union all
    select c.[Id]
    from
		[Catalog_Category] c
		join [CategoryFilter] f on f.[CategoryID] = c.[Parent] and @includeDescendantCategories = 1
)
select 
	i.[ID],
	i.[ItemNo],
	i.[IsActive],
	[Title] = isnull(l.[Title], g.[Title]),
	i.[VatGroupCode],
	i.[OnSale],
	i.[VariantMode],
	-- List Price
	[ListPriceCurrencyCode] = case
		when pg_c.[ID] is not null then pg_c.[CurrencyCode]
		when pg_d.[ID] is not null then pg_d.[CurrencyCode]
		when p_c.[ID] is not null then p_c.[CurrencyCode]
		else p_d.[CurrencyCode]
	end,
	[ListPricePriceGroupCode] = case
		when pg_c.[ID] is not null then pg_c.[PriceGroupCode]
		when pg_d.[ID] is not null then pg_d.[PriceGroupCode]
		when p_c.[ID] is not null then p_c.[PriceGroupCode]
		else p_d.[PriceGroupCode]
	end,
	[ListPricePrice] = case
		when pg_c.[ID] is not null then pg_c.[Price]
		when pg_d.[ID] is not null then pg_d.[Price]
		when p_c.[ID] is not null then p_c.[Price]
		else p_d.[Price]
	end,
	[ListPriceMultiplePrices] = case
		when pg_c.[ID] is not null then pg_c.[MultiplePrices]
		when pg_d.[ID] is not null then pg_d.[MultiplePrices]
		when p_c.[ID] is not null then p_c.[MultiplePrices]
		else p_d.[MultiplePrices]
	end,
	-- Default List Price
	[DefaultListPriceCurrencyCode] = case
		when p_c.[ID] is not null then p_c.[CurrencyCode]
		else p_d.[CurrencyCode]
	end,
	[DefaultListPricePriceGroupCode] = case
		when p_c.[ID] is not null then p_c.[PriceGroupCode]
		else p_d.[PriceGroupCode]
	end,
	[DefaultListPricePrice] = case
		when p_c.[ID] is not null then p_c.[Price]
		else p_d.[Price]
	end,
	[DefaultListPriceMultiplePrices] = case
		when pg_c.[ID] is not null then pg_c.[MultiplePrices]
		when pg_d.[ID] is not null then pg_d.[MultiplePrices]
		when p_c.[ID] is not null then p_c.[MultiplePrices]
		else p_d.[MultiplePrices]
	end,
	-- Category
	[CategoryID] = i.[Category],
	[CategoryTitle] = isnull(cat_l.[Title], cat_g.[Title]),
	[CategoryIndex] = i.[Index],
	-- Brand
	[BrandID] = i.[Brand],
	[BrandName] = brand.[Name],
	[BrandIndex] = i.[BrandIndex],
	-- Image
	[ImageFile] = (select top 1 img.[ImageFile] from [Catalog_ItemImage] img where img.[ItemID] = i.[ID] order by img.[DisplayIndex]),
	-- Stock Management
	i.[UseStockControl],
    i.[IsSoldOut],
	i.[StockQuantity],
	i.[ReservedQuantity]
from
	[Catalog_Item] i
	left join [Catalog_ItemLocalization] l on l.[Item] = i.[ID] and l.[Culture] = @cultureCode
	left join [Catalog_ItemLocalization] g on g.[Item] = i.[ID] and g.[Culture] is null

	left join [Catalog_CategoryLocalization] cat_l on cat_l.[CategoryID] = i.[Category] and cat_l.[Culture] = @cultureCode
	left join [Catalog_CategoryLocalization] cat_g on cat_g.[CategoryID] = i.[Category] and cat_g.[Culture] is null
	-- Price (Specified Price Group)
	left join [Catalog_ItemListPrice] pg_c on pg_c.[ItemNo] = i.[ItemNo] and pg_c.[CurrencyCode] = @currentCurrencyCode and pg_c.[PriceGroupCode] = @priceGroupCode
	left join [Catalog_ItemListPrice] pg_d on pg_d.[ItemNo] = i.[ItemNo] and pg_d.[CurrencyCode] = @defaultCurrencyCode and pg_d.[PriceGroupCode] = @priceGroupCode
	-- Price (Default Price Group)
	left join [Catalog_ItemListPrice] p_c on p_c.[ItemNo] = i.[ItemNo] and p_c.[CurrencyCode] = @currentCurrencyCode and p_c.[PriceGroupCode] is null
	left join [Catalog_ItemListPrice] p_d on p_d.[ItemNo] = i.[ItemNo] and p_d.[CurrencyCode] = @defaultCurrencyCode and p_c.[PriceGroupCode] is null

	left join [Catalog_Brand] brand on brand.[ID] = i.[Brand]

	join [CategoryFilter] filter_cat on filter_cat.[CategoryID] = i.[Category]
where
	i.[IsActive] = 1 or @includeInactive = 1
order by
    i.[Index];
";

        #endregion

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

        public IEnumerable<ItemListView> ListItemByCategory(Guid categoryId, bool includeDescendantCategories = false, bool includeInactive = false, bool? includeVat = null, int skip = 0, int? take = null)
        {
            bool _includeVat = includeVat ?? BasketController.SalesChannel.ShowPricesIncludingVat;

            var currentCurrencyCode = CurrencyController.CurrentCurrency?.Code;
            var defaultCurrencyCode = CurrencyController.DefaultCurrency?.Code;

            var data = dbModel.Connection.Query<ItemListDto>(listItemByCategoryQuery, new
            {
                cultureCode = CultureInfo.CurrentUICulture.Name,
                currentCurrencyCode = currentCurrencyCode,
                defaultCurrencyCode = defaultCurrencyCode,
                priceGroupCode = priceGroup,
                categoryId = categoryId,
                includeDescendantCategories = includeDescendantCategories,
                includeInactive = includeInactive,
            });

            /* Map data to View Model */
            var viewData = new List<ItemListView>(data.Count());

            foreach (var item in data)
            {
                var itemListView = new ItemListView
                {
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

                if (item.ListPricePrice != null)
                {
                    /* Correct the currency of the list price. */
                    var price = CurrencyController.ConvertAmount(item.ListPricePrice.Value, item.ListPriceCurrencyCode, currentCurrencyCode);

                    /* Currect VAT, if needed. */
                    price = VatUtilities.GetDisplayPrice(price, item.VatGroupCode, item.ListPricePriceGroupCode, _includeVat);

                    /* 2013-08-13: Implementing advanced rounding options. */
                    price = CurrencyController.ApplyRoundingRule(price, CurrencyController.CurrentCurrency);

                    itemListView.ListPrice = new ItemListPriceView()
                    {
                        Price = price,
                        CurrencyCode = currentCurrencyCode,
                        MultiplePrices = item.ListPriceMultiplePrices.Value,
                        PriceGroupCode = item.ListPricePriceGroupCode
                    };
                }

                if (item.DefaultListPricePrice != null)
                {
                    /* Correct the currency of the list price. */
                    var price = CurrencyController.ConvertAmount(item.DefaultListPricePrice.Value, item.DefaultListPriceCurrencyCode, currentCurrencyCode);

                    /* Currect VAT, if needed. */
                    price = VatUtilities.GetDisplayPrice(price, item.VatGroupCode, item.DefaultListPricePriceGroupCode, _includeVat);

                    /* 2013-08-13: Implementing advanced rounding options. */
                    price = CurrencyController.ApplyRoundingRule(price, CurrencyController.CurrentCurrency);

                    itemListView.DefaultListPrice = new ItemListPriceView()
                    {
                        Price = price,
                        CurrencyCode = currentCurrencyCode,
                        MultiplePrices = item.DefaultListPriceMultiplePrices.Value,
                        PriceGroupCode = item.DefaultListPricePriceGroupCode
                    };
                }

                viewData.Add(itemListView);
            }

            return viewData;
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
                        CurrencyCode = currentCurrencyCode,
                        MultiplePrices = item.ListPrice.MultiplePrices,
                        PriceGroupCode = item.ListPrice.PriceGroupCode
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
                        CurrencyCode = currentCurrencyCode,
                        MultiplePrices = item.ListPrice.MultiplePrices,
                        PriceGroupCode = item.ListPrice.PriceGroupCode
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
            string currentCurrencyCode = CurrencyController.CurrentCurrency?.Code;
            string defaultCurrencyCode = CurrencyController.DefaultCurrency?.Code;

            var data = (from i in dbModel.Items
                        from g in dbModel.ItemLocalizations.Where(x => x.ItemID == i.ID && x.Culture == null)
                        from l in dbModel.ItemLocalizations.Where(x => x.ItemID == i.ID && x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from ct in i.ListPrices.Where(x => x.CurrencyCode == currentCurrencyCode && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                        from dt in i.ListPrices.Where(x => x.CurrencyCode == defaultCurrencyCode && x.PriceGroupCode == priceGroup).DefaultIfEmpty()
                        from c in i.ListPrices.Where(x => x.CurrencyCode == currentCurrencyCode && x.PriceGroupCode == null).DefaultIfEmpty()
                        from d in i.ListPrices.Where(x => x.CurrencyCode == defaultCurrencyCode && x.PriceGroupCode == null).DefaultIfEmpty()
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

        private class ItemListDto
        {
            public Guid ID { get; set; }
            public string ItemNo { get; set; }
            public bool IsActive { get; set; }

            public string Title { get; set; }
            public string VatGroupCode { get; set; }

            /* List Price */

            public string ListPriceCurrencyCode { get; set; }

            public string ListPricePriceGroupCode { get; set; }

            public decimal? ListPricePrice { get; set; }

            public bool? ListPriceMultiplePrices { get; set; }

            /* Default List Price */

            public string DefaultListPriceCurrencyCode { get; set; }

            public string DefaultListPricePriceGroupCode { get; set; }

            public decimal? DefaultListPricePrice { get; set; }

            public bool? DefaultListPriceMultiplePrices { get; set; }

            /* Misc */

            public bool OnSale { get; set; }
            public VariantMode VariantMode { get; set; }

            /* Category */

            public Guid? CategoryID { get; set; }
            public string CategoryTitle { get; set; }
            public int CategoryIndex { get; set; }

            /* Brand */

            public Guid? BrandID { get; set; }
            public string BrandName { get; set; }
            public int BrandIndex { get; set; }

            /* Image */

            public string ImageFile { get; set; }

            /* Stock Management */
            public bool UseStockControl { get; set; }
            public bool IsSoldOut { get; set; }
            public int StockQuantity { get; set; }
            public int ReservedQuantity { get; set; }

            /* Todo: Move this away from DTO and into logic. */
            public int Available
            {
                get
                {
                    return StockQuantity - ReservedQuantity;
                }
            }

            /* Todo: Move this away from DTO and into logic. */
            public bool IsAvailable
            {
                get
                {
                    /* This item has Dependent Variants, simply return static value represented by IsSoldOut. */
                    if (VariantMode == VariantMode.Dependent)
                    {
                        return !IsSoldOut;
                    }
                    /* Simple Stock Control */
                    else if (SalesSettings.StockControlLevel == StockControlLevel.Simple)
                    {
                        /* 2010-10-13: Ignore if marked as 'Sold Out'. */
                        return !IsSoldOut;
                    }
                    /* Normal/Advanced Stock Control */
                    else if (UseStockControl)
                    {
                        /* 2010-08-22: If we Allow Backorder, ignore Item stock check. */
                        return (SalesSettings.AllowBackorder || Available > 0);
                    }

                    return true;
                }
            }
        }
    }
}
