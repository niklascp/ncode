using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using nCode.Catalog.Delivery;
using nCode.Catalog.Models;
using nCode.Catalog.Payment;

namespace nCode.Catalog
{
    /// <summary>
    /// Represents a Basket.
    /// </summary>
    public class Basket
    {
        private string currencyCode;                        /* Holds the currency code for this basket. */
        private string discountCode;                        /* Holds the discount code for this basket. */
        private List<BasketItem> items;                     /* Holds items currently in this basket. */
        private Dictionary<string, object> properties;      /* Holds additional user-defined properties. */

        /// <summary>
        /// Initializes a new instance of the <see cref="Basket"/> class.
        /// </summary>
        public Basket()
        {
            if (CurrencyController.CurrentCurrency != null)
                currencyCode = CurrencyController.CurrentCurrency.Code;

            items = new List<BasketItem>();
            properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Basket"/> class.
        /// </summary>
        /// <param name="salesChannelCode">The sales channel code.</param>
        public Basket(string salesChannelCode)
            : this()
        {
            SalesChannelCode = salesChannelCode;
        }

        private void UpdateDiscountAmount()
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                if (!string.IsNullOrEmpty(discountCode))
                {
                    var discount = (from dc in model.DiscountCodes
                                    where dc.Code == discountCode
                                    select dc).SingleOrDefault();

                    if (discount != null && discount.IsValid)
                    {
                        /* Calculate DiscountAmount depending on DiscountCode. */
                        switch (discount.DiscountType)
                        {
                            case DiscountType.FixedAmount:
                                if (discount.FixedAmount != null && discount.CurrencyCode != null)
                                {
                                    decimal maxDiscountAmount = CurrencyController.ConvertAmount(discount.FixedAmount.Value, discount.CurrencyCode, CurrencyCode);
                                    decimal discountAmount = Math.Min(maxDiscountAmount, SalesSettings.PricesIncludesVat ? ItemsTotal : ItemsNetTotal);
                                    DiscountNetAmount = VatUtilities.GetDisplayPrice(discountAmount, VatUtilities.DefaultVatGroup.Code, null, includeVat: false);
                                    DiscountAmount = VatUtilities.GetDisplayPrice(discountAmount, VatUtilities.DefaultVatGroup.Code, null, includeVat: IncludeVat);
                                    return;
                                }
                                break;

                            case DiscountType.Procentage:
                                if (discount.Procentage != null)
                                {
                                    DiscountNetAmount = discount.Procentage.Value * ItemsNetTotal;
                                    DiscountAmount = discount.Procentage.Value * ItemsTotal;
                                    return;
                                }
                                break;
                        }
                    }
                }
                
                /* Something went wrong - reset Discount Code. */
                discountCode = null;
                DiscountNetAmount = 0;
                DiscountAmount = 0;
            }
        }

        private void UpdateFreightAmount()
        {
            using (var model = new CatalogModel())
            {
                var deliveryType = model.DeliveryTypes.SingleOrDefault(x => x.Code == DeliveryTypeCode);

                if (deliveryType != null)
                {
                    var provider = DeliveryManager.Providers[deliveryType.ProviderName];
                    var freightAmount = provider.GetFreightAmount(model, this, deliveryType);

                    FreightVatGroupCode = provider.GetFreightVatGroupCode(model, this, deliveryType);
                    FreightNetAmount =  VatUtilities.GetDisplayPrice(freightAmount, FreightVatGroupCode, null, includeVat: false);
                    FreightAmount = VatUtilities.GetDisplayPrice(freightAmount, FreightVatGroupCode, null, includeVat: IncludeVat);                 
                }
                else
                {
                    FreightVatGroupCode = null;
                    FreightNetAmount = 0m;
                    FreightAmount = 0m;                    
                }
            }
        }

        private void UpdatePaymentFee()
        {
            using (var model = new CatalogModel())
            {
                var paymentType = model.PaymentTypes.SingleOrDefault(x => x.Code == PaymentTypeCode);

                if (paymentType != null)
                {
                    var provider = PaymentManager.Providers[paymentType.ProviderName];
                    PaymentFee = provider.GetPaymentFee(model, this, paymentType);
                }
                else
                {
                    PaymentFee = null;
                }
            }
        }


        /// <summary>
        /// Updates the sums.
        /// </summary>
        public void UpdateSums()
        {
            WeightTotal = items.Sum(i => i.Weight);
            ItemsNetTotal = items.Sum((item) => item.NetPrice);
            ItemsTotal = items.Sum((item) => item.Price);

            NetTotal = ItemsNetTotal;
            Total = ItemsTotal;

            /* Calculate Discount */
            UpdateDiscountAmount();
            NetTotal -= DiscountNetAmount;
            Total -= DiscountAmount;

            /* Calculate Freight Amount */
            UpdateFreightAmount();
            NetTotal += FreightNetAmount;
            Total += FreightAmount;

            UpdatePaymentFee();
            if (PaymentFee != null)
            {
                NetTotal += VatUtilities.GetDisplayPrice(PaymentFee.Amount, PaymentFee.VatGroupCode, null, includeVat: false);
                Total += VatUtilities.GetDisplayPrice(PaymentFee.Amount, PaymentFee.VatGroupCode, null, includeVat: IncludeVat);
            }
        }

        /// <summary>
        /// Gets or sets the Order No.
        /// </summary>
        public string OrderNo { get; set; }

        #region Customer Info
        
        /// <summary>
        /// Gets or sets the Customer No.
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// Gets or sets the Customer Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Customer Address1.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the Customer Address2.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the Customer PostalCode.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the Customer City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Customer CountryCode.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the Customer Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Customer Phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the Customer Ean.
        /// </summary>
        public string CustomerEan { get; set; }

        /// <summary>
        /// Gets or sets the Customer Reference.
        /// </summary>
        public string CustomerReference { get; set; }

        #endregion

        #region Shipping Info

        /// <summary>
        /// Gets or sets a value indicating whether use the Shipping Address.
        /// </summary>
        public bool UseShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Name.
        /// </summary>
        public string ShippingName { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Address1.
        /// </summary>
        public string ShippingAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Address2.
        /// </summary>
        public string ShippingAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Postal Code.
        /// </summary>
        public string ShippingPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the Shipping City.
        /// </summary>
        public string ShippingCity { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Country Code.
        /// </summary>
        public string ShippingCountryCode { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the Code for currently selected Payment Type.
        /// </summary>
        public string PaymentTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the Code for currently selected Delivery Type.
        /// </summary>
        public string DeliveryTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the Delivery Date.
        /// </summary>
        public DateTime? DeliveryDate { get; set; }


        /// <summary>
        /// Gets the Code for sales for the Sales Channel, that this Basket was created within.
        /// </summary>
        public string SalesChannelCode { get;  private set; }

        /// <summary>
        /// Gets or sets the Code for currently selected Currency.
        /// </summary>
        public string CurrencyCode { 
            get 
            { 
                return currencyCode;
            }
            set 
            {
                currencyCode = value;
                UpdateSums();
            }
        }

        /// <summary>
        /// Gets the currently selected Currency.
        /// </summary>
        public Currency Currency
        {
            get
            {
                if (currencyCode == null)
                    return null;

                using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
                {
                    return (from c in model.Currencies where c.Code == CurrencyCode select c).Single(); 
                }
            }
        }

        /// <summary>
        /// Gets or sets the a Comment.
        /// </summary>
        public string Comment { get; set; }


        #region Items

        /// <summary>
        /// Gets a list of Items in the Basket.
        /// </summary>
        public IList<BasketItem> Items
        {
            get { return items.AsReadOnly(); }
        }

        /// <summary>
        /// Adds the given item to the basket and updates basket sums.
        /// </summary>
        public void AddItem(BasketItem item)
        {
            if (item.Basket != null)
                throw new ApplicationException("The BasketItem is already added to a Basket");

            item.Basket = this;
            items.Add(item);
            UpdateSums();
        }

        /// <summary>
        /// Adds the given item to the basket at the given position (zero-based), and updates basket sums.
        /// </summary>
        public void InsertItem(BasketItem item, int position)
        {
            if (item.Basket != null)
                throw new ApplicationException("The BasketItem is already added to a Basket");

            item.Basket = this;
            items.Insert(position, item);
            UpdateSums();
        }

        /// <summary>
        /// Removes the given item from the basket and updates basket sums.
        /// </summary>
        public void RemoveItem(BasketItem item)
        {
            items.Remove(item);
            UpdateSums();
        }

        /// <summary>
        /// Removes all items from the basket and updates basket sums.
        /// </summary>
        public void ClearItems()
        {
            items.Clear();
            UpdateSums();
        }

        #endregion

        #region Items Total

        /// <summary>
        /// Gets the items net total.
        /// </summary>
        public decimal ItemsNetTotal { get; private set; }

        /// <summary>
        /// Gets the items total.
        /// </summary>
        public decimal ItemsTotal { get; private set; }

        /// <summary>
        /// Gets the total vat amount for the items.
        /// </summary>
        public decimal ItemsVatAmount { get { return ItemsTotal - ItemsNetTotal; } }

        #endregion

        #region Discount

        /// <summary>
        /// Gets or sets the discount code.
        /// </summary>
        public string DiscountCode
        {
            get
            {
                return discountCode;
            }
            set
            {
                if (!SalesSettings.AllowDiscountCodes)
                    return;

                discountCode = value;
                UpdateSums();
            }
        }

        /// <summary>
        /// Gets the discount net amount.
        /// </summary>
        public decimal DiscountNetAmount { get; private set; }

        /// <summary>
        /// Gets the discount amount.
        /// </summary>
        public decimal DiscountAmount { get; private set; }

        /// <summary>
        /// Gets the discount vat amount.
        /// </summary>
        public decimal DiscountVatAmount { get { return DiscountAmount - DiscountNetAmount; } }

        #endregion

        #region Freight

        /// <summary>
        /// Gets the freight vat group code.
        /// </summary>
        public string FreightVatGroupCode { get; private set; }

        /// <summary>
        /// Gets the freight net amount.
        /// </summary>
        public decimal FreightNetAmount { get; private set; }

        /// <summary>
        /// Gets the freight amount.
        /// </summary>
        public decimal FreightAmount { get; private set; }

        /// <summary>
        /// Gets the freight vat amount.
        /// </summary>
        public decimal FreightVatAmount { get { return FreightAmount - FreightNetAmount; } }

        #endregion

        #region PaymentFee

        /// <summary>
        /// Gets the Payment Fee, if any.
        /// </summary>
        public PaymentFee PaymentFee { get; set; }

        #endregion

        #region Totals

        /// <summary>
        /// Gets the weight total.
        /// </summary>
        public decimal WeightTotal { get; private set; }

        /// <summary>
        /// Gets the net total.
        /// </summary>
        public decimal NetTotal { get; private set; }

        /// <summary>
        /// Gets the total.
        /// </summary>
        public decimal Total { get; private set; }

        #endregion


        /// <summary>
        /// Gets the total vat amount.
        /// </summary>
        public decimal VatAmount { get { return Total - NetTotal; } }

        /// <summary>
        /// Gets a value indicating if this order should include vat, depending on the delivery country.
        /// </summary>
        public bool IncludeVat
        {
            get
            {
                string countryCode = UseShippingAddress ? ShippingCountryCode : CountryCode;
                bool? includeVat = null;

                using (var model = new CatalogModel())
                {
                    if (countryCode != null)
                        includeVat = (from c in model.Countries where c.Code == countryCode select c.IncludeVat).SingleOrDefault();
                    if (includeVat == null)
                        includeVat = (from c in model.Countries where c.IsDefault select (bool?)c.IncludeVat).SingleOrDefault() ?? true;
                }

                return includeVat.Value;
            }
        }

        /// <summary>
        /// Gets the Metadata Property identified by the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T GetProperty<T>(string key, T defaultValue)
        {
            object value;

            if (properties.TryGetValue(key, out value))
                return (T)value;
            
            return defaultValue;
        }

        /// <summary>
        /// Sets the Metadata Property identified by the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public void SetProperty<T>(string key, T value)
        {
            if (properties.ContainsKey(key))
                properties[key] = value;
            else
                properties.Add(key, value);
        }


        //public IList<XElement> GetPropertyElements()
        //{
        //    return GetPropertyElements(null);
        //}

        //public IList<XElement> GetPropertyElements(IDictionary<string, Func<object, string>> transformations)
        //{
        //    List<XElement> elements = new List<XElement>();

        //    foreach (string key in properties.Keys) 
        //    {
        //        var element = new XElement(key);
        //        object value = properties[key];
        //        Func<object, string> transformation;


        //        if (transformations != null && transformations.TryGetValue(key, out transformation))
        //            element.Value = transformation(value);
        //        else if (value != null)
        //            element.Value = value.ToString();

        //        elements.Add(element);
        //    }

        //    return elements;
        //}

        /// <summary>
        /// Places the Basket as an Order.
        /// </summary>
        public Order PlaceAsOrder(bool clearExistingItems = true)
        {
            using (var context = new CatalogDbContext())
            using (var model = new CatalogModel())
            {
                Order order = null;

                if (OrderNo != null)
                    order = model.Orders.SingleOrDefault(x => x.OrderNo == OrderNo);

                if (order == null)
                {
                    order = new Order();
                    order.ID = Guid.NewGuid();
                    order.Created = DateTime.Now;

                    if (OrderNo == null)
                        OrderNo = NoSerieNumber.GetNext("Order").ToString();
                    
                    order.OrderNo = OrderNo;
                    model.Orders.InsertOnSubmit(order);
                }
                
                order.Modified = DateTime.Now;

                order.CustomerNo = CustomerNo;
                order.Name = Name;
                order.Address1 = Address1;
                order.Address2 = Address2;
                order.PostalCode = PostalCode;
                order.City = City;
                order.CountryCode = CountryCode;
                
                order.Phone = Phone;
                order.Email = Email;
                order.CustomerEan = CustomerEan;
                order.CustomerReference = CustomerReference;

                if (UseShippingAddress)
                {
                    order.UseShippingAddress = true;
                    order.ShippingName = ShippingName;
                    order.ShippingAddress1 = ShippingAddress1;
                    order.ShippingAddress2 = ShippingAddress2;
                    order.ShippingPostalCode = ShippingPostalCode;
                    order.ShippingCity = ShippingCity;
                    order.ShippingCountryCode = ShippingCountryCode;
                }

                var salesChannel = context.SalesChannels.Where(x => x.Code == SalesChannelCode).Single();

                order.SalesChannelCode = salesChannel.Code;
                order.PaymentTypeCode = PaymentTypeCode;
                order.DeliveryTypeCode = DeliveryTypeCode;
                order.DeliveryDate = DeliveryDate;

                order.VatIncluded = salesChannel.ShowPricesIncludingVat;
                order.CurrencyCode = CurrencyCode;

                order.Culture = Thread.CurrentThread.CurrentUICulture.Name;
                
                /* FIX for 3.5 pre SP1 */
                model.SubmitChanges();
                order = (from o in model.Orders where o.ID == order.ID select o).Single();

                UpdateOrderItems(model, order, clearExistingItems);

                /* Handle Discount Code */
                if (order.DiscountCode != DiscountCode)
                {
                    /* Reset Discount Code */
                    if (order.DiscountCode != null)
                    {
                        model.DiscountCodes.Single(dc => dc.Code == order.DiscountCode).Applied--;
                    }

                    /* Apply Discount Code */
                    if (DiscountCode != null)
                    {
                        model.DiscountCodes.Single(dc => dc.Code == DiscountCode).Applied++;
                        order.DiscountCode = DiscountCode;
                        order.DiscountAmount = order.VatIncluded ? DiscountAmount : DiscountNetAmount;
                    }
                }

                /* Copy Freight */
                order.FreightAmount = order.VatIncluded ? FreightAmount : FreightNetAmount;
                order.FreightVatGroupCode = IncludeVat ? FreightVatGroupCode : null;

                if (PaymentFee != null)
                {
                    order.PaymentFeeAmount = PaymentFee.Amount;
                    order.PaymentFeeVatGroupCode = IncludeVat ? PaymentFee.VatGroupCode : null;
                }

                /* Legacy fix: Move Comment from Property to actual field. */
                if (properties.Keys.Contains("Comment"))
                {
                    Comment = properties["Comment"].ToString();
                    properties.Remove("Comment");
                }

                order.Comment = Comment;

                /* Copy Properties */
                foreach (string key in properties.Keys) {
                    // Reflective way of doing the pseudo-code: 
                    // order.SetProperty<properties[key].GetType()>(key, properties[key]);
                    typeof(Order)
                        .GetMethod("SetProperty")
                        .MakeGenericMethod(properties[key].GetType())
                        .Invoke(order, new object[] { key, properties[key] });                    
                }

                order.UpdateTotals();
                model.SubmitChanges();

                return order;
            }
        }

        private void UpdateOrderItems(CatalogModel model, Order order, bool clearExistingItems)
        {
            if (clearExistingItems)
            {
                /* Correct Qtyties */
                foreach (OrderItem orderItem in order.Items)
                {
                    if (string.IsNullOrEmpty(orderItem.ItemNo))
                        continue;

                    var item = model.Items.SingleOrDefault(i => i.ItemNo == orderItem.ItemNo);

                    if (item == null || !item.UseStockControl)
                        continue;

                    item.ReservedQuantity -= orderItem.Qty;

                    if (orderItem.ItemVariantID != null)
                    {
                        var itemVariant = model.ItemVariants.Single(x => x.ID == orderItem.ItemVariantID);
                        itemVariant.ReservedQuantity -= orderItem.Qty;
                    }

                    model.SubmitChanges();

                    /* 2013-10-24: Update Availability on Variant Tree */
                    item.UpdateAvailability(model);
                    model.SubmitChanges();
                }

                order.Items.Clear();
                model.SubmitChanges();
            }

            int index = order.Items.Count();
            foreach (BasketItem basketItem in Items)
            {
                var orderItem = new OrderItem();
                orderItem.ID = Guid.NewGuid();
                orderItem.Created = DateTime.Now;
                orderItem.Modified = DateTime.Now;
                orderItem.Order = order;
                orderItem.DisplayIndex = index;
                
                /* Copy Special Fields if Item, and reserve if stock control is active for the item. */
                if (basketItem is ItemBasketItem)
                {
                    var item = (from i in model.Items where i.ItemNo == ((ItemBasketItem)basketItem).ItemNo select i).Single();

                    orderItem.ItemNo = item.ItemNo;                    
                    
                    if (((ItemBasketItem)basketItem).ItemVariantID != null)
                    {
                        var itemVariant = model.ItemVariants.Single(x => x.ID == ((ItemBasketItem)basketItem).ItemVariantID);
                        orderItem.ItemVariantID = itemVariant.ID;
                        orderItem.UnitCostPrice = !string.IsNullOrEmpty(item.CostCurrencyCode) ? CurrencyController.ConvertAmount(itemVariant.CostPrice ?? item.CostPrice, item.CostCurrencyCode, CurrencyCode) : 0m;

                        if (item.UseStockControl)
                        {
                            item.ReservedQuantity += basketItem.Qty;
                            itemVariant.ReservedQuantity += basketItem.Qty;
                        }
                    }
                    else
                    {
                        orderItem.UnitCostPrice = !string.IsNullOrEmpty(item.CostCurrencyCode) ? CurrencyController.ConvertAmount(item.CostPrice, item.CostCurrencyCode, CurrencyCode) : 0m;

                        if (item.UseStockControl)
                            item.ReservedQuantity += basketItem.Qty;
                    }
                }

                orderItem.ImageFile = basketItem.ImageFile;
                orderItem.Title = basketItem.Title.Length <= 255 ? basketItem.Title : basketItem.Title.Substring(0, 255);
                orderItem.Qty = basketItem.Qty;
                /* 2011-09-18: Set UnitPrice depending on wheater the order is incl. or excl. vat. */
                orderItem.UnitPrice = order.VatIncluded ? basketItem.UnitPrice : basketItem.NetUnitPrice;
                /* 2013-08-26: Set VatGroupCode depending on wheater the current basket is set the apply vat. */
                orderItem.VatGroupCode = IncludeVat ? basketItem.VatGroupCode : null;

                model.OrderItems.InsertOnSubmit(orderItem);
                index++;
            }

            model.SubmitChanges();

            foreach (BasketItem basketItem in Items)
            {
                if (basketItem is ItemBasketItem)
                {
                    var item = (from i in model.Items where i.ItemNo == ((ItemBasketItem)basketItem).ItemNo select i).Single();

                    /* 2013-10-24: Update Availability on Variant Tree */
                    item.UpdateAvailability(model);
                }
            }

            model.SubmitChanges();
        }
    }
}
