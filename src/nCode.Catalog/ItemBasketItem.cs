using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using nCode.Catalog.UI;

namespace nCode.Catalog
{
    /// <summary>
    /// Represents an Item in a Basket.
    /// </summary>
    public class ItemBasketItem : BasketItem
    {
        string loadedCulture;
        bool unitWeightLoaded;

        bool imageLoaded;
        bool vatGroupCodeLoaded;

        /// <summary>
        /// Gets or sets the item unique identifier.
        /// </summary>
        public Guid ItemID { get; set; }

        /// <summary>
        /// Gets or sets the item no.
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// Gets or sets the item variant unique identifier.
        /// </summary>
        public Guid? ItemVariantID { get; set; }

        /// <summary>
        /// Gets or sets the price group.
        /// </summary>
        public string PriceGroup { get; set; }

        /// <summary>
        /// Gets or sets the qty.
        /// </summary>
        public override int Qty
        {
            get
            {
                return base.Qty;
            }
            set
            {
                int qty = value;

                /* Intercept illegal quanties (i.e. stock control, sales qty, min sale qty) */
                using (var model = new CatalogModel())
                {
                    var item = (from i in model.Items
                                where i.ID == ItemID
                                select new
                                {
                                    i.UseStockControl,
                                    i.Available,
                                    i.MinimumOrderQuantity,
                                    i.MultipleOrderQuantity
                                }).Single();

                    /* Handle Minimum Order Quantity and Multiple Order Quantity */
                    if (item.MinimumOrderQuantity.HasValue && qty < item.MinimumOrderQuantity.Value)
                        qty = item.MinimumOrderQuantity.Value;

                    if (item.MultipleOrderQuantity.HasValue && qty % item.MultipleOrderQuantity != 0)
                        qty += item.MultipleOrderQuantity.Value - (qty % item.MultipleOrderQuantity.Value);

                    /* Handle Stock Control. */
                    if (SalesSettings.StockControlLevel != StockControlLevel.Simple && 
                        item.UseStockControl && 
                        !SalesSettings.AllowBackorder)
                    {
                        /* Item has variant. */
                        if (ItemVariantID != null)
                        {
                            var variant = (from i in model.ItemVariants
                                           where i.ID == ItemVariantID
                                           select new
                                           {
                                               i.Available
                                           }).Single();
                            if (qty > variant.Available)
                                qty = variant.Available;
                        }
                        else
                        {
                            if (qty > item.Available)
                                qty = item.Available;
                        }
                    }
                }

                base.Qty = qty;
            }
        }

        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        public override string ImageFile
        {
            get
            {
                if (!imageLoaded)
                {
                    using (var model = new CatalogModel())
                    {
                        var image = (from img in model.ItemImages
                                     where img.ItemID == ItemID
                                     orderby img.DisplayIndex
                                     select img).FirstOrDefault();

                        if (image != null)
                            base.ImageFile = image.ImageFile;

                        imageLoaded = true;
                    }
                }

                return base.ImageFile;
            }
            set
            {
                base.ImageFile = value;
                imageLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Setting Title of ItemBasketItem is not allowed.</exception>
        public override string Title
        {
            get
            {
                if (base.Title == null || loadedCulture != CultureInfo.CurrentUICulture.Name) {
                    base.Title = ItemUtilities.GetItemTitle(ItemID, ItemVariantID, CultureInfo.CurrentUICulture.Name);
                    loadedCulture = CultureInfo.CurrentUICulture.Name;
                }
                return base.Title;
            }
            set
            {
                throw new InvalidOperationException("Setting Title of ItemBasketItem is not allowed.");
            }
        }

        /// <summary>
        /// Gets or sets the unit weight.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Setting UnitWeight of ItemBasketItem is not allowed.</exception>
        public override decimal UnitWeight
        {
            get
            {
                if (!unitWeightLoaded)
                {
                    using (var model = new CatalogModel())
                    {
                        base.UnitWeight = (from i in model.Items where i.ItemNo == ItemNo select i.Weight).SingleOrDefault();
                    }

                    unitWeightLoaded = true;
                }

                return base.UnitWeight;
            }
            set
            {
                throw new InvalidOperationException("Setting UnitWeight of ItemBasketItem is not allowed.");
            }
        }

        /// <summary>
        /// Gets or sets the net unit price.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Setting NetUnitPrice of ItemBasketItem is not allowed.</exception>
        public override decimal NetUnitPrice
        {
            get
            {
                if (ItemNo == null)
                    return 0;

                if (ItemVariantID != null)
                    return VatUtilities.GetItemVariantPrice(ItemNo, ItemVariantID.Value, Basket.CurrencyCode, PriceGroup, false) ?? 0m;
                return VatUtilities.GetItemPrice(ItemNo, Basket.CurrencyCode, PriceGroup, false) ?? 0m;
            }
            set
            {
                throw new InvalidOperationException("Setting NetUnitPrice of ItemBasketItem is not allowed.");
            }
        }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Setting UnitPrice of ItemBasketItem is not allowed.</exception>
        public override decimal UnitPrice
        {
            get
            {
                if (ItemNo == null)
                    return 0;

                if (ItemVariantID != null)
                    return VatUtilities.GetItemVariantPrice(ItemNo, ItemVariantID.Value, Basket.CurrencyCode, PriceGroup, Basket.IncludeVat) ?? 0m;
                return VatUtilities.GetItemPrice(ItemNo, Basket.CurrencyCode, PriceGroup, Basket.IncludeVat) ?? 0m;
            }
            set
            {
                throw new InvalidOperationException("Setting UnitPrice of ItemBasketItem is not allowed.");
            }
        }

        /// <summary>
        /// Gets or sets the Vat Group Code.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Setting VatGroupCode of ItemBasketItem is not allowed.</exception>
        public override string VatGroupCode
        {
            get
            {
                if (!vatGroupCodeLoaded)
                {
                    using (var model = new CatalogModel())
                    {
                        base.VatGroupCode = model.Items.Where(x => x.ID == ItemID).Select(x => x.VatGroupCode).SingleOrDefault();
                        vatGroupCodeLoaded = true;
                    }
                }

                return base.VatGroupCode;
            }
            set
            {
                throw new InvalidOperationException("Setting VatGroupCode of ItemBasketItem is not allowed.");
            }
        }
    }
}
