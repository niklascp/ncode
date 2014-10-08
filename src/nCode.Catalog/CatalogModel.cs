namespace nCode.Catalog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.Web;

    using nCode.Metadata;
    using nCode.Data.Linq2Sql;    
    using nCode.Imaging;
    using nCode.Templating;

    using nCode.Catalog.Delivery;
    using nCode.Catalog.Payment;

    using nCode.Geographics;
    using nCode.Geographics.GsApi;
    using nCode.Catalog.Model;

    /// <summary>
    /// Represents a Category.
    /// </summary>
    partial class Category : IMetadataContext
    {
        private static Guid objectTypeId = new Guid("d02e94d7-7cc7-4a0c-8bb7-86ae40e053ba");

        /// <summary>
        /// Gets the object type unique identifier.
        /// </summary>
        /// <value>
        /// The object type unique identifier.
        /// </value>
        public static Guid ObjectTypeId { get { return objectTypeId; } }

        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(objectTypeId, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(objectTypeId, ID, key, value);
        }
    }

    /// <summary>
    /// Represents a Brand.
    /// </summary>
    partial class Brand : IMetadataContext
    {
        private static Guid objectTypeId = new Guid("34054c40-98fb-40b3-8755-000aee62e5b0");

        /// <summary>
        /// Gets the object type unique identifier.
        /// </summary>
        /// <value>
        /// The object type unique identifier.
        /// </value>
        public static Guid ObjectTypeId { get { return objectTypeId; } }

        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(objectTypeId, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(objectTypeId, ID, key, value);
        }
    }

    /// <summary>
    /// Represents an Item.
    /// </summary>
    partial class Item : IMetadataContext
    {
        private static Guid objectTypeId = new Guid("2bdf6cd7-7e28-4447-a577-6e5d95363312");

        /// <summary>
        /// Gets the object type unique identifier.
        /// </summary>
        /// <value>
        /// The object type unique identifier.
        /// </value>
        public static Guid ObjectTypeId { get { return objectTypeId; } }

        public static T GetProperty<T>(Guid id, string key, T defaultValue)
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                var property = (from p in model.ItemProperties
                                where p.ItemID == id && p.Key == key
                                select p.Value).SingleOrDefault();

                if (property == null)
                    return defaultValue;

                /* Copy the string data to a Memory Stream. */
                using (MemoryStream ms = new MemoryStream())
                {
                    StreamWriter sw = new StreamWriter(ms);
                    sw.Write(property);
                    sw.Flush();

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(ms);
                }
            }
        }

        public static void SetProperty<T>(Guid id, string key, T value)
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                /* Serialize the data and copy to string. */
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(ms, value);

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    using (StreamReader sr = new StreamReader(ms))
                    {
                        var property = (from p in model.ItemProperties
                                        where p.ItemID == id && p.Key == key
                                        select p).SingleOrDefault();

                        /* Property does not exists. */
                        if (property == null)
                        {
                            property = new ItemProperty();
                            property.ID = Guid.NewGuid();
                            property.ItemID = id;
                            property.Key = key;
                            model.ItemProperties.InsertOnSubmit(property);
                        }

                        property.Value = sr.ReadToEnd();

                        model.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates StockQuantity and Availability for Items with Variants. This method should be called each time 
        /// a change is made to the StockQuantity, ReservedQuantity an Item Variants. The changes are not committed to the model.
        /// <remarks>Calling this method on Items without Variants has no effect.</remarks>
        /// </summary>
        public void UpdateAvailability(CatalogModel catalogModel)
        {
            if (this.VariantMode != VariantMode.Dependent)
                return;

            var ivts = catalogModel.ItemVariantTypes.Where(x => x.ItemID == ID).OrderBy(x => x.Depth).ToList();
            var ivs = ivts.SelectMany(x => catalogModel.ItemVariants.Where(y => y.ItemVariantTypeID == x.ID)).ToList();

            /* Start at leaf nodes, and work backwards */
            for (var i = ivts.Count() - 1; i >= 0; i--)
            {
                /* Leaf - Just update total quantities */
                if (i == ivts.Count() - 1)
                {
                    foreach (var iv in ivs.Where(y => y.ItemVariantTypeID == ivts[i].ID))
                        iv.IsLeaf = true;

                    StockQuantity = ivs.Where(y => y.ItemVariantTypeID == ivts[i].ID).Sum(x => (int?)x.StockQuantity) ?? 0;
                    ReservedQuantity = ivs.Where(y => y.ItemVariantTypeID == ivts[i].ID).Sum(x => (int?)x.ReservedQuantity) ?? 0;
                }
                /* Non-leaf - Update IsSoldOut of ItemVariants to hold negated IsAvailable flag. */
                else
                {
                    foreach (var iv in ivs.Where(y => y.ItemVariantTypeID == ivts[i].ID)) 
                    {
                        iv.IsLeaf = false;
                        iv.IsSoldOut = !ivs.Where(y => y.ParentID == iv.ID).Any(x => x.IsLeaf && x.IsAvailable || !x.IsLeaf && !x.IsSoldOut);
                    }
                }

                /* Root - Update IsSoldOut of Item to hold negated IsAvailable flag. */
                if (i == 0)
                {
                    IsSoldOut = !ivs.Where(y => y.ParentID == null).Any(x => x.IsLeaf && x.IsAvailable || !x.IsLeaf && !x.IsSoldOut);
                }
            }
        }

        /// <summary>
        /// This method removes any ophen Item Variant for this Item. This is a utility method implemented since the data model does not 
        /// enforce the relationship, since it would create multiple cascading paths. The changes are not committed to the model.
        /// <remarks>Calling this method on Items without Variants has no effect.</remarks>
        /// </summary>
        public void DeleteOrphenItemVariants(CatalogModel catalogModel)
        {
            if (VariantMode != VariantMode.Dependent)
                return;

            foreach (var vt in this.ItemVariantTypes)
            {
                var ivs = vt.ItemVariants.Where(x => x.ParentID != null && !catalogModel.ItemVariants.Any(y => y.ID == x.ParentID));
                catalogModel.ItemVariants.DeleteAllOnSubmit(ivs);
            }
        }

        public int Available {
            get 
            {
                return StockQuantity - ReservedQuantity; 
            }
        }

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

        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GetProperty<T>(ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            SetProperty<T>(ID, key, value);
        }
    }

    /// <summary>
    /// Represents an Item Price.
    /// </summary>
    partial class ItemPrice
    {
        public override string ToString()
        {
            return string.Format("{0} {1:n2} ({2})", CurrencyCode, Price, PriceGroupCode ?? "(Standard)");
        }
    }

    /// <summary>
    /// Represents an Item Variant.
    /// </summary>
    partial class ItemVariant
    {
        public int Available
        {
            get
            {
                return StockQuantity - ReservedQuantity;
            }
        }

        public bool IsAvailable
        {
            get
            {
                /* This is not a leaf - simple return static value represented by IsSoldOut. */
                if (!IsLeaf)
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
                else if (this.ItemVariantType.Item.UseStockControl)
                {
                    /* 2010-08-22: If we Allow Backorder, ignore Item stock check. */
                    return (SalesSettings.AllowBackorder || Available > 0);
                }

                return true;
            }
        }
    }

    /// <summary>
    /// Represents a Delivery Type.
    /// </summary>
    partial class DeliveryType
    {
        private static object coconut = new object();

        public static T GetProperty<T>(CatalogModel model, Guid id, string key, T defaultValue)
        {
            lock (coconut)
            {
                var property = (from p in model.DeliveryTypeProperties
                                where p.DeliveryTypeID == id && p.Key == key
                                select p.Value).SingleOrDefault();

                if (property == null)
                    return defaultValue;

                /* Copy the string data to a Memory Stream. */
                using (MemoryStream ms = new MemoryStream())
                {
                    StreamWriter sw = new StreamWriter(ms);
                    sw.Write(property);
                    sw.Flush();

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(ms);
                }
            }
        }

        public static void SetProperty<T>(CatalogModel model, Guid id, string key, T value)
        {
            lock (coconut) 
            {
                /* Serialize the data and copy to string. */
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(ms, value);

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    using (StreamReader sr = new StreamReader(ms))
                    {
                        var property = (from p in model.DeliveryTypeProperties
                                        where p.DeliveryTypeID == id && p.Key == key
                                        select p).SingleOrDefault();

                        /* Property does not exists. */
                        if (property == null)
                        {
                            property = new DeliveryTypeProperty();
                            property.ID = Guid.NewGuid();
                            property.DeliveryTypeID = id;
                            property.Key = key;
                            model.DeliveryTypeProperties.InsertOnSubmit(property);
                        }

                        property.Value = sr.ReadToEnd();
                    }
                }
            }
        }

        public T GetProperty<T>(CatalogModel model, string key, T defaultValue)
        {
            return GetProperty<T>(model, ID, key, defaultValue);
        }

        public void SetProperty<T>(CatalogModel model, string key, T value)
        {
            SetProperty<T>(model, ID, key, value);
        }

        public DeliveryProvider Provider
        {
            get { return DeliveryManager.Providers[ProviderName]; }
        }
    }

    /// <summary>
    /// Represents a Payment Type.
    /// </summary>
    partial class PaymentType : IMetadataContext
    {
        private static object coconut = new object();

        public static T GetProperty<T>(CatalogModel model, Guid id, string key, T defaultValue)
        {
            lock (coconut)
            {
                var property = (from p in model.PaymentTypeProperties
                                where p.PaymentTypeID == id && p.Key == key
                                select p.Value).SingleOrDefault();

                if (property == null)
                    return defaultValue;

                /* Copy the string data to a Memory Stream. */
                using (MemoryStream ms = new MemoryStream())
                {
                    StreamWriter sw = new StreamWriter(ms);
                    sw.Write(property);
                    sw.Flush();

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(ms);
                }
            }
        }

        public static void SetProperty<T>(CatalogModel model, Guid id, string key, T value)
        {
            lock (coconut)
            {
                /* Serialize the data and copy to string. */
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(ms, value);

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    using (StreamReader sr = new StreamReader(ms))
                    {
                        var property = (from p in model.PaymentTypeProperties
                                        where p.PaymentTypeID == id && p.Key == key
                                        select p).SingleOrDefault();

                        /* Property does not exists. */
                        if (property == null)
                        {
                            property = new PaymentTypeProperty();
                            property.ID = Guid.NewGuid();
                            property.PaymentTypeID = id;
                            property.Key = key;
                            model.PaymentTypeProperties.InsertOnSubmit(property);
                        }

                        property.Value = sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Metadata Property given by the key from the model, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(CatalogModel model, string key, T defaultValue)
        {
            return GetProperty<T>(model, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the key in the model to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(CatalogModel model, string key, T value)
        {
            SetProperty<T>(model, ID, key, value);
        }

        /// <summary>
        /// Gets the Metadata Property given by the key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            using (CatalogModel model = new CatalogModel())
            {
                return GetProperty<T>(model, key, defaultValue);
            }
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            using (CatalogModel model = new CatalogModel())
            {
                SetProperty<T>(model, key, value);
                model.SubmitChanges();
            }
        }

        public PaymentProvider Provider
        {
            get { return PaymentManager.Providers[ProviderName]; }
        }
    }

    partial class CatalogModel
    {
        public CatalogModel()
            : base(SqlUtilities.ConnectionString)
        { }
    }

    /// <summary>
    /// Represents an Order. 
    /// </summary>
    partial class Order : IMetadataContext
    {
        private static Guid objectTypeId = new Guid("33c92b8d-b993-451b-94ef-7975da3f3d87");

        /// <summary>
        /// Gets the object type unique identifier.
        /// </summary>
        /// <value>
        /// The object type unique identifier.
        /// </value>
        public static Guid ObjectTypeId { get { return objectTypeId; } }

        private IEnumerable<XElement> GetPropertyElements()
        {
            return GenericMetadataHelper.GetProperties(objectTypeId, ID);
        }

        private decimal GetNetAmount(decimal amount, string vatGroupCode)
        {
            if (!VatIncluded)
                return amount;
            else
                return VatUtilities.GetNetAmount(amount, vatGroupCode);
        }

        private decimal GetNetAmount(decimal amount, VatGroup vatGroup)
        {
            if (!VatIncluded)
                return amount;
            else
                return VatUtilities.GetNetAmount(amount, vatGroup);
        }

        private decimal GetAmount(decimal amount, string vatGroupCode)
        {
            if (VatIncluded)
                return amount;
            else
                return VatUtilities.GetAmount(amount, vatGroupCode);
        }

        private decimal GetAmount(decimal amount, VatGroup vatGroup)
        {
            if (VatIncluded)
                return amount;
            else
                return VatUtilities.GetAmount(amount, vatGroup);
        }


        /// <summary>
        /// Updates the totals.
        /// </summary>
        public void UpdateTotals()
        {
            /* Sum over items. */
            NetTotal = Items.Sum(i => i.Qty * GetNetAmount(i.UnitPrice, i.VatGroupCode));
            Total = Items.Sum(i => i.Qty * GetAmount(i.UnitPrice, i.VatGroupCode));

            NetTotal -= GetNetAmount(DiscountAmount, VatUtilities.DefaultVatGroup); /* TODO: DiscountAmount should really have its own vat group property! This is just a quick fix. */
            Total -= GetAmount(DiscountAmount, VatUtilities.DefaultVatGroup);

            NetTotal += GetNetAmount(FreightAmount, FreightVatGroupCode);
            Total += GetAmount(FreightAmount, FreightVatGroupCode);

            NetTotal += GetNetAmount(PaymentFeeAmount, PaymentFeeVatGroupCode);
            Total += GetAmount(PaymentFeeAmount, PaymentFeeVatGroupCode);
        }

        /// <summary>
        /// Gets the Metadata Property identified by the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The value.</param>
        /// <returns></returns>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(objectTypeId, ID, key, defaultValue);
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
            GenericMetadataHelper.SetProperty<T>(objectTypeId, ID, key, value);
        }

        /// <summary>
        /// Converts this order to an invoice.
        /// </summary>
        public void ConvertToInvoice(CatalogModel model, string invoiceNo)
        {
            Log.Info(string.Format("Converting Order ({0}) to Status: Invoice.", OrderNo));

            if (Status != OrderStatus.Order)
                throw new InvalidOperationException("Only Orders can be converted to an Invoice.");
            
            InvoiceNo = invoiceNo;
            InvoiceDate = DateTime.Today;

            /* Set DueDate */
            var paymentType = model.PaymentTypes.SingleOrDefault(x => x.Code == PaymentTypeCode);
            if (paymentType != null && PaymentManager.Providers[paymentType.ProviderName] != null)
                DueDate = PaymentManager.Providers[paymentType.ProviderName].GetDueDate(model, this, paymentType);

            Log.Info(string.Format("Calculated Due Date: {0} for Order ({1}).", DueDate, OrderNo));

            Status = OrderStatus.Invoice;
            
            foreach (var oi in Items)
            {
                var item = model.Items.SingleOrDefault(x => x.ItemNo == oi.ItemNo);

                if (SalesSettings.StockControlLevel == StockControlLevel.Normal || SalesSettings.StockControlLevel == StockControlLevel.Advanced)
                {
                    /* Update Item Stock */
                    if (item != null && item.UseStockControl)
                    {
                        Log.Info(string.Format("Calculating Stock Level for Item {0}: Current State: ({1}, {2}).", item.ItemNo, item.StockQuantity, item.ReservedQuantity));

                        item.StockQuantity -= oi.Qty;
                        item.ReservedQuantity -= oi.Qty;

                        if (oi.ItemVariantID != null)
                        {
                            var itemVariant = model.ItemVariants.SingleOrDefault(x => x.ID == oi.ItemVariantID);
                            if (itemVariant != null)
                            {
                                itemVariant.StockQuantity -= oi.Qty;
                                itemVariant.ReservedQuantity -= oi.Qty;
                            }
                        }

                        Log.Info(string.Format("Calculated Stock Level for Item {0}: New State: ({1}, {2}).", item.ItemNo, item.StockQuantity, item.ReservedQuantity));

                        if (SalesSettings.StockControlLevel == StockControlLevel.Advanced)
                        {
                            /* Add Stock Transaction */
                            var stockTransaction = new StockTransaction();
                            stockTransaction.ItemNo = item.ItemNo;
                            stockTransaction.OrderNo = OrderNo;
                            stockTransaction.Date = InvoiceDate.Value;
                            stockTransaction.Qty = -oi.Qty;
                            model.StockTransactions.InsertOnSubmit(stockTransaction);
                        }
                    }
                }
            }

            try
            {
                Log.Info(string.Format("Updating Location Data for Order ({0}).", OrderNo));
                UpdateLocationData(model);
                Log.Info(string.Format("Updated Location Data for Order ({0}).", OrderNo));
            }
            catch (Exception ex) {
                Log.Info(string.Format("Failed to Update Location Data for Order ({0}).", OrderNo), ex);
            }
        }

        /// <summary>
        /// Converts this order to an invoice.
        /// </summary>
        public void ConvertToInvoice(CatalogModel model)
        {
            var invoiceNo = NoSerieNumber.GetNext("Invoice").ToString();
            ConvertToInvoice(model, invoiceNo);
        }

        /// <summary>
        /// Converts this order to an invoice.
        /// </summary>
        public void ConvertToDeleted(CatalogModel model)
        {
            Log.Info(string.Format("Converting Order ({0}) to Status: Deleted.", OrderNo));

            if (Status != OrderStatus.Order)
                throw new InvalidOperationException("Only Orders can be deleted.");

            Status = OrderStatus.Deleted;

            foreach (var oi in Items)
            {
                var item = model.Items.SingleOrDefault(x => x.ItemNo == oi.ItemNo);

                if (item != null && item.UseStockControl)
                {
                    Log.Info(string.Format("Calculating Stock Level for Item {0}: Current State: ({1}, {2}).", item.ItemNo, item.StockQuantity, item.ReservedQuantity));

                    item.ReservedQuantity -= oi.Qty;

                    if (oi.ItemVariantID != null)
                    {
                        var itemVariant = model.ItemVariants.SingleOrDefault(x => x.ID == oi.ItemVariantID);
                        if (itemVariant != null)
                        {
                            itemVariant.ReservedQuantity -= oi.Qty;
                        }
                    }

                    Log.Info(string.Format("Calculated Stock Level for Item {0}: New State: ({1}, {2}).", item.ItemNo, item.StockQuantity, item.ReservedQuantity));
                }
            }

            Log.WriteEntry(EntryType.Information, "Catalog", "Delete Order", string.Format("User '{0}' deleted the order {1}.", HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null ? HttpContext.Current.User.Identity.Name : null, OrderNo));
        }

        /// <summary>
        /// Creates the credit order.
        /// </summary>
        public Order CreateCreditOrder()
        {
            Log.Info(string.Format("Creating Credit Order from Invoiced Order (OrderNo: {0}, InvoiceNo: {1}).", OrderNo, InvoiceNo));

            if (Status != OrderStatus.Invoice)
                throw new InvalidOperationException("Only Invoice can by Credited.");

            using (var model = new CatalogModel())
            {
                var creditOrder = this.Clone();
                model.Orders.InsertOnSubmit(creditOrder);

                creditOrder.ID = Guid.NewGuid();
                creditOrder.Created = DateTime.Now;
                creditOrder.Modified = DateTime.Now;
                creditOrder.Status = OrderStatus.Order;
                creditOrder.OrderNo = NoSerieNumber.GetNext("Order").ToString();
                creditOrder.InvoiceNo = null;
                creditOrder.InvoiceDate = null;
                creditOrder.PaymentStatus = Catalog.PaymentStatus.Unconfirmed;
                creditOrder.TransactionNo = null;
                creditOrder.FreightVatGroupCode = null;
                creditOrder.FreightAmount = 0;

                foreach (var oi in Items)
                {
                    var creditItem = oi.Clone();
                    creditItem.ID = Guid.NewGuid();
                    creditItem.Created = DateTime.Now;
                    creditItem.Modified = DateTime.Now;
                    creditItem.OrderNo = creditOrder.OrderNo;
                    creditItem.Qty = -creditItem.Qty;
                    model.OrderItems.InsertOnSubmit(creditItem);

                    var item = model.Items.SingleOrDefault(x => x.ItemNo == oi.ItemNo);

                    if (item != null && item.UseStockControl)
                    {
                        Log.Info(string.Format("Calculating Stock Level for Item {0}: Current State: ({1}, {2}).", item.ItemNo, item.StockQuantity, item.ReservedQuantity));

                        item.ReservedQuantity -= oi.Qty;

                        if (oi.ItemVariantID != null)
                        {
                            var itemVariant = model.ItemVariants.SingleOrDefault(x => x.ID == oi.ItemVariantID);
                            if (itemVariant != null)
                            {
                                itemVariant.ReservedQuantity -= oi.Qty;
                            }
                        }

                        Log.Info(string.Format("Calculated Stock Level for Item {0}: New State: ({1}, {2}).", item.ItemNo, item.StockQuantity, item.ReservedQuantity));
                    }
                }

                model.SubmitChanges();
                creditOrder.UpdateTotals();
                model.SubmitChanges();

                return creditOrder;
            }
        }

        /// <summary>
        /// Returns an Xml Representation of this order.
        /// </summary>
        public XDocument AsXDocument(Func<OrderItem,bool> itemFilter = null)
        {
            if (itemFilter == null)
                itemFilter = _ => true;

            UpdateTotals();

            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                var pt = model.PaymentTypes.SingleOrDefault(x => x.Code == PaymentTypeCode);
                var dt = model.DeliveryTypes.SingleOrDefault(x => x.Code == DeliveryTypeCode);

                /* Build XmlData */
                var data = new XDocument(
                    new XElement("Order",
                        new XElement("Created", Created),
                        new XElement("OrderNo", OrderNo),
                        new XElement("DeliveryDate", DeliveryDate),
                        new XElement("InvoiceNo", InvoiceNo),
                        new XElement("InvoiceDate", InvoiceDate),
                        new XElement("DueDate", DueDate),

                        new XElement("CustomerNo", CustomerNo),
                        new XElement("Name", Name),
                        new XElement("Address1", Address1),
                        new XElement("Address2", Address2),
                        new XElement("PostalCode", PostalCode),
                        new XElement("City", City),
                        new XElement("Country", (from c in model.Countries where c.Code == CountryCode select c.Name).SingleOrDefault()),
                        new XElement("Phone", Phone),
                        new XElement("Email", Email),
                        new XElement("CustomerReference", CustomerReference),
                        new XElement("CustomerEan", CustomerEan),

                        new XElement("UseShippingAddress", UseShippingAddress),
                        new XElement("ShippingName", ShippingName),
                        new XElement("ShippingAddress1", ShippingAddress1),
                        new XElement("ShippingAddress2", ShippingAddress2),
                        new XElement("ShippingPostalCode", ShippingPostalCode),
                        new XElement("ShippingCity", ShippingCity),
                        new XElement("ShippingCountry", UseShippingAddress ? (from c in model.Countries where c.Code == ShippingCountryCode select c.Name).SingleOrDefault() : null),

                        new XElement("VatIncluded", VatIncluded),
                        new XElement("CurrencyCode", CurrencyCode),
                        new XElement("DefaultVatRate", VatUtilities.DefaultVatGroup.Rate),

                        new XElement("Items", from i in Items.OrderBy(x => x.DisplayIndex).ToList().Where(itemFilter)
                                              select new XElement("Item",
                                                  new XElement("ImageFile", LayoutSettings.NoImageImageFile != null ? (Settings.Url.TrimEnd(new char[] { '/' }) + ImageUtilities.EnsureImageSize(i.ImageFile ?? LayoutSettings.NoImageImageFile, 100, 80)) : null),
                                                  new XElement("ItemNo", i.ItemNo),
                                                  new XElement("Title", i.Title),
                                                  new XElement("Qty", i.Qty),
                                                  new XElement("UnitPrice", i.UnitPrice),
                                                  new XElement("Price", i.Qty * i.UnitPrice),
                                                  new XElement("Properties", i.GetPropertyElements())
                                              )),                        

                        new XElement("PaymentTypeCode", PaymentTypeCode),
                        new XElement("PaymentTypeText", pt != null ? pt.Provider.GetOrderString(model, this, pt) : null),
                        new XElement("PaymentInstructions", pt != null ? pt.Provider.GetIntructionString(model, this, pt) : null),
                        new XElement("DeliveryTypeCode", DeliveryTypeCode),
                        new XElement("DeliveryTypeText", dt != null ? dt.Provider.GetOrderString(model, this,  dt) : null),

                        /* We might filter on the position of the item, so we also need this order when we are summing. */
                        new XElement("ItemsTotal", Items.OrderBy(x => x.DisplayIndex).ToList().Where(itemFilter).Sum(x => x.Qty * x.UnitPrice)),
                        new XElement("DiscountAmount", DiscountAmount),
                        new XElement("FreightAmount", FreightAmount),                       
                        
                        /* TODO: Include Vat Specification */
                                                                      
                        new XElement("VatAmount", Total - NetTotal),
                        new XElement("NetTotal", NetTotal),
                        new XElement("Total", Total),

                        new XElement("Comment", Comment),

                        CompanyInformation.AsXElement(),
                        
                        new XElement("Settings",                            
                            new XElement("ShowCustomerNo", SalesSettings.ShowCustomerNo),
                            new XElement("ShowCustomerEan", SalesSettings.ShowCustomerEan),
                            new XElement("ShowCustomerReference", SalesSettings.ShowCustomerReference),
                            new XElement("ShowOrderItemImage", LayoutSettings.ShowOrderItemImage)),
                        
                        new XElement("Properties", GetPropertyElements())
                    )
                );

                return data;
            }
        }

        /// <summary>
        /// Sends an order confirmation confirmation.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="templateSearchPath">The template search path.</param>
        /// <param name="itemFilter">The item filter.</param>
        public void SendConfirmation(string templateName, string[] templateSearchPath, Func<OrderItem, bool> itemFilter = null)
        {
            try
            {
                XDocument templateData = AsXDocument(itemFilter);
                XsltTemplate template = XsltTemplate.Load(templateName, templateSearchPath, Culture);

                MailMessage mailMessage = template.TransformToMailMessage(templateData);
                mailMessage.From = new MailAddress(Settings.SenderAddress, Settings.SenderName);
                mailMessage.To.Add(new MailAddress(Email, Name));

                if (!string.IsNullOrEmpty(SalesSettings.OrderEmail))
                    mailMessage.Bcc.Add(SalesSettings.OrderEmail);

                mailMessage.Bcc.Add("hr@cmspartner.dk");
                mailMessage.Bcc.Add("ncp@ncode.dk");

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Log.WriteEntry(EntryType.Error, "Catalog", "Send Order Confirmation", exception: ex);
            }
        }

        /// <summary>
        /// Updates/creates GIS location entries for this order.
        /// </summary>
        public void UpdateLocationData(CatalogModel model)
        {
            var customerLocation = model.OrderLocations.SingleOrDefault(x => x.OrderNo == OrderNo && x.LocationCode == "BILL");

            // Todo: Parameter that specifies that the information should be updatet?
            if (customerLocation == null && CountryCode == "DK")
            {
                Log.Info(string.Format("Extracting Address Information for Order ({0}): {1}", OrderNo, Address1));
                var address = AddressUtilities.ExtractStreetAddress(Address1);

                if (address == null)
                {
                    Log.Info(string.Format("Extracting Address Information for Order ({0}): {1}", OrderNo, Address2));
                    address = AddressUtilities.ExtractStreetAddress(Address2);
                }

                if (address != null)
                {
                    Log.Info(string.Format("Extracted Address Information for Order ({0}): {1}", OrderNo, address));
                    
                    var res = GsApi.LookupAddress(address, PostalCode);

                    if (res != null)
                    {
                        // Todo: Parameter that specifies that the information should be updatet?
                        //if (customerLocation == null)
                        //{
                        customerLocation = new OrderLocation();
                        customerLocation.ID = Guid.NewGuid();
                        customerLocation.OrderNo = OrderNo;
                        customerLocation.LocationCode = "BILL";
                        model.OrderLocations.InsertOnSubmit(customerLocation);
                        //}

                        if (res.Municipality != null)
                            customerLocation.MunicipalityCode = res.Municipality.Code;
                        if (res.Region != null)
                            customerLocation.RegionCode = res.Region.No;
                        if (res.Wgs84Coordinate != null)
                        {
                            customerLocation.Latitude = res.Wgs84Coordinate.Latitude;
                            customerLocation.Longitude = res.Wgs84Coordinate.Longitude;
                        }
                    }
                    else
                    {
                        Log.WriteEntry(EntryType.Information, "Catalog", "Lookup Address " + OrderNo + ": " + address.StreetName + " - " + address.HouseNo + " - " + PostalCode);
                    }
                }
                else
                {
                    Log.WriteEntry(EntryType.Information, "Catalog", "Extract Address " + OrderNo + ": " + Address1 + " - " + Address2);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether payment is confirmed.
        /// </summary>
        [Obsolete("Use PaymentStatus")]
        public bool PaymentConfirmed
        {
            get
            {
                return PaymentStatus == PaymentStatus.Confirmed || PaymentStatus == PaymentStatus.Completed;
            }
            set
            {
                PaymentStatus = value ? PaymentStatus.Confirmed : PaymentStatus.Unconfirmed;
            }
        }
    }

    /// <summary>
    /// Represents an Order Item.
    /// </summary>
    partial class OrderItem
    {
        internal IList<XElement> GetPropertyElements()
        {
            List<XElement> elements = new List<XElement>();

            foreach (string key in properties.Keys)
            {
                var element = new XElement(key);
                object value = properties[key];
                element.Value = value.ToString();
                elements.Add(element);
            }

            return elements;
        }

        /* Holds additional user-defined properties. ToDo: Move into database. */
        private Dictionary<string, object> properties = new Dictionary<string, object>();

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
        /// <param name="value">The default value.</param>
        /// <returns></returns>
        public void SetProperty<T>(string key, T value)
        {
            if (properties.ContainsKey(key))
                properties[key] = value;
            else
                properties.Add(key, value);
        }
    }

    /// <summary>
    /// Represents a Discount Code.
    /// </summary>
    partial class DiscountCode
    {
        /// <summary>
        /// Gets a value indicating whether this Discount Code is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Applied < Issued &&
                    (ValidFrom == null || ValidFrom <= DateTime.Today) &&
                    (ValidTo == null || DateTime.Today <= ValidTo);
            }
        }
    }
}
