using nCode.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.Catalog
{
    public static class CurrencyController
    {
        private const string currenciesKey = "nCode.Catalog.Currencies";
        private const string defaultCurrencyKey = "nCode.Catalog.DefaultCurrency";
        private const string currentCurrencyKey = "nCode.Catalog.CurrentCurrency";

        private static object lockObject = new object();

        public static IList<Currency> Currencies
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items[currenciesKey] != null)
                    return (IList<Currency>)HttpContext.Current.Items[currenciesKey];

                lock (lockObject)
                {
                    using (CatalogModel model = new CatalogModel()) {
                        var currencies = model.Currencies.ToList();

                        if (HttpContext.Current != null && HttpContext.Current.Items[currenciesKey] == null)
                            HttpContext.Current.Items[currenciesKey] = currencies;

                        return currencies;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the current currency, or the default currency, or null if no default currentcy is set.
        /// </summary>
        public static Currency CurrentCurrency
        {
            get
            {
                lock (lockObject)
                {
                    Currency currentCurrency = HttpContext.Current.Items[currentCurrencyKey] as Currency;

                    if (currentCurrency != null)
                        return currentCurrency;

                    string currencyCode = (string)HttpContext.Current.Session[currentCurrencyKey];

                    if (currencyCode != null)
                        currentCurrency = Currencies.SingleOrDefault(x => x.Code == currencyCode);

                    if (currentCurrency == null)
                        currentCurrency = Currencies.SingleOrDefault(x => x.IsDefault);

                    HttpContext.Current.Items[currentCurrencyKey] = currentCurrency;
                    return currentCurrency;
                }
            }
        }

        /// <summary>
        /// Returns the current currency, or the default currency, or null if no default currentcy is set.
        /// </summary>
        public static Currency DefaultCurrency
        {
            get
            {
                lock (lockObject)
                {
                    Currency defaultCurrency = null;
                    
                    /* If we are serving through a HttpContext, check that wa migth have already loaded the Default Currency for this request. */
                    /* TODO: It would make more sense cahcing this across requests, since it only changes through configuration. */
                    if (HttpContext.Current != null)
                        defaultCurrency = HttpContext.Current.Items[defaultCurrencyKey] as Currency;

                    if (defaultCurrency != null)
                        return defaultCurrency;

                    defaultCurrency = Currencies.SingleOrDefault(x => x.IsDefault);

                    /* If we are serving through a HttpContext, save the default currency for this request. */
                    /* TODO: It would make more sense cahcing this across requests, since it only changes through configuration. */                   
                    if (HttpContext.Current != null)
                        HttpContext.Current.Items[defaultCurrencyKey] = defaultCurrency;

                    return defaultCurrency;
                }
            }
        }

        /// <summary>
        /// Converts the amount given amount from one currency to the current currency, without applying rounding rules.
        /// </summary>
        public static decimal ConvertAmount(decimal amount, string fromCurrencyCode)
        {
            Currency fromCurrency = Currencies.Single(c => c.Code == fromCurrencyCode);
            return ConvertAmount(amount, fromCurrency, CurrentCurrency);        
        }

        /// <summary>
        /// Converts the amount given amount from one currency to the current currency, without applying rounding rules.
        /// </summary>
        public static decimal ConvertAmount(decimal amount, Currency fromCurrency)
        {
            return ConvertAmount(amount, fromCurrency, CurrentCurrency);
        }

        /// <summary>
        /// Converts the amount given amount from one currency to other, without applying rounding rules.
        /// </summary>
        public static decimal ConvertAmount(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            if (string.Equals(fromCurrencyCode, toCurrencyCode, StringComparison.OrdinalIgnoreCase))
                return amount;

            Currency fromCurrency = Currencies.Single(c => c.Code == fromCurrencyCode);
            Currency toCurrency = Currencies.Single(c => c.Code == toCurrencyCode);
            return ConvertAmount(amount, fromCurrency, toCurrency);
        }

        /// <summary>
        /// Converts the amount given amount from one currency to other, without applying rounding rules.
        /// </summary>
        public static decimal ConvertAmount(decimal amount, Currency fromCurrency, Currency toCorrency)
        {
            if (fromCurrency == null)
                throw new ArgumentNullException("fromCurrency", "From Currency must be supplied");

            if (toCorrency == null)
                throw new ArgumentNullException("toConrrency", "To Currency must be supplied");

            return amount * fromCurrency.Rate / toCorrency.Rate;
        }

        /// <summary>
        /// Sets the currency.
        /// </summary>
        /// <param name="code">The code.</param>
        public static void SetCurrency(string code)
        {
            HttpContext.Current.Session[currentCurrencyKey] = code;
            HttpContext.Current.Items[currentCurrencyKey] = null;
        }

        /// <summary>
        /// Rounds the given price by the rules specified by the current currency.
        /// </summary>
        public static decimal ApplyRoundingRule(decimal amount)
        {
            return ApplyRoundingRule(amount, CurrentCurrency);
        }

        /// <summary>
        /// Rounds the given price by the rules specified by the given currency.
        /// </summary>
        public static decimal ApplyRoundingRule(decimal amount, Currency currency)
        {
            switch (currency.RoundingRule)
            {
                case CurrencyRoundingRule.Round:
                    return Math.Round(amount / currency.RoundingMultiple, MidpointRounding.AwayFromZero) * currency.RoundingMultiple;
                case CurrencyRoundingRule.Ceiling:
                    return Math.Ceiling(amount / currency.RoundingMultiple) * currency.RoundingMultiple;
                case CurrencyRoundingRule.Floor:
                    return Math.Floor(amount / currency.RoundingMultiple) * currency.RoundingMultiple;
                default:
                    return amount;
            }
        }

    }
}
