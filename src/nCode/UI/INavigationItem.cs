using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.Metadata;

namespace nCode.UI
{
    /// <summary>
    /// Represents requirements for implementing a Navigation Item.
    /// </summary>
    public interface INavigationItem
    {
        /// <summary>
        /// Gets the unique identifier of this Navigation Item.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Gets the title of this Navigation Item.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets a possible frindly URL that can be used for navigation to this Navigation Item.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Gets a possibly context-specific logical Parent of this Navigation Item in the Navigation Graph. This is used to automatically calculate the breadcrumb path available in <see cref="Navigation.CurrentPath"/>.
        /// </summary>
        INavigationItem GetParent();
    }
}
