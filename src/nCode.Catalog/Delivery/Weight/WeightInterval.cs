using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace nCode.Catalog.Delivery.Weight
{
    /// <summary>
    /// Represents a Weight Interval
    /// </summary>
    public class WeightInterval
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightInterval"/> class.
        /// </summary>
        public WeightInterval()
        {
            Prices = new List<WeightIntervalPrice>();
        }

        /// <summary>
        /// Gets or sets from weight (inclusive).
        /// </summary>
        public decimal FromWeight { get; set; }

        /// <summary>
        /// Gets or sets the to weight (exclusive).
        /// </summary>
        public decimal ToWeight { get; set; }

        /// <summary>
        /// Gets or sets the prices.
        /// </summary>
        public List<WeightIntervalPrice> Prices { get; set; }
    }
}
