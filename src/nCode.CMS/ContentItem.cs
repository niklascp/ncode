using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.CMS
{
    /// <summary>
    /// Represents a content item on a content page.
    /// </summary>
    public class ContentItem : IContentItemContainer, ICloneable
    {
        Guid id;
        private ContentPage contentPage;
        private ContentType contentType;
        private IContentItemContainer container;
        private List<ContentItem> childs;
        private int index;

        /// <summary>
        /// Initializes a new Content Item
        /// </summary>
        public ContentItem(Guid id, ContentPage contentPage, ContentType contentType)
        {
            this.id = id;
            this.contentPage = contentPage;
            this.contentType = contentType;
            this.childs = new List<ContentItem>();
        }

        public Guid ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the content page this content item belongs to.
        /// </summary>
        public ContentPage ContentPage
        {
            get { return contentPage; }
        }

        public IContentItemContainer Container
        {
            get { return container; }
            set { 
                if (container != null && container is ContentPage)
                    ((ContentPage)container).contentItems.Remove(this);
                if (container != null && container is ContentItem)
                    ((ContentItem)container).childs.Remove(this);

                container = value;

                if (container != null && container is ContentPage)
                    ((ContentPage)container).contentItems.Add(this);
                if (container != null && container is ContentItem)
                    ((ContentItem)container).childs.Add(this);
            }
        }

        /// <summary>
        /// Gets the content type of this content item.
        /// </summary>
        public ContentType ContentType
        {
            get { return contentType; }
        }

        public ContentItem[] ContentItems
        {
            get { return childs.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the index of this content item.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public object Clone()
        {
            // Clone this object
            ContentItem contentItem = new ContentItem(id, contentPage, contentType);
            contentItem.index = index;

            // Clone childs
            for (int i = 0; i < childs.Count; i++)
            {
                ContentItem childItem = (ContentItem)childs[i].Clone();
                childItem.Container = contentItem;
            }

            return contentItem;
        }
    }
}
