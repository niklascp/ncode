using nCode.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    public class SiteNavigationItem : SiteMetadataContext, INavigationItem
    {
        public Guid ID {get; set; }

        public string Title {get; set; }

        public string Url {get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or set a value indicating if this node has child nodes.
        /// </summary>
        public bool HasChildren { get; set; }

        public INavigationItem GetParent() { return null; }

        public IEnumerable<INavigationItem> GetChildren() { return null; }
    }
}
