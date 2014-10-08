/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

using nCode.Security;
using nCode.UI;

using nCode.Metadata;

namespace nCode.CMS
{
    /// <summary>
    /// Represents a Content Item.
    /// </summary>
    public class ContentPageNode : ContentPage
    {
        private ContentPageNode parent;
        private List<ContentPageNode> children;

        public ContentPageNode(Guid id)
            : base(id)
        {
            this.children = new List<ContentPageNode>();
        }

        public ContentPageNode Parent
        {
            get { return parent; }
            set
            {
                // Remove from old parents child
                if (parent != null && parent.children.Contains(this))
                    parent.children.Remove(this);

                // Set new parent and add this as child
                parent = value;
                if (parent != null)
                    parent.children.Add(this);
            }
        }

        public IList<ContentPageNode> Children
        {
            get { return children; }
        }

        public int Depth { get; set; }

        /// <summary>
        /// Returns true if this page has any child pages.
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return Children.Count > 0;
            }
        }

        /// <summary>
        /// Returns true is this page is allowed to have child pages.
        /// </summary>
        public bool AllowChildren
        {
            get
            {
                /* Menu (i.e. ParentId == null), or a Content Page Type that allows childs  */
                return ParentId == null || ContentType != null && ContentType.AllowChildren;
            }
        }

        public IList<ContentPageNode> Path
        {
            get
            {
                var contentPage = this;
                var path = new List<ContentPageNode>();

                while (contentPage != null && contentPage.Parent != null)
                {
                    path.Add(contentPage);
                    contentPage = contentPage.Parent;
                }

                path.Reverse();

                return path;
            }
        }
    }
}