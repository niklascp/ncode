using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.IO.vCards
{
    /// <summary>
    /// Represents a vCard phone number type.
    /// </summary>
    public enum PhoneNumberType
    {
        /// <summary>
        /// Indicates a preferred number.
        /// </summary>
        PREF,
        /// <summary>
        /// Indicates a work number. 
        /// </summary>
        WORK,
        /// <summary>
        /// Indicates a home number. 
        /// </summary>
        HOME,
        /// <summary>
        /// Indicates a voice number. 
        /// </summary>
        VOICE,
        /// <summary>
        /// Indicates a fax number. 
        /// </summary>
        FAX,
        /// <summary>
        /// Indicates a messaging service on the number. 
        /// </summary>
        MSG,
        /// <summary>
        /// Indicates a cell phone. 
        /// </summary>
        CELL,
        /// <summary>
        /// Indicates a pager number.
        /// </summary>
        PAGER,
        /// <summary>
        /// Indicates a bulletin board system.
        /// </summary>
        BBS,
        /// <summary>
        /// Indicates a MODEM number.
        /// </summary>
        MODEM,
        /// <summary>
        ///  Indicates a car phone.
        /// </summary>
        CAR
    }
}
