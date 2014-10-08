using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using nCode.CMS.UI;
using nCode.Security;
using System.IO;
using System.Web;

namespace nCode.CMS
{
    /// <summary>
    /// Represents a abstract Content Type.
    /// </summary>
    public abstract class ContentType
    {
        private bool supportContentPartMode;

        private string contentPartDirectoryVirtualPath;
        private string contentPartControlVirtualPath;
        private string icon;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentType"/> class.
        /// </summary>
        public ContentType()
        {
        }

        /// <summary>
        /// A unique ID identifies this Content Type.
        /// </summary>
        public abstract Guid ID { get; }

        /// <summary>
        /// Gets the non-localized system name for the module that loaded this Content Type.
        /// </summary>
        public virtual string ModuleName { get; set; }

        /// <summary>
        /// Gets the non-localized system name for this Content Type.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a localized title for this Content Type.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets a localized description for this Content Type.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets a icon for this Content Type.
        /// </summary>
        public virtual string Icon { get { return icon; } }

        /// <summary>
        /// Gets a icon for this Content Type.
        /// </summary>
        public virtual string VirtualRoot { get; protected set; }

        /// <summary>
        /// Initializes the Content Type.
        /// </summary>
        public virtual void Initialize(NameValueCollection config, string root)
        {
            VirtualRoot = VirtualPathUtility.AppendTrailingSlash(root);
            contentPartControlVirtualPath = VirtualPathUtility.Combine(VirtualRoot, "ContentPart.ascx");

            /* Test for Default ContentPart Control. */
            if (File.Exists(HttpContext.Current.Server.MapPath(contentPartControlVirtualPath)))
            {
                /* Default ContentPart Control Found. */
                supportContentPartMode = true;                
            }
            //else
            //{
            //    /* Test for Custom ContentPart Control. */
            //    contentPartControlVirtualPath = string.Format(@"~/{0}/ContentControls/{1}/ContentPart.ascx", ModuleName, Name);
            //    if (File.Exists(HttpContext.Current.Server.MapPath(contentPartControlVirtualPath)))
            //    {
            //        /* Custom ContentPart Control Found. */
            //        supportContentPartMode = true;
            //    }
            //    else
            //    {
            //        supportContentPartMode = false;                  
            //    }
            //}

            icon = config["icon"];

            if (icon == null)
                icon = VirtualPathUtility.Combine(VirtualRoot, "Icon.png");

            VirtualRoot = root;
        }

        /// <summary>
        /// When overridden in derived classes, called upon content page creations.
        /// </summary>
        public virtual void ContentPageCreated(ContentPage contentPage)
        {
            /* Set calculated properties, that migth be dependent on the content type. */
            contentPage.Permalink = @"/cms/Content-View?ID=" + contentPage.ID.ToString();
        }

        /// <summary>
        /// True if this Content Type supports Content Part Mode, otherwise false.
        /// </summary>
        public virtual bool SupportContentPartMode
        {
            get { return supportContentPartMode; }
        }

        public virtual bool SupportNeastedContentType(ContentType c)
        {
            return false;
        }

        /// <summary>
        /// Gets a ContentControl that can be used to view Content Parts of this Content Type.
        /// </summary>
        public virtual ContentPartControl GetContentPartControl(Page page)
        {
            if (SupportContentPartMode)
                return (ContentPartControl)page.LoadControl(contentPartControlVirtualPath);

            return null;
        }

        /// <summary>
        /// Gets a ContentControl that can be used to view this Content Type.
        /// </summary>
        [Obsolete]
        public abstract ContentControl GetControl(Page page);

        /// <summary>
        /// Gets a ContentEditControl that can be used to edit this Content Type.
        /// </summary>
        [Obsolete]
        public abstract ContentEditControl GetEditControl(Page page);

        /// <summary>
        /// Called when building Content Page Tree, before travesing childrens.
        /// </summary>
        [Obsolete]
        public virtual void PreTraverseChildren(ContentPageNode contentPage)
        {
            
        }

        /// <summary>
        /// Called when building Content Page Tree, after childrens.
        /// </summary>
        [Obsolete]
        public virtual void PostTraverseChildren(ContentPageNode contentPage)
        {
            
        }

        /// <summary>
        /// When overridden in derived classes, returns a value that indicate wheater this content page type can have child pages.
        /// </summary>
        public virtual bool AllowChildren {
            get { return true; } 
        }
    }
}