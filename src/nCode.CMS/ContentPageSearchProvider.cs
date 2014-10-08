using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.UI;

namespace nCode.CMS
{
    public class ContentPageSearchProvider : ISearchProvider
    {
        public IList<ContentPage> ExecuteSearch(string searchQuery)
        {
            List<ContentPage> result = new List<ContentPage>();

            if (string.IsNullOrWhiteSpace(searchQuery))
                return result;

            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT " + ContentPageFactory.SelectFields + " FROM CMS_ContentPage " +
                    "WHERE Parent IS NOT NULL AND (Title LIKE @Query@ OR Keywords LIKE @Query@ OR Description LIKE @Query@ OR PageContent LIKE @Query@) ORDER BY Title", conn);

                cmd.Parameters.AddWithValue("@Query@", "%" + searchQuery.Replace(" ", "%") + "%");

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ContentPage contentPage = ContentPageFactory.CreateContentPage(rdr);
                    if (contentPage.IsVisible)
                        result.Add(contentPage);
                }

                rdr.Close();
            }

            return result;
        }

        public IEnumerable<SearchResult> Search(string searchQuery)
        {
            List<SearchResult> result = new List<SearchResult>();

            if (string.IsNullOrWhiteSpace(searchQuery))
                return result;

            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT " + ContentPageFactory.SelectFields + " FROM CMS_ContentPage " +
                    "WHERE Parent IS NOT NULL AND (Title LIKE @Query@ OR Keywords LIKE @Query@ OR Description LIKE @Query@ OR PageContent LIKE @Query@) ORDER BY Title", conn);

                cmd.Parameters.AddWithValue("@Query@", "%" + searchQuery.Replace(" ", "%") + "%");

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ContentPage contentPage = ContentPageFactory.CreateContentPage(rdr);
                    if (contentPage.IsVisible)
                        result.Add(new SearchResult
                        {
                            Title = contentPage.Title,
                            Url = contentPage.Url
                        });
                }

                rdr.Close();
            }

            return result;
        }
    }
}
