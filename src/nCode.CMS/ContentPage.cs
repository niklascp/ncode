using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

using Dapper;
using Newtonsoft.Json;

using nCode.Security;
using nCode.UI;

using nCode.Metadata;
using nCode.CMS.UI;

namespace nCode.CMS
{
    /// <summary>
    /// Represents a Content Item.
    /// </summary>
    public class ContentPage : INavigationItem, IMetadataContext
    {
        /// <summary>
        /// Gets the current ContentPage for this request, or null if this is not
        /// a content page request.
        /// </summary>
        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static ContentPage Current
        {
            get
            {
                return Navigation.Current as ContentPage;
            }
        }

        /// <summary>
        /// Gets the current Path of content pages for this requist, or null if this os 
        /// not a content page request.
        /// </summary>
        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static IList<ContentPage> CurrentPath
        {
            get
            {
                if (Navigation.CurrentPath == null)
                    return null;

                return Navigation.CurrentPath.OfType<ContentPage>().ToList();
            }
        }

        // Data Holders
        private Guid id;

        public ContentPage(Guid id)
        {
            this.id = id;
        }

        public Guid ID
        {
            get { return id; }
        }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Gets the index of this content.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets the index of this content.
        /// </summary>
        public int DisplayIndex { get; set; }

        public ContentType ContentType { get; set; }

        public string StaticTitle { get; set; }

        /// <summary>
        /// Gets or sets the static path which is used to indentify the page
        /// on url requests.
        /// </summary>
        public string StaticPath { get; set; }

        /// <summary>
        /// Gets or sets weather Priviliges shuld be evaluated upon access.
        /// </summary>
        public bool IsProtected { get; set; }

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
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Title Mode.
        /// </summary>
        public TitleMode TitleMode { get; set; }

        /// <summary>
        /// Gets or sets the path to the Master Page File. If null, the default Master Page File will be used.
        /// </summary>
        public string MasterPageFile { get; set; }

        /// <summary>
        /// Gets or sets the Image1 Url.
        /// </summary>
        public string Image1 { get; set; }

        /// <summary>
        /// Gets or sets the Image1 Url.
        /// </summary>
        public string Image2 { get; set; }

        /// <summary>
        /// Gets or sets the Image1 Url.
        /// </summary>
        public string Image3 { get; set; }

        /// <summary>
        /// Gets or sets the Link Url.
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets how the Link Url is handled.
        /// </summary>
        public LinkMode LinkMode { get; set; }




        /* Generated Properties */

        /// <summary>
        /// Returns true if:
        ///   - The Page i shown in menu
        ///   - Page is Valid (From - To)
        ///   - Page is unprotected or user has the View Privilege
        /// </summary>
        public bool IsVisible
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
        }

        /// <summary>
        /// Returns a Permanent Url string for this page that can be used whithin the current domain.
        /// </summary>
        public string Permalink { get; set; }

        /// <summary>
        /// Returns a Url string for this page that can be used whithin the current domain.
        /// </summary>
        public string Url
        {
            get
            {
                if (LinkUrl != null && LinkMode == LinkMode.Normal)
                    return LinkUrl;

                if (CmsPathMappingCache.ReverseMapping != null && CmsPathMappingCache.ReverseMapping.ContainsKey(ID))
                    return "/" + CmsPathMappingCache.ReverseMapping[ID];

                return Permalink;
            }
        }

        /// <summary>
        /// Returns a fully qualified Url string for this page that can be used accross domains.
        /// </summary>
        public string FullUrl
        {
            get
            {
                string url = nCode.Settings.Url;
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);
                return url + Url;
            }
        }

        /// <summary>
        /// Returns true if this page, or any child of this page is the active page.
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (Navigation.CurrentPath != null && Navigation.CurrentPath.Contains(this))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Returns true if this page, or any child of this page is the active page.
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                return Navigation.Current == this;
            }
        }

        /// <summary>
        /// Gets or sets a value if this contentpage is editable.
        /// </summary>
        public bool IsEditable { get; set; }



        /* Methods */

        /// <summary>
        /// Returns the parent ContentPage, or null, if this is the root of the menu.
        /// </summary>
        public INavigationItem GetParent()
        {
            if (ParentId != null)
            {
                var parent = Utilities.GetContentPage(ParentId.Value);

                if (parent.Language != null)
                    return parent;
            }

            return null;
        }

        /// <summary>
        /// Gets a property of type T with the given key. Returns the default value if the property does no exists.
        /// </summary>
        public T GetProperty<T>(string key, T defaultValue)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                var value = conn.ExecuteScalar<string>(@"
select [Value]
from [CMS_ContentPageProperty]
where [ContentPage] = @contentPageId and [Key] = @key",
                    new
                    {
                        contentPageId = this.ID,
                        key = key
                    });

                return GenericMetadataHelper.Deserialize<T>(value, defaultValue);
            }
        }

        /// <summary>
        /// Sets a property of type T with the given key to the given value.
        /// </summary>
        public void SetProperty<T>(string key, T value)
        {
            var serializedValue = GenericMetadataHelper.Serialize(value);

            using (var conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                var affectedRows = conn.Execute(@"
update [CMS_ContentPageProperty]
set [Value] = @value
where [ContentPage] = @contentPageId AND [Key] = @key",
                    new
                    {
                        contentPageId = this.ID,
                        key = key,
                        value = serializedValue
                    });

                if (affectedRows == 0)
                {
                    conn.Execute(@"
insert into [CMS_ContentPageProperty] ([ID], [ContentPage], [Key], [Value])
values (@id, @contentPageId, @key, @value)",
                        new
                        {
                            id = Guid.NewGuid(),
                            contentPageId = this.ID,
                            key = key,
                            value = serializedValue,
                        }
                    );
                }
            }
        }

        /// <summary>
        /// Updates this Content Object in the Database.
        /// </summary>
        public void Update()
        {
            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE CMS_ContentPage SET " +
                    "[Language] = @Language, " +
                    "[DisplayIndex] = @DisplayIndex, " +
                    "[StaticTitle] = @StaticTitle, " +
                    "[LinkMode] = @LinkMode, " +
                    "[IsProtected] = @IsProtected " +
                    "WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", ID);
                cmd.Parameters.AddWithValue("@Language", SqlUtilities.FromString(Language));
                cmd.Parameters.AddWithValue("@DisplayIndex", DisplayIndex);
                cmd.Parameters.AddWithValue("@StaticTitle", StaticTitle);
                cmd.Parameters.AddWithValue("@LinkMode", LinkMode);
                cmd.Parameters.AddWithValue("@IsProtected", IsProtected);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns true, iff. the Content Pages reffers to the same record, i.e. has equal IDs.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj != null && obj is ContentPage && ((ContentPage)obj).ID == ID);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>        
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }

    /// <summary>
    /// Defines how a link is handled.
    /// </summary>
    public enum LinkMode
    {
        /// <summary>
        /// Redirects to the link by sending the link to the requesting client's browser.
        /// </summary>
        Normal,
        /// <summary>
        /// Redirects to the link by rewriting the standard cms url to the link.
        /// </summary>
        UrlRewrite
    }
}