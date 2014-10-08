using System;

namespace nCode.Catalog
{
    /// <summary>
    /// Represents the type of a Discount.
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// The discount is a Fixed Amount.
        /// </summary>
        FixedAmount,

        /// <summary>
        /// The discount is a Percentage of the Items Total.
        /// </summary>
        Procentage
    }
}