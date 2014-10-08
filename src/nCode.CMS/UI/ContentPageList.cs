using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace nCode.CMS.UI
{
    [DefaultProperty("DataSource")]
    [ParseChildren(true)]
    [PersistChildren(false)]
    public class ContentPageList : WebControl
    {
        private int maxDepth;
        private IList<ContentPage> contents;
        private ITemplate headerTemplate;
        private ITemplate footerTemplate;
        private ITemplate innerHeaderTemplate;
        private ITemplate innerFooterTemplate;
        private ITemplate itemTemplate;
        private ITemplate itemEndTemplate;

        /// <summary>
        /// Initializes a new Content List Control.
        /// </summary>
        public ContentPageList()
        { }

        public virtual int MaxDepth {
            get { return maxDepth; }
            set { maxDepth = value; }
        }

        public ExpandMode ExpandMode
        {
            get {
                return (ExpandMode)(ViewState["ExpandMode"] ?? ExpandMode.All);
            }
            set
            {
                ViewState["ExpandMode"] = value;
            }
        }


        /// <summary>
        /// Gets or sets the <see cref='System.Web.UI.ITemplate' qualify='true'/> that defines how the control header is 
        /// rendered.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ContentPageListItem))]
        public virtual ITemplate HeaderTemplate
        {
            get
            {
                return headerTemplate;
            }
            set
            {
                headerTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='System.Web.UI.ITemplate' qualify='true'/> that defines how the control footer is 
        /// rendered.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ContentPageListItem))]
        public virtual ITemplate FooterTemplate
        {
            get
            {
                return footerTemplate;
            }
            set
            {
                footerTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='System.Web.UI.ITemplate' qualify='true'/> that defines how the control inner header is 
        /// rendered.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ContentPageListItem))]
        public virtual ITemplate InnerHeaderTemplate
        {
            get
            {
                return innerHeaderTemplate;
            }
            set
            {
                innerHeaderTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='System.Web.UI.ITemplate' qualify='true'/> that defines how the control inner footer is 
        /// rendered.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ContentPageListItem))]
        public virtual ITemplate InnerFooterTemplate
        {
            get
            {
                return innerFooterTemplate;
            }
            set
            {
                innerFooterTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='System.Web.UI.ITemplate' qualify='true'/> that defines how items
        /// are rendered.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ContentPageListItem))]
        public virtual ITemplate ItemTemplate
        {
            get
            {
                return itemTemplate;
            }
            set
            {
                itemTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='System.Web.UI.ITemplate' qualify='true'/> that defines how items
        /// ends are rendered.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(ContentPageListItem))]
        public virtual ITemplate ItemEndTemplate
        {
            get
            {
                return itemEndTemplate;
            }
            set
            {
                itemEndTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the data source that provides data for populating the list.
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IList<ContentPage> Contents
        {
            get
            {
                return contents;
            }
            set
            {
                contents = value;
            }
        }

        protected int BindItemsRecursive(IList<ContentPage> contentPages, Control parent, int depth, int indexOffset)
        {
            int itemCount = contentPages.Count;
            ITemplate currentHeaderTemplate = depth == 0 ? headerTemplate : innerHeaderTemplate;
            ITemplate currentFooterTemplate = depth == 0 ? footerTemplate : innerFooterTemplate;

            if (currentHeaderTemplate != null)
            {
                ContentPageListItem header = new ContentPageListItem(null, 0, depth, indexOffset);
                currentHeaderTemplate.InstantiateIn(header);
                parent.Controls.Add(header);
            }

            for (int index = 0; index < contentPages.Count; index++)
            {
                ContentPageListItem contentPageListItem = new ContentPageListItem(contentPages[index], index, depth, indexOffset + index);
                itemTemplate.InstantiateIn(contentPageListItem);
                parent.Controls.Add(contentPageListItem);
                contentPageListItem.DataBind();

                // Recursive into childs of the current Content Page
                IList<ContentPage> childContentPages = contentPages[index].Childs;
                if (childContentPages.Count > 0 && (maxDepth == 0 || depth < maxDepth) && (ExpandMode == ExpandMode.All || contentPages[index].IsActive))
                {
                    int childCount = BindItemsRecursive(childContentPages, contentPageListItem, depth + 1, indexOffset + index + 1);
                    indexOffset += childCount;
                    itemCount += childCount;
                }

                if (itemEndTemplate != null)
                {
                    ContentPageListItem contentEndItem = new ContentPageListItem(contentPages[index], depth, index, indexOffset + index);
                    itemEndTemplate.InstantiateIn(contentEndItem);
                    parent.Controls.Add(contentEndItem);
                    contentEndItem.DataBind();
                }
            }

            if (currentFooterTemplate != null)
            {
                ContentPageListItem footer = new ContentPageListItem(null, 0, depth, indexOffset);
                currentFooterTemplate.InstantiateIn(footer);
                parent.Controls.Add(footer);
            }

            return itemCount;
        }

        protected override void CreateChildControls()
        {
            if (contents != null)
                BindItemsRecursive(contents, this, 0, 0);
        }

        public override void DataBind()
        {
            Controls.Clear();
            ClearChildViewState();

            base.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.RenderChildren(writer);
        }
    }

    public enum ExpandMode
    {
        All,
        Active
    }

}