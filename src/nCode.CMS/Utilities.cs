using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace nCode.CMS
{
    public static class Utilities
    {
        /// <summary>
        /// Returns true, if the given content page exitsts.
        /// </summary>
        public static bool GetContentExists(string culture, string staticPath)
        {
            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdSelectPage = new SqlCommand("SELECT 1 FROM CMS_ContentPage WHERE Language = @Culture AND StaticPath = @StaticPath", conn);
                cmdSelectPage.Parameters.AddWithValue("@Culture", culture);
                cmdSelectPage.Parameters.AddWithValue("@StaticPath", staticPath);

                return cmdSelectPage.ExecuteScalar() != null;
            }
        }

        /// <summary>
        /// Returns the given content page, if it exitsts, otherwise null.
        /// </summary>
        public static ContentPage GetContentPage(string culture, string staticPath)
        {
            ContentPage content = null;
            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdSelectPage = new SqlCommand("SELECT " + ContentPageFactory.SelectFields + " FROM CMS_ContentPage WHERE Language = @Culture AND StaticPath = @StaticPath", conn);
                cmdSelectPage.Parameters.AddWithValue("@Culture", culture);
                cmdSelectPage.Parameters.AddWithValue("@StaticPath", staticPath);

                SqlDataReader rdrPage = cmdSelectPage.ExecuteReader();

                if (rdrPage.Read())
                {
                    content = ContentPageFactory.CreateContentPage(rdrPage);
                }

                rdrPage.Close();
            }

            return content;
        }

        public static ContentPage GetContentPage(Guid id)
        {
            ContentPage content = null;
            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdSelectPage = new SqlCommand("SELECT " + ContentPageFactory.SelectFields + " FROM CMS_ContentPage WHERE ID = @ID", conn);
                cmdSelectPage.Parameters.AddWithValue("@ID", id);

                SqlDataReader rdrPage = cmdSelectPage.ExecuteReader();

                if (rdrPage.Read())
                {
                    content = ContentPageFactory.CreateContentPage(rdrPage);
                }

                rdrPage.Close();
            }

            return content;
        }

        /// <summary>
        /// Gets a Content object given its ID.
        /// </summary>
        [Obsolete("Please use the ContentBlock feature.")]
        public static ContentPage GetContent(Guid id)
        {
            nCode.CMS.ContentPage content = null;
            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdSelectPage = new SqlCommand("SELECT ID, ContentType, Language, Title FROM CMS_ContentPage WHERE ID = @ID", conn);
                cmdSelectPage.Parameters.AddWithValue("@ID", id);

                SqlDataReader rdrPage = cmdSelectPage.ExecuteReader();

                if (rdrPage.Read())
                {
                    content = new nCode.CMS.ContentPage((Guid)rdrPage["ID"]);
                    content.ContentType = nCode.CMS.CmsSettings.ContentTypes[(Guid)rdrPage["ContentType"]];
                    content.Title = (string)rdrPage["Title"];
                    content.Language = SqlUtilities.ToString(rdrPage["Language"]);
                }
            }

            return content;
        }

        /// <summary>
        /// Gets the PageContent given the page's unique id.
        /// </summary>
        [Obsolete("Please use the ContentBlock feature.")]
        public static string GetPageContent(Guid id)
        {
            string content = null;

            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT PageContent FROM CMS_ContentPage WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    content = (string)rdr["PageContent"];

                rdr.Close();
            }

            return content;
        }

        /// <summary>
        /// Gets the PageContent given the Static Title in the current UI language.
        /// </summary>
        [Obsolete("Please use the ContentBlock feature.")]
        public static string GetPageContent(string staticTitle)
        {
            return GetPageContent(staticTitle, CultureInfo.CurrentUICulture.Name);
        }

        /// <summary>
        /// Gets the PageContent given the Static Title in the given language.
        /// </summary>
        [Obsolete("Please use the ContentBlock feature.")]
        public static string GetPageContent(string staticTitle, string language)
        {
            string content = null;

            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT PageContent FROM CMS_ContentPage WHERE Language = @Language AND StaticTitle = @StaticTitle", conn);
                cmd.Parameters.AddWithValue("@Language", language);
                cmd.Parameters.AddWithValue("@StaticTitle", staticTitle);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    content = (string)rdr["PageContent"];

                rdr.Close();
            }

            return content;
        }
    }
}
