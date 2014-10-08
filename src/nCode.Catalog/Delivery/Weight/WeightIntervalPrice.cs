using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Delivery.Weight
{
    /// <summary>
    /// Represents a Price of a Weight Interval
    /// </summary>
    public class WeightIntervalPrice
    {
        /// <summary>
        /// Gets or sets the Currency Code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the Price.
        /// </summary>
        public decimal Price { get; set; }
    }
}
