using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Compilation;
using System.Web.UI;

namespace nCode.Catalog.Delivery.Weight
{
    /// <summary>
    /// Represents Delivery Provider, that calculates Freight Amount based on the Weight of the Items.
    /// </summary>
    public class WeightDeliveryProvider : DeliveryProvider
    {
        /// <summary>
        /// Gets the Code of the Vat Group used for the Freight Amount.
        /// </summary>
        public override string GetFreightVatGroupCode(CatalogModel model, Basket basket, DeliveryType deliveryType)
        {
            return deliveryType.GetProperty<string>(model, "VatGroup", null);
        }

        /// <summary>
        /// Gets the Freight Amount.
        /// </summary>
        public override decimal GetFreightAmount(CatalogModel model, Basket basket, DeliveryType deliveryType)
        {
            var freightFreeAmount = GetFreightFreeAmount(model, deliveryType);
            if (freightFreeAmount != null)
            {
                var freightFreeCurrentcy = GetFreightFreeCurrency(model, deliveryType);

                if (freightFreeAmount <= CurrencyController.ConvertAmount(basket.ItemsTotal, basket.CurrencyCode, freightFreeCurrentcy))
                    return 0m;
            }

            var weightIntervals = GetWeightIntervals(model, deliveryType);
            var weightInterval = (from w in weightIntervals
                                  where w.FromWeight <= basket.WeightTotal && basket.WeightTotal < w.ToWeight
                                  select w).FirstOrDefault();

            if (weightInterval == null)
                return 0m;

            var price = (from p in weightInterval.Prices
                         where p.CurrencyCode == basket.CurrencyCode
                         select p).SingleOrDefault();

            if (price != null)
                return price.Price;

            var defaultCurrency = model.Currencies.Where(x => x.IsDefault).SingleOrDefault();

            if (defaultCurrency == null)
                return 0m;

            price = (from p in weightInterval.Prices
                     where p.CurrencyCode == defaultCurrency.Code
                     select p).SingleOrDefault();

            if (price != null)
                return CurrencyController.ConvertAmount(price.Price, defaultCurrency, basket.Currency);

            return 0m;
        }

        /// <summary>
        /// Determines whether the specified Delivery Type (provided by this provider) is available.
        /// </summary>
        public override bool IsAvailable(CatalogModel model, Basket basket, DeliveryType deliveryType)
        {
            List<string> supportedCountryCodes = deliveryType.GetProperty<List<string>>(model, "SupportedCountries", null);

            if (supportedCountryCodes == null)
                return false;

            return supportedCountryCodes.Contains(basket.UseShippingAddress ? basket.ShippingCountryCode : basket.CountryCode);
        }

        /// <summary>
        /// Gets a IDeliveryTypeSetupControl to configure types provided by this Delivery Provider.
        /// </summary>
        /// <returns></returns>
        public override IDeliveryTypeSetupControl GetDeliveryTypeSetupControl()
        {
            return (IDeliveryTypeSetupControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/Delivery/Weight/DeliveryTypeSetupControl.ascx", typeof(Control));
        }

        /// <summary>
        /// Gets the Code of the Vat Group used for the given Delivery Type.
        /// </summary>
        public string GetVatGroupCode(CatalogModel model, DeliveryType deliveryType)
        {
            return deliveryType.GetProperty<string>(model, "VatGroup", null);
        }

        /// <summary>
        /// Sets the Code of the Vat Group used for the given Delivery Type.
        /// </summary>
        public void SetVatGroupCode(CatalogModel model, DeliveryType deliveryType, string vatGroupCode)
        {
            deliveryType.SetProperty<string>(model, "VatGroup", vatGroupCode);
        }

        /// <summary>
        /// Gets the list of Supported Country Codes for the given Delivery Type.
        /// </summary>
        public IEnumerable<string> GetSupportedCountryCodes(CatalogModel model, DeliveryType deliveryType)
        {
            return deliveryType.GetProperty<List<string>>(model, "SupportedCountries", null);
        }

        /// <summary>
        /// Sets the list of Supported Country Codes for the given Delivery Type.
        /// </summary>
        public void SetSupportedCountryCodes(CatalogModel model, DeliveryType deliveryType, IEnumerable<string> supportedCountrieCodes)
        {
            deliveryType.SetProperty<List<string>>(model, "SupportedCountries", supportedCountrieCodes.ToList());
        }

        /// <summary>
        /// Gets the list of Weight Intervals for the given Delivery Type.
        /// </summary>
        public IEnumerable<WeightInterval> GetWeightIntervals(CatalogModel model, DeliveryType deliveryType)
        {
            return deliveryType.GetProperty<List<WeightInterval>>(model, "WeightIntervals", null);
        }

        /// <summary>
        /// Sets the list of Weight Intervals for the given Delivery Type.
        /// </summary>
        public void SetWeightIntervals(CatalogModel model, DeliveryType deliveryType, IEnumerable<WeightInterval> weightIntervals)
        {
            deliveryType.SetProperty<List<WeightInterval>>(model, "WeightIntervals", weightIntervals.ToList());
        }

        public decimal? GetFreightFreeAmount(CatalogModel model, DeliveryType deliveryType)
        {
            return deliveryType.GetProperty<decimal?>(model, "FreightFreeAmount", null);
        }

        public void SetFreightFreeAmount(CatalogModel model, DeliveryType deliveryType, decimal? freightFreeAmount)
        {
            deliveryType.SetProperty<decimal?>(model, "FreightFreeAmount", freightFreeAmount);
        }

        public string GetFreightFreeCurrency(CatalogModel model, DeliveryType deliveryType)
        {
            return deliveryType.GetProperty<string>(model, "FreightFreeCurrency", null);
        }

        public void SetFreightFreeCurrency(CatalogModel model, DeliveryType deliveryType, string freightFreeCurrency)
        {
            deliveryType.SetProperty<string>(model, "FreightFreeCurrency", freightFreeCurrency);
        }

    }
}
