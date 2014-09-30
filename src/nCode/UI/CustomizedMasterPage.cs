using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

using nCode.Metadata;
using System.Web;

namespace nCode.UI
{
    /// <summary>
    /// A Master Page that allows customization through a number of metadata properties.
    /// </summary>
    public abstract class CustomizedMasterPage : MasterPage, IMasterPage
    {
        private const string masterPageDir = "~/MasterPages/";
        private const string defaultMasterPage = "Default.master";

        /// <summary>
        /// Initializa a new Customized Master Page.
        /// </summary>
        public CustomizedMasterPage()
        {
            MetadataDescriptors = new MetadataDescriptorCollection();
        }

        /// <summary>
        /// Gets the default masterpage for this site. Usefull to set masterpage in constructor of nested master pages.
        /// </summary>
        public string GetDefaultMasterPageFile()
        {
            if (CanonicalDomainModule.CurrentHostMapping != null &&
                !string.IsNullOrEmpty(CanonicalDomainModule.CurrentHostMapping.MasterPageFile))
                return CanonicalDomainModule.CurrentHostMapping.MasterPageFile;

            return masterPageDir + defaultMasterPage;
        }

        /// <summary>
        /// Gets a list of MetadataDescriptors associated with this Master Page.
        /// </summary>
        public virtual MetadataDescriptorCollection MetadataDescriptors { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating that Legacy Metadata (i.e. Image1, Image2, Image3) should be hidden during page editing.
        /// </summary>
        public virtual bool HideLegacyMetadata { get; set; }

        /// <summary>
        /// Gets a value indication ot the current page is in edit mode.
        /// </summary>
        public virtual bool IsEditing
        {
            get
            {
                if (Page is nCode.UI.Page)
                    return ((nCode.UI.Page)Page).IsEditing;

                return false;
            }
        }

        /// <summary>
        /// Gets the Metadata Context for the current Page, if Page is nCode.UI.Page, otherwise null.
        /// </summary>
        public virtual IMetadataContext MetadataContext
        {
            get
            {
                if (Page is nCode.UI.Page)
                    return ((nCode.UI.Page)Page).MetadataContext;

                return null;
            }
        }

        /// <summary>
        /// Gets the Metadata Context for the site.
        /// </summary>
        public virtual IMetadataContext SiteMetadataContext
        {
            get
            {
                return nCode.Metadata.SiteMetadataContext.Instance;
            }
        }

        /// <summary>
        /// Gets the Navigation object for the current Page, if Page is nCode.UI.Page, otherwise null.
        /// </summary>
        public virtual Navigation Navigation
        {
            get
            {
                if (Page is nCode.UI.Page)
                    return ((nCode.UI.Page)Page).Navigation;

                return null;
            }
        }

        public string CultureUrlPrefix {
            get
            {
                if (Page is nCode.UI.Page)
                    return ((nCode.UI.Page)Page).CultureUrlPrefix;

                return null;
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Gets a name identifying this Master Page during page editing.
        /// </summary>
        public abstract string Name { get; }
    }
}
