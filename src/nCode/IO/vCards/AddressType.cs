using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.IO.vCards
{
    /// <summary>
    /// Represents a vCard address type.
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// A domestic address. 
        /// </summary>
        DOM,
        /// <summary>
        /// An international address. 
        /// </summary>
        INTL,
        /// <summary>
        /// A postal address.
        /// </summary>
        POSTAL,
        /// <summary>
        /// A parcel address.
        /// </summary>
        PARCEL,
        /// <summary>
        /// A home address.
        /// </summary>
        HOME,
        /// <summary>
        /// A work address.
        /// </summary>
        WORK
    }
}
