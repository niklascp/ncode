using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using nCode.Configuration;
using nCode.UI;

namespace nCode.CMS
{
    public static class CmsSettings
    {
        // Singleton Pattern
        private static ContentTypeCollection contentTypes;

        static CmsSettings()
        {
            contentTypes = new ContentTypeCollection();
        }

        /// <summary>
        /// Gets a collection of supported Content Types.
        /// </summary>
        public static ContentTypeCollection ContentTypes
        {
            get { return contentTypes; }
        }

        // Settings Properties

        public static bool UsePermalinks
        {
            get { return nCode.Settings.GetProperty<bool>("nCode.CMS.UsePermalinks", false); }
            set { nCode.Settings.SetProperty<bool>("nCode.CMS.UsePermalinks", value); }
        }

        public static bool OmitExtension
        {
            get { return nCode.Settings.GetProperty<bool>("nCode.CMS.OmitExtension", false); }
            set { nCode.Settings.SetProperty<bool>("nCode.CMS.OmitExtension", value); }
        }

        public static bool CacheMenuViews
        {
            get { return nCode.Settings.GetProperty<bool>("nCode.CMS.CacheMenuViews", false); }
            set { nCode.Settings.SetProperty<bool>("nCode.CMS.CacheMenuViews", value); }
        }

        public static bool UseLegacyPathSchema
        {
            get { return nCode.Settings.GetProperty<bool>("nCode.CMS.UseLegacyPathSchema", true); }
            set { nCode.Settings.SetProperty<bool>("nCode.CMS.UseLegacyPathSchema", value); }
        }

        public static bool AcceptLegacyPathSchema
        {
            get { return nCode.Settings.GetProperty<bool>("nCode.CMS.AcceptLegacyPathSchema", false); }
            set { nCode.Settings.SetProperty<bool>("nCode.CMS.AcceptLegacyPathSchema", value); }
        }

        /// <summary>
        /// Gets or sets wheather the actual name of the default master page (/CMS/MasterPages/Default.master or /MasterPages/Default.master) shuld be shown in UI, or 
        /// wheather it is simply denoted "(Standard)". Default is false.
        /// </summary>
        public static bool ShowActualDefaultMasterPageName
        {
            get { return nCode.Settings.GetProperty<bool>("nCode.CMS.ShowActualDefaultMasterPageName", false); }
            set { nCode.Settings.SetProperty<bool>("nCode.CMS.ShowActualDefaultMasterPageName", value); }
        }
    }

}
