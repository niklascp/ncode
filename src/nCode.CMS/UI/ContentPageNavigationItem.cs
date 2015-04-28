using System;
using System.Globalization;
using System.Linq;

using nCode.Metadata;
using nCode.UI;
using nCode.CMS;
using nCode.CMS.Models;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Represents an Brand in the Navigation Framework.
    /// </summary>
    public class ContentPageNavigationItem : TreeNavigationItem, IMetadataContext
    {
        public static ContentPageNavigationItem GetFromID(Guid id)
        {
            using (var db = new CmsDbContext())
            {
                return (from p in db.ContentPages.Where(x => x.ID == id)
                        orderby p.DisplayIndex
                        select new ContentPageNavigationItem()
                        {
                            ID = p.ID,
                            ParentID = p.ParentID,
                            Title = p.Title,
                            ShowInMenu = p.ShowInMenu,
                            ValidFrom = p.ValidFrom,
                            ValidTo = p.ValidTo,
                            LinkUrl = p.LinkUrl,
                            LinkMode = p.LinkMode
                        }).SingleOrDefault();
            }
        }

        private Lazy<Dictionary<string, string>> properties;

        private Dictionary<string, string> GetProperties()
        {
            var p = new Dictionary<string, string>();
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT [Key], [Value] FROM [CMS_ContentPageProperty] WHERE [ContentPage] = @ContentPage", conn);
                cmd.Parameters.AddWithValue("@ContentPage", ID);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    p.Add(((string)rdr["Key"]).ToLower(), (string)rdr["Value"]);
                }
            }

            return p;
        }

        public ContentPageNavigationItem()
        {
            properties = new Lazy<Dictionary<string, string>>(() => GetProperties(), isThreadSafe: true);
        }

        /// <summary>
        /// Gets or sets weather this page should be listed in its menu.
        /// </summary>
        public bool ShowInMenu { get; set; }

        /// <summary>
        /// Gets or sets when this page is valid from.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets when this page is valid to.
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Returns true if:
        ///   - The Page i shown in menu
        ///   - Page is Valid (From - To)
        ///   - Page is unprotected or user has the View Privilege
        /// </summary>
        public override bool IsVisible
        {
            get
            {
                if (!ShowInMenu)
                {
                    return false;
                }
                else if (ValidFrom.HasValue && ValidFrom.Value > DateTime.Today)
                {
                    return false;
                }
                else if (ValidTo.HasValue && ValidTo.Value < DateTime.Today)
                {
                    return false;
                }

                return true;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets the Link Url.
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets how the Link Url is handled.
        /// </summary>
        public LinkMode LinkMode { get; set; }

        /// <summary>
        /// Returns a Url string for this page that can be used whithin the current domain.
        /// </summary>
        public override string Url
        {
            get
            {
                if (LinkUrl != null && LinkMode == LinkMode.Normal)
                    return LinkUrl;

                if (CmsPathMappingCache.ReverseMapping.ContainsKey(ID))
                    return "/" + CmsPathMappingCache.ReverseMapping[ID];

                return null; // Permalink;
            }
        }

        public override INavigationItem GetParent()
        {
            if (ParentID != null)
                return GetFromID(ParentID.Value);

            return null;
        }

        /// <summary>
        /// Get the property with the given key.
        /// </summary>
        public T GetProperty<T>(string key, T defaultValue)
        {
            var ps = properties.Value;

            if (ps.ContainsKey(key.ToLower()))
            {
                return GenericMetadataHelper.Deserialize(ps[key.ToLower()], defaultValue);
            }

            return defaultValue;
        }

        public void SetProperty<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}