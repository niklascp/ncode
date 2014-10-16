using nCode.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.Catalog
{
    /// <summary>
    /// Utilities for accesing Item Prices, and ajusting them currectly according to Vat Settings.
    /// </summary>
    public static class VatUtilities
    {
        private const string vatGroupsRequestItem = "nCode.Catalog.VatUtilities.VatGroups";

        private static object lockObject = new object();

        /// <summary>
        /// Return the raw price given a itemNo. Vat is not considered.
        /// </summary>
        private static PriceInfo GetItemPriceInternal(CatalogModel model, string itemNo, Currency currency, string priceGroupCode)
        {
            /* Valdate arguments. */
            if (model == null)
                throw new ArgumentNullException("model");

            if (string.IsNullOrEmpty(itemNo))
                throw new ArgumentNullException("itemNo", "ItemNo cannot be null or empty.");

            if (currency == null)
                throw new ArgumentNullException("currency");

            if (CurrencyController.DefaultCurrency == null)
                return null;

            var defaultCurrency = CurrencyController.DefaultCurrency;

            /* 2011-10-21: It is faster to simply load all prices for the item and do the processing here.
             * It is limited by O(c * p) where c is |Currency| and p is |PriceGroup|. */
            var prices = model.ItemPrices.Where(x => x.ItemNo == itemNo).ToList();

            var price = (from cp in prices.Where(x => x.CurrencyCode == currency.Code && x.PriceGroupCode != null && x.PriceGroupCode == priceGroupCode).DefaultIfEmpty()
                         from dp in prices.Where(x => x.CurrencyCode == defaultCurrency.Code && x.PriceGroupCode != null && x.PriceGroupCode == priceGroupCode).DefaultIfEmpty()
                         from c in prices.Where(x => x.CurrencyCode == currency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                         from d in prices.Where(x => x.CurrencyCode == defaultCurrency.Code && x.PriceGroupCode == null)
                         select
                            (dp != null ? (cp ?? dp) : (c ?? d))).SingleOrDefault();

            if (price != null)
            {
                var priceInfo = new PriceInfo()
                {
                    Price = price.Price,
                    PriceGroupCode = price.PriceGroupCode,
                    CurrencyCode = currency.Code
                };

                /* We might need to correct for using default currency */
                if (!string.Equals(price.CurrencyCode, currency.Code))
                    /* If price.CurrencyCode <> currency.Code, then price.CurrencyCode == Default Currency Code */
                    priceInfo.Price = priceInfo.Price * defaultCurrency.Rate / currency.Rate;

                return priceInfo;
            }

            return null;
        }

        /// <summary>
        /// Return the raw price given a itemNo and itemVariantID. Vat is not considered.
        /// </summary>
        private static PriceInfo GetItemVariantPriceInternal(CatalogModel model, string itemNo, Guid itemVariantID, Currency currency, string priceGroupCode)
        {
            /* Valdate arguments. */
            if (model == null)
                throw new ArgumentNullException("model");

            if (string.IsNullOrEmpty(itemNo))
                throw new ArgumentNullException("itemNo", "ItemNo cannot be null or empty.");

            if (currency == null)
                throw new ArgumentNullException("currency");

            var defaultCurrency = CurrencyController.DefaultCurrency;

            /* 2011-10-21: We load to list, since the translation of this query is incorrect. 
             * It showed even to be faster since the total number of variant prices is O(c * p) where v is |Currency| and p is |PriceGroup|. */
            var prices = model.ItemVariantPrices.Where(x => x.ItemVariantID == itemVariantID).ToList();
            var price = (from cp in prices.Where(x => x.CurrencyCode == currency.Code && x.PriceGroupCode != null && string.Equals(x.PriceGroupCode, priceGroupCode, StringComparison.OrdinalIgnoreCase)).DefaultIfEmpty()
                         from dp in prices.Where(x => x.CurrencyCode == defaultCurrency.Code && x.PriceGroupCode != null && string.Equals(x.PriceGroupCode, priceGroupCode, StringComparison.OrdinalIgnoreCase)).DefaultIfEmpty()
                         from c in prices.Where(x => x.CurrencyCode == currency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                         from d in prices.Where(x => x.CurrencyCode == defaultCurrency.Code && x.PriceGroupCode == null).DefaultIfEmpty()
                         select
                            dp != null ? (cp ?? dp) : (c ?? d)).SingleOrDefault();

            if (price != null)
            {
                var priceInfo = new PriceInfo()
                {
                    Price = price.Price,
                    PriceGroupCode = price.PriceGroupCode,
                    CurrencyCode = currency.Code
                };

                /* We might need to correct for using default currency */
                if (!string.Equals(price.CurrencyCode, currency.Code))
                    /* If price.CurrencyCode <> currency.Code, then price.CurrencyCode == Default Currency Code */
                    priceInfo.Price = priceInfo.Price * defaultCurrency.Rate / currency.Rate;

                return priceInfo;
            }

            return GetItemPriceInternal(model, itemNo, currency, priceGroupCode);
        }


        /* Common properties. */

        /// <summary>
        /// Gets available vat groups. Cached for each request.
        /// </summary>
        public static IDictionary<String, VatGroup> VatGroups
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items[vatGroupsRequestItem] != null)
                    return (IDictionary<String, VatGroup>)HttpContext.Current.Items[vatGroupsRequestItem];

                lock (lockObject)
                {
                    using (var model = new CatalogModel())
                    {
                        var vatGroups = model.VatGroups.ToDictionary(x => x.Code);

                        if (HttpContext.Current != null && HttpContext.Current.Items[vatGroupsRequestItem] == null)
                            HttpContext.Current.Items[vatGroupsRequestItem] = vatGroups;

                        return vatGroups;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the default vat group.
        /// </summary>
        public static VatGroup DefaultVatGroup
        {
            get
            {
                return VatGroups.Values.SingleOrDefault(x => x.IsDefault);
            }
        }


        /* Item Price functions. */

        private static void UpdateListPrice(CatalogModel model, Item item, Currency currency, string priceGroupCode)
        {
            /* Get the current item list price if any. */
            var itemListPrice = (from p in model.ItemListPrices
                                 where
                                     p.ItemNo == item.ItemNo &&
                                     p.CurrencyCode == currency.Code &&
                                     (priceGroupCode != null ? p.PriceGroupCode == priceGroupCode : p.PriceGroupCode == null)
                                 select p).SingleOrDefault();

            /* Get the default price */
            var defaultPrice = (from p in model.ItemPrices
                                where p.ItemNo == item.ItemNo &&
                                      p.CurrencyCode == currency.Code &&
                                      (priceGroupCode != null ? p.PriceGroupCode == priceGroupCode : p.PriceGroupCode == null)
                                select p).SingleOrDefault();

            if (defaultPrice == null)
            {
                if (itemListPrice != null)
                    model.ItemListPrices.DeleteOnSubmit(itemListPrice);

                return;
            }
            else
            {
                if (itemListPrice == null)
                {
                    itemListPrice = new ItemListPrice();
                    itemListPrice.ID = Guid.NewGuid();
                    itemListPrice.ItemNo = item.ItemNo;
                    itemListPrice.CurrencyCode = currency.Code;
                    itemListPrice.PriceGroupCode = priceGroupCode;
                    model.ItemListPrices.InsertOnSubmit(itemListPrice);
                }

                itemListPrice.MultiplePrices = false;
                itemListPrice.Price = defaultPrice.Price;
            }

            if (item.VariantMode == VariantMode.Dependent)
            {
                /* Base Variant Price Query 
                 *
                 * Returns item variant prices for the given currency or null,
                 * if no price is stated in the given currency.
                 * 
                 *       Ex:  Var A:  10 
                 *            Var B:  --
                 *            Var C: 100 
                 *            
                 */
                var variantPrices = (from ivt in model.ItemVariantTypes.Where(x => x.ItemID == item.ID && x.Depth == model.ItemVariantTypes.Where(y => y.ItemID == item.ID).Max(y => y.Depth))
                                     from iv in model.ItemVariants.Where(x => x.ItemVariantTypeID == ivt.ID)
                                     from ivp in model.ItemVariantPrices.Where(x => x.ItemVariantID == iv.ID && x.CurrencyCode == currency.Code && (priceGroupCode != null ? x.PriceGroupCode == priceGroupCode : x.PriceGroupCode == null)).DefaultIfEmpty()
                                     select ivp != null ? ivp.Price : (decimal?)null);

                var minPrice = variantPrices.Where(x => x != null).Min();


                if (minPrice != null)
                {
                    var usesDefaultPrice = variantPrices.Any(x => x == null);

                    /* Case 1: Min price is smaller then the default price, or no variant uses the default price. */
                    if (minPrice < defaultPrice.Price || !usesDefaultPrice)
                        itemListPrice.Price = minPrice.Value;

                    /* Check for multiple prices: Case 1:
                     * 
                     *   Ex:  Var A: 10     Default: 12
                     *        Var B: --
                     *             
                     *   Ex:  Var A: --     Default: 10
                     *        Var B: 12
                     */
                    if (usesDefaultPrice && minPrice != defaultPrice.Price)
                        itemListPrice.MultiplePrices = true;

                    /* Check for multiple prices: Case 2:
                     * 
                     *   Ex:  Var A: 10     Default: 10
                     *        Var B: 12
                     */
                    else if (minPrice != variantPrices.Where(x => x != null).Max())
                        itemListPrice.MultiplePrices = true;
                }
            }
        }

        /// <summary>
        /// Recalculates the list price for the given item, and updates the datamodel. 
        /// The model is not comitted during this process.
        /// </summary>
        public static void UpdateListPrice(CatalogModel model, Item item)
        {
            var defaultCurrency = CurrencyController.Currencies.SingleOrDefault(x => x.IsDefault);

            if (defaultCurrency == null)
                return;

            /* We need to do this for all currencies */
            foreach (var currency in CurrencyController.Currencies)
            {
                foreach (var priceGroupCode in model.PriceGroups.Select(x => x.Code))
                    UpdateListPrice(model, item, currency, priceGroupCode);
            }
        }

        /// <summary>
        /// Gets the price for the given Item in the given Currency and Price Group, with or without Vat. If IncludeVat the value of 
        /// SalesSettings.ShowPricesIncludingVat will be used.
        /// </summary>
        public static decimal? GetItemPrice(string itemNo, string currencyCode, string priceGroupCode = null, bool? includeVat = null)
        {
            using (var model = new CatalogModel())
            using (var db = new CatalogDbContext())
            {
                var currency = CurrencyController.Currencies.Single(c => c.Code == currencyCode);

                var priceInfo = GetItemPriceInternal(model, itemNo, currency, priceGroupCode);

                if (priceInfo != null)
                {
                    var priceGroup = db.PriceGroups.Where(x => x.Code == priceInfo.PriceGroupCode).Single();

                    var vatGroupCode = (from i in model.Items where i.ItemNo == itemNo select i.VatGroupCode).SingleOrDefault();

                    if (includeVat == null)
                        includeVat = BasketController.SalesChannel.ShowPricesIncludingVat;

                    var price = GetDisplayPrice(priceInfo.Price, vatGroupCode, priceInfo.PriceGroupCode, includeVat.Value);

                    /* 2013-08-13: Implementing advanced rounding options. */
                    return CurrencyController.ApplyRoundingRule(price, currency);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the price for the given Item in the given Currency, with or without Vat.
        /// </summary>
        public static decimal? GetItemPrice(string itemNo, string currencyCode, bool includeVat)
        {
            return GetItemPrice(itemNo, currencyCode, null, includeVat);
        }

        /// <summary>
        /// Gets the price for the given Item Variant in the given Currency, with or without Vat. If IncludeVat the value of 
        /// SalesSettings.ShowPricesIncludingVat will be used.
        /// </summary>
        public static decimal? GetItemVariantPrice(string itemNo, Guid? itemVariantID, string currencyCode, string priceGroupCode = null, bool? includeVat = null)
        {
            using (var model = new CatalogModel())
            using (var db = new CatalogDbContext())
            {
                var currency = CurrencyController.Currencies.Single(c => c.Code == currencyCode);

                PriceInfo priceInfo;

                if (itemVariantID != null)
                    priceInfo = GetItemVariantPriceInternal(model, itemNo, itemVariantID.Value, currency, priceGroupCode);
                else
                    priceInfo = GetItemPriceInternal(model, itemNo, currency, priceGroupCode);

                if (priceInfo != null)
                {
                    var priceGroup = db.PriceGroups.Where(x => x.Code == priceInfo.PriceGroupCode).Single();

                    var vatGroupCode = (from i in model.Items where i.ItemNo == itemNo select i.VatGroupCode).SingleOrDefault();

                    if (includeVat == null)
                        includeVat = BasketController.SalesChannel.ShowPricesIncludingVat;

                    var price = GetDisplayPrice(priceInfo.Price, vatGroupCode, priceInfo.PriceGroupCode, includeVat.Value);

                    /* 2013-08-13: Implementing advanced rounding options. */
                    return CurrencyController.ApplyRoundingRule(price, currency);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the price for the given Item Variant in the given Currency, with or without Vat.
        /// </summary>
        public static decimal? GetItemVariantPrice(string itemNo, Guid? itemVariantID, string currencyCode, bool includeVat)
        {
            return GetItemVariantPrice(itemNo, itemVariantID, currencyCode, null, includeVat);
        }

        /// <summary>
        /// Gets a listing price an item.
        /// </summary>
        /// <param name="itemNo">ItemNo</param>
        /// <param name="currencyCode">CurrencyCode</param>
        /// <param name="includeVat">Indicates if the found price should include Vat.</param>
        /// <param name="multiplePrices">Outputs true if there exists higher prices then the one found.</param>
        /// <returns>The found listing price for the given item.</returns>
        [Obsolete("You should use direct lookup from the ItemListPrice table.")]
        public static decimal? GetItemListPrice(string itemNo, string currencyCode, bool includeVat, out bool multiplePrices)
        {
            using (var model = new CatalogModel())
            {
                var defaultCurrency = CurrencyController.Currencies.SingleOrDefault(x => x.IsDefault);
                var currency = CurrencyController.Currencies.SingleOrDefault(x => x.Code == currencyCode);

                /* Get some basic information about the item. */
                var item = (from i in model.Items
                            where i.ItemNo == itemNo
                            select new
                            {
                                i.ID,
                                i.VariantMode,
                                i.VatGroupCode
                            }).SingleOrDefault();

                multiplePrices = false;

                /* Nothing to do. */
                if (item == null || defaultCurrency == null || currency == null)
                {
                    return null;
                }

                decimal? price = null;

                /* Get the list price for the currency if any. */
                var listPrice = (from p in model.ItemListPrices
                                 where
                                     p.ItemNo == itemNo &&
                                     p.CurrencyCode == currency.Code &&
                                     p.PriceGroupCode == null
                                 select new
                                 {
                                     Price = p.Price,
                                     MultiplePrices = p.MultiplePrices
                                 }).SingleOrDefault();

                if (listPrice != null)
                {
                    multiplePrices = listPrice.MultiplePrices;
                    price = listPrice.Price;
                }
                /* Get the list price for the default currency if any. */
                else if (currency != defaultCurrency)
                {
                    listPrice = (from p in model.ItemListPrices
                                 where
                                     p.ItemNo == itemNo &&
                                     p.CurrencyCode == defaultCurrency.Code &&
                                     p.PriceGroupCode == null
                                 select new
                                 {
                                     Price = p.Price * defaultCurrency.Rate / currency.Rate,
                                     MultiplePrices = p.MultiplePrices
                                 }).SingleOrDefault();

                    if (listPrice != null)
                    {
                        multiplePrices = listPrice.MultiplePrices;
                        price = listPrice.Price;
                    }
                }

                /* Consider Vat */
                if (price != null && includeVat != SalesSettings.PricesIncludesVat)
                {
                    if (includeVat)
                        price = GetPrice((decimal)price, item.VatGroupCode);
                    else
                        price = GetNetPrice((decimal)price, item.VatGroupCode);

                    /* 2012-01-31: Round correctly to 2 decimal points, before using the price. */
                    if (price != null)
                        price = Math.Round(price.Value, 2, MidpointRounding.AwayFromZero);
                }

                return price;
            }
        }


        /* Other helping functions. */

        /// <summary>
        /// Adds vat to the given amount depending on the given vat group code.
        /// </summary>
        public static decimal GetAmount(decimal netAmount, string vatGroupCode)
        {
            if (vatGroupCode == null || !VatGroups.ContainsKey(vatGroupCode))
                return netAmount;

            return GetAmount(netAmount, VatGroups[vatGroupCode]);
        }

        /// <summary>
        /// Adds vat to the given amount depending on the given vat group.
        /// </summary>
        public static decimal GetAmount(decimal netAmount, VatGroup vatGroup)
        {
            if (vatGroup == null)
                return netAmount;

            return netAmount * (1 + vatGroup.Rate);
        }

        /// <summary>
        /// Subtracts vat from the given amount depending on the given vat group code.
        /// </summary>
        public static decimal GetNetAmount(decimal amount, string vatGroupCode)
        {
            if (vatGroupCode == null || !VatGroups.ContainsKey(vatGroupCode))
                return amount;

            return GetNetAmount(amount, VatGroups[vatGroupCode]);
        }

        /// <summary>
        /// Subtracts vat from the given amount depending on the given vat group.
        /// </summary>
        public static decimal GetNetAmount(decimal amount, VatGroup vatGroup)
        {
            if (vatGroup == null)
                return amount;

            return amount / (1 + vatGroup.Rate);
        }

        /// <summary>
        /// Gets the price incl. vat, depending on whether prices includes vat, and the given vat group code.
        /// </summary>
        [Obsolete("You should use VatUtilities.GetDisplayPrice.")]
        public static decimal GetPrice(decimal price, string vatGroupCode)
        {
            if (SalesSettings.PricesIncludesVat)
                return price;

            return GetAmount(price, vatGroupCode);
        }

        /// <summary>
        /// Gets the price incl. vat, depending on whether prices includes vat, and the given vat group.
        /// </summary>
        [Obsolete("You should use VatUtilities.GetDisplayPrice.")]
        public static decimal GetPrice(decimal price, VatGroup vatGroup)
        {
            if (SalesSettings.PricesIncludesVat)
                return price;

            return GetAmount(price, vatGroup);
        }

        /// <summary>
        /// Gets the price excl. vat, depending on whether prices includes vat, and the given vat group code.
        /// </summary>
        [Obsolete("You should use VatUtilities.GetDisplayPrice.")]
        public static decimal GetNetPrice(decimal price, string vatGroupCode)
        {
            if (!SalesSettings.PricesIncludesVat)
                return price;

            return GetNetAmount(price, vatGroupCode);
        }

        /// <summary>
        /// Gets the price excl. vat, depending on whether prices includes vat, and the given vat group.
        /// </summary>
        [Obsolete("You should use VatUtilities.GetDisplayPrice.")]
        public static decimal GetNetPrice(decimal price, VatGroup vatGroup)
        {
            if (!SalesSettings.PricesIncludesVat)
                return price;

            return GetNetAmount(price, vatGroup);
        }

        public static decimal GetDisplayPrice(decimal price, string vatGroupCode = null, string priceGroupCode = null, string salesChannelCode = null)
        {
            using (var dbContext = new CatalogDbContext())
            {
                /* TODO: Cache this for performance! */
                var vatGroup = dbContext.VatGroups.Where(x => x.Code == vatGroupCode).SingleOrDefault();
                var priceGroup = dbContext.PriceGroups.Where(x => x.Code == priceGroupCode).Single();
                var salesChannel = dbContext.SalesChannels.Where(x => x.Code == salesChannelCode).SingleOrDefault();

                return GetDisplayPrice(price, vatGroup, priceGroup, salesChannel);
            }
        }

        public static decimal GetDisplayPrice(decimal price, string vatGroupCode, string priceGroupCode , bool includeVat)
        {
            using (var dbContext = new CatalogDbContext())
            {
                /* TODO: Cache this for performance! */
                var vatGroup = dbContext.VatGroups.Where(x => x.Code == vatGroupCode).SingleOrDefault();
                var priceGroup = dbContext.PriceGroups.Where(x => x.Code == priceGroupCode).Single();

                return GetDisplayPrice(price, vatGroup, priceGroup, includeVat);
            }
        }

        public static decimal GetDisplayPrice(decimal price, Models.VatGroup vatGroup, Models.PriceGroup priceGroup, SalesChannel salesChannel)
        {
            if (vatGroup == null)
                return price;
            else if (!priceGroup.PricesIncludeVat && salesChannel.ShowPricesIncludingVat)
                return price * (1 + vatGroup.Rate);
            else if (priceGroup.PricesIncludeVat && !salesChannel.ShowPricesIncludingVat)
                return price / (1 + vatGroup.Rate);
            else
                return price;
        }

        public static decimal GetDisplayPrice(decimal price, Models.VatGroup vatGroup, Models.PriceGroup priceGroup, bool includeVat)
        {
            if (vatGroup == null)
                return price;
            else if (!priceGroup.PricesIncludeVat && includeVat)
                return price * (1 + vatGroup.Rate);
            else if (priceGroup.PricesIncludeVat && !includeVat)
                return price / (1 + vatGroup.Rate);
            else
                return price;
        }
    }

    public class PriceInfo
    {
        public decimal Price { get; set; }

        public string PriceGroupCode { get; set; }

        public string CurrencyCode { get; set; }
    }
}
