using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.UI
{
    /// <summary>
    /// Represents a Navigation Item in a Navigation Tree.
    /// </summary>
    public abstract class TreeNavigationItem : INavigationItem
    {
        /// <summary>
        /// Gets the unique identifier of this Navigation Item.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets the unique identifier of the parent Navigation Item in the Navigation Tree, or null if this item is a root node.
        /// </summary>
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets the title of this Navigation Item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this Navigation Item is visible.
        /// </summary>
        public virtual bool IsVisible { get; set; }

        /// <summary>
        /// Gets a possible frindly URL that can be used for navigation to this Navigation Item.
        /// </summary>
        public abstract string Url { get; }

        /// <summary>
        /// Gets a value indicating whether this Navigation Item is part of the current logical breadcrumb path available in <see cref="Navigation.CurrentPath"/>.
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                return Navigation.CurrentPath.Any(x => x.ID == ID);
            }
        }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets or set a value indicating if this node has child nodes.
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// Gets a possibly context-specific logical Parent of this Navigation Item in the Navigation Graph. This is used to automatically calculate the breadcrumb path available in <see cref="Navigation.CurrentPath" />.
        /// </summary>
        /// <returns></returns>
        public virtual INavigationItem GetParent()
        {
            return null;
        }
    }
}
