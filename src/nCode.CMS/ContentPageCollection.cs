using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace nCode.CMS
{
    public delegate bool ContentPageFilter(ContentPage content);

    public class ContentPageCollection : IEnumerable
    {
        // Contains All Content
        private Hashtable contentPagesByID;
        private Hashtable contentPagesByPath;
        private ArrayList menus;

        public ContentPageCollection()
        {
            contentPagesByID = new Hashtable();
            contentPagesByPath = new Hashtable();
            menus = new ArrayList();
        }

        /// <summary>
        /// Loads this Content Collection with Content Objects using the <c>CultureInfo.CurrentUICulture.Name</c> Language Code.
        /// </summary>
        public void LoadContent()
        {
            LoadContent(CultureInfo.CurrentUICulture.Name, null);
        }

        /// <summary>
        /// Loads this Content Collection with Content Objects.
        /// </summary>
        /// <param name="onlyVisible">If true only load visible content (evalueted by the Content.IsVisible property).</param>
        public void LoadContent(bool onlyVisible)
        {
            if (onlyVisible)
                LoadContent(CultureInfo.CurrentUICulture.Name, content => content.IsVisible);
            else
                LoadContent(CultureInfo.CurrentUICulture.Name, null);
        }

        /// <summary>
        /// Loads this Content Collection with Content Objects, in the given Language Code, using a Custom filter.
        /// </summary>
        public void LoadContent(string language, ContentPageFilter filter)
        {
            contentPagesByID.Clear();
            menus.Clear();

            // Temp. Collection to hold Content and ParentID
            ArrayList tempContent = new ArrayList();
            ArrayList tempParentID = new ArrayList();

            using (SqlConnection conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT " +
                    "ID, " +
                    "Created, " +
                    "Modified, " +
                    "Language, " +
                    "Parent, " +
                    "[Index], " +
                    "StaticTitle, " +
                    "StaticPath, " +

                    // Content
                    "ContentType, " +
                    "LinkUrl, " +
                    "LinkMode, " +

                    // Security and Access
                    "IsProtected, " +
                    "ShowInMenu, " +
                    "ValidFrom, " +
                    "ValidTo, " +

                    // Look and Feel
                    "Title, " +
                    "TitleMode, " +
                    "MasterPageFile, " +
                    "Image1, " +
                    "Image2, " +
                    "Image3");

                sb.AppendLine("FROM CMS_ContentPages");
                
                if (language != null)
                {
                    sb.AppendLine("WHERE Language IS NULL OR Language = @Language");
                    cmd.Parameters.AddWithValue("@Language", language);
                }

                sb.AppendLine("ORDER BY Language, Parent, [Index]");

                cmd.CommandText = sb.ToString();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ContentPage content = ContentPageFactory.CreateContentPage(rdr);

                    if (filter == null || filter(content))
                    {
                        if (contentPagesByID.ContainsKey(content.ID))
                            continue;

                        contentPagesByID.Add(content.ID, content);

                        if (content.StaticPath != null)
                            contentPagesByPath.Add(content.StaticPath.ToLower(), content);

                        // Create the Content Object and add it to the main collection.
                        if (rdr["Parent"] != DBNull.Value)
                        {
                            // Remember the ID off it's parent node.
                            tempParentID.Add((Guid)rdr["Parent"]);
                            tempContent.Add(content);
                        }
                        // Create Menu Object
                        else
                        {
                            menus.Add(content);
                        }
                    }
                }

                rdr.Close();
            }

            // Associate nodes with their parents.
            int count = tempContent.Count;
            int index = 0;

            while (index < count)
            {
                ContentPage node = (ContentPage)tempContent[index];

                // We have a child node                
                ContentPage parent = (ContentPage)contentPagesByID[(Guid)tempParentID[index]];
                node.Parent = parent;
                index++;
            }
        }

        public ContentPage[] Menus
        {

            get { return (ContentPage[])menus.ToArray(typeof(ContentPage)); }
        }

        public ContentPage this[string path]
        {
            get
            {
                return (ContentPage)contentPagesByPath[path.ToLower()];
            }
        }

        public ContentPage this[Guid id]
        {
            get { return (ContentPage)contentPagesByID[id]; }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return contentPagesByPath.Values.GetEnumerator();
        }

        #endregion
    }
}
