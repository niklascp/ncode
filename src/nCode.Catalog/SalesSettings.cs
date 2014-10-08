using nCode.Catalog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog
{
    /// <summary>
    /// Settings for sales made through the Catalog Module.
    /// </summary>
    public static class SalesSettings
    {
        private const string activateSalesFeaturesKey = "nCode.Catalog.Sales.ActivateSalesFeatures";
        private const string activateSalesStatisticsKey = "nCode.Catalog.Sales.ActivateSalesStatistics";        
        private const string allowBackorderKey = "nCode.Catalog.Sales.AllowBackorder";
        private const string allowDiscountCodesKey = "nCode.Catalog.Sales.AllowDiscountCodes";
        private const string allowShippingAddressKey = "nCode.Catalog.Sales.AllowShippingAddress";
        private const string orderEmailKey = "nCode.Catalog.Sales.OrderEmail";
        private const string stockControlLevelKey = "nCode.Catalog.Sales.StockControlLevel";
        private const string useInvoicingKey = "nCode.Catalog.Sales.UseInvoicing";
        private const string askForInvoiceNoKey = "nCode.Catalog.Sales.AskForInvoiceNo";      

        private const string showCustomerNoKey = "nCode.Catalog.Sales.ShowCustomerNo";
        private const string showCustomerEanKey = "nCode.Catalog.Sales.ShowCustomerEan";        
        private const string showCustomerReferenceKey = "nCode.Catalog.Sales.ShowCustomerReference";

        /* Sale */
        /// <summary>
        /// Gets the default sales channel.
        /// </summary>
        /// <value>
        /// The default sales channel.
        /// </value>
        public static SalesChannel DefaultSalesChannel
        {
            get
            {
                using (var db = new CatalogDbContext())
                {
                    return db.SalesChannels.Find(1);
                }
            }
        }

        /// <summary>
        /// Gets the default sales channel.
        /// </summary>
        /// <value>
        /// The default sales channel.
        /// </value>
        public static Model.PriceGroup DefaultPriceGroup
        {
            get
            {
                using (var db = new CatalogDbContext())
                {
                    return db.PriceGroups.Where(x => x.Code == null).SingleOrDefault();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating wheather Sales features such as order is activated.
        /// Feature access control cache should be recalculated after setting this values.
        /// </summary>
        public static bool ActivateSalesFeatures
        {
            get { return Settings.GetProperty<bool>(activateSalesFeaturesKey, true); }
            set { Settings.SetProperty<bool>(activateSalesFeaturesKey, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating wheather Sales statistics is activated.
        /// Feature access control cache should be recalculated after setting this values.
        /// </summary>
        public static bool ActivateSalesStatistics
        {
            get { return Settings.GetProperty<bool>(activateSalesStatisticsKey, true); }
            set { Settings.SetProperty<bool>(activateSalesStatisticsKey, value); }
        }

        /// <summary>
        /// True if Customers should be able to add items to basket even though they are not
        /// available in the desired qty.
        /// </summary>
        public static bool AllowBackorder
        {
            get { return Settings.GetProperty<bool>(allowBackorderKey, false); }
            set { Settings.SetProperty<bool>(allowBackorderKey, value); }
        }

        /// <summary>
        /// True if Customers should be able to enter an alternative shipment address.
        /// </summary>
        public static bool AllowShippingAddress
        {
            get { return Settings.GetProperty<bool>(allowShippingAddressKey, true); }
            set { Settings.SetProperty<bool>(allowShippingAddressKey, value); }
        }

        /// <summary>
        /// True if Customers should be able to enter Discount Codes.
        /// </summary>
        public static bool AllowDiscountCodes
        {
            get { return Settings.GetProperty<bool>(allowDiscountCodesKey, true); }
            set { Settings.SetProperty<bool>(allowDiscountCodesKey, value); }
        }

        /* Prices */

        /// <summary>
        /// True if the entered Prices includes vat.
        /// </summary>
        [Obsolete("Use PricesIncludeVat on the current Price Group")]
        public static bool PricesIncludesVat
        {
            get { return DefaultPriceGroup.PricesIncludeVat; }
        }

        /// <summary>
        /// True if the prices on front end should be shown including vat.
        /// </summary>
        [Obsolete("Use ShowPricesIncludingVat on the current SalesChannel")]
        public static bool ShowPricesIncludingVat
        {
            get { return DefaultSalesChannel.ShowPricesIncludingVat; }
            //get { return Settings.GetProperty<bool>(showPricesIncludingVatKey, true); }
            //set { Settings.SetProperty<bool>(showPricesIncludingVatKey, value); }
        }

        /* Stock */

        /// <summary>
        /// True if Simple Stock Control is activated. Simple Stock Control allows a Item to 
        /// be either In Stock ore not without having actual Stock Count.
        /// </summary>
        [Obsolete("Use StockControlLevel")]
        public static bool UseSimpleStockControl
        {
            get { return StockControlLevel == Catalog.StockControlLevel.Simple; }
        }

        /// <summary>
        /// Gets or sets the Stock ControlLevel. Changing this value in production can invalidate stock count.
        /// Feature access control cache should be recalculated after setting this values.
        /// </summary>
        public static StockControlLevel StockControlLevel
        {
            get { return Settings.GetProperty<StockControlLevel>(stockControlLevelKey, StockControlLevel.Normal); }
            set { Settings.SetProperty<StockControlLevel>(stockControlLevelKey, value); }
        }

        /* Orderhandling */

        /// <summary>
        /// True if the invoicing capalities is activated.
        /// </summary>
        public static bool UseInvoicing
        {
            get { return Settings.GetProperty<bool>(useInvoicingKey, true); }
            set { Settings.SetProperty<bool>(useInvoicingKey, value); }
        }

        /// <summary>
        /// True if the user should be asked for an InvoiceNo upon Invoice Creation.
        /// </summary>
        public static bool AskForInvoiceNo
        {
            get { return Settings.GetProperty<bool>(askForInvoiceNoKey, false); }
            set { Settings.SetProperty<bool>(askForInvoiceNoKey, value); }
        }

        /// <summary>
        /// Gets or sets a value indication if the CustomerNo-field should be visiable during the order process.
        /// </summary>
        public static bool ShowCustomerNo
        {
            get { return Settings.GetProperty<bool>(showCustomerNoKey, false); }
            set { Settings.SetProperty<bool>(showCustomerNoKey, value); }
        }

        /// <summary>
        /// Gets or sets a value indication if the CustomerEan-field should be visiable during the order process.
        /// </summary>
        public static bool ShowCustomerEan
        {
            get { return Settings.GetProperty<bool>(showCustomerEanKey, false); }
            set { Settings.SetProperty<bool>(showCustomerEanKey, value); }
        }

        /// <summary>
        /// Gets or sets a value indication if the CustomerReference-field should be visiable during the order process.
        /// </summary>
        public static bool ShowCustomerReference
        {
            get { return Settings.GetProperty<bool>(showCustomerReferenceKey, false); }
            set { Settings.SetProperty<bool>(showCustomerReferenceKey, value); }
        }

        /// <summary>
        /// The email address of which orders should be sent to.
        /// </summary>
        public static string OrderEmail
        {
            get { return Settings.GetProperty<string>(orderEmailKey, null); }
            set { Settings.SetProperty<string>(orderEmailKey, value); }
        }
    }


    public enum StockControlLevel
    {
        /// <summary>
        /// Simple stock control allows a item to be available or sold out.
        /// </summary>
        Simple,
        /// <summary>
        /// Normal stock control tracks in-stock qty and qty reserved for orders.
        /// </summary>
        Normal,
        /// <summary>
        /// Same as normal, but with record history of supply and consumption.
        /// </summary>
        Advanced
    }
}
