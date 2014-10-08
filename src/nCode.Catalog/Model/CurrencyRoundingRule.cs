using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Model
{
    /// <summary>
    /// Represents how Currency Rounding is performed.
    /// </summary>
    public enum CurrencyRoundingRule
    {
        /// <summary>
        /// Round to the nearest multiplicity.
        /// </summary>
        Round,

        /// <summary>
        /// Round up (away from zero) to the nearest multiplicity.
        /// </summary>
        Ceiling,

        /// <summary>
        /// Round down (towards from zero) to the nearest multiplicity.
        /// </summary>
        Floor
    }
}
