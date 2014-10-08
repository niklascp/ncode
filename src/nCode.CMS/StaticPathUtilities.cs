using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace nCode.CMS
{
    /// <summary>
    /// Provides some utilities for handling Static Paths.
    /// </summary>
    public static class StaticPathUtilities
    {
        private static string GetStaticPath(string parentStaticPath, string staticTitle)
        {
            if (string.IsNullOrEmpty(parentStaticPath))
                return staticTitle;
            return parentStaticPath + "/" + staticTitle;
        }

        private static void UpdateStaticPathInternal(SqlConnection conn, string parentStaticPath, Guid contentPageId)
        {
            SqlCommand cmdStaticTitle = new SqlCommand("SELECT StaticTitle FROM CMS_ContentPage WHERE [ID] = @ID", conn);
            cmdStaticTitle.Parameters.AddWithValue("@ID", contentPageId);

            string staticTitle = SqlUtilities.ToString(cmdStaticTitle.ExecuteScalar());

            string staticPath = GetStaticPath(parentStaticPath, staticTitle);

            if (staticPath != null)
            {
                SqlCommand cmdExists = new SqlCommand("SELECT 1 FROM CMS_ContentPage c1 WHERE [ID] = @ID AND EXISTS(SELECT 1 FROM CMS_ContentPage c2 WHERE (NOT c1.[ID] = c2.[ID]) AND (c1.[Language] = c2.[Language]) AND (c2.[StaticPath] = @StaticPath))", conn);
                cmdExists.Parameters.AddWithValue("@ID", contentPageId);
                cmdExists.Parameters.AddWithValue("@StaticPath", staticPath);

                for (int i = 1; cmdExists.ExecuteScalar() != null; i++)
                {
                    staticPath = GetStaticPath(parentStaticPath, staticTitle + "-" + i);
                    cmdExists.Parameters["@StaticPath"].Value = staticPath;
                }
            }

            SqlCommand cmdUpdate = new SqlCommand("UPDATE [CMS_ContentPage] SET [StaticPath] = @StaticPath WHERE [ID] = @ID", conn);
            cmdUpdate.Parameters.AddWithValue("@ID", contentPageId);
            cmdUpdate.Parameters.AddWithValue("@StaticPath", SqlUtilities.FromString(staticPath));
            cmdUpdate.ExecuteNonQuery();

            List<Guid> childContentPageIds = new List<Guid>();

            SqlCommand cmdChilds = new SqlCommand("SELECT ID FROM CMS_ContentPage WHERE [ParentID] = @ID", conn);
            cmdChilds.Parameters.AddWithValue("@ID", contentPageId);

            SqlDataReader rdrChilds = cmdChilds.ExecuteReader();

            while (rdrChilds.Read())
            {
                childContentPageIds.Add((Guid)rdrChilds["ID"]);
            }

            rdrChilds.Close();

            foreach (var childContentPageId in childContentPageIds)
            {
                UpdateStaticPathInternal(conn, staticPath, childContentPageId);
            }
        }

        /// <summary>
        /// Updates the given Content Page static path.
        /// </summary>
        /// <param name="contentPageId"></param>
        public static void UpdateStaticPath(Guid contentPageId)
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdParent = new SqlCommand("SELECT p1.[Language], p2.[StaticPath] FROM [CMS_ContentPage] p1 LEFT JOIN [CMS_ContentPage] p2 ON p1.[ParentID] = p2.[ID] WHERE p1.[ID] = @ID", conn);
                cmdParent.Parameters.AddWithValue("@ID", contentPageId);

                SqlDataReader rdrParent = cmdParent.ExecuteReader();

                string parentPath = null;

                if (rdrParent.Read())
                {
                    parentPath = SqlUtilities.ToString(rdrParent["StaticPath"]);

                    /* If we use old path schema, e.g. /cms/<culture>/<static-path>. */
                    if (parentPath == null && CmsSettings.UseLegacyPathSchema)
                        parentPath = ((string)rdrParent["Language"]).ToLower();
                }

                rdrParent.Close();

                UpdateStaticPathInternal(conn, parentPath, contentPageId);
            }

            CmsPathMappingCache.RefreshPathMapping();
        }

        /// <summary>
        /// Updates all Content Page static paths.
        /// </summary>
        public static void UpdateStaticPath()
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT ID FROM CMS_ContentPage WHERE NOT Parent IS NULL", conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    UpdateStaticPath((Guid)rdr["ID"]);
                }

                rdr.Close();
            }
        }
    }
}
