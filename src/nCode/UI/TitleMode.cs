using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    /// <summary>
    /// Defines how a titles is handled.
    /// </summary>
    public enum TitleMode
    {
        /// <summary>
        /// Uses default settings.
        /// </summary>
        Default,
        /// <summary>
        /// Uses only content page title.
        /// </summary>
        None,
        /// <summary>
        /// Append global title after.
        /// </summary>
        AppendAfter,
        /// <summary>
        /// Appende glibal title before.
        /// </summary>
        AppendBefore
    }
}
