using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nCode.CMS.Models;

using Dapper;

namespace nCode.CMS.Data
{
    public class CmsRepository : ICmsRepository
    {
        public ContentPage GetContentPage(Guid id)
        {
            ContentPage content = null;
            using (var conn = new SqlConnection(nCode.Settings.ConnectionString))
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

        public void ConvertContentPage(ContentPage contentPageView)
        {
            using (var cmsDbContext = new CmsDbContext())
            using (var conn = new SqlConnection(nCode.Settings.ConnectionString))
            {
                conn.Open();

                var contentPage = cmsDbContext.ContentPages.Find(contentPageView.ID);
                var contentType = contentPageView.ContentType;

                ContentPart contentPart = null;

                if (contentType is StaticContent)
                {
                    contentPart = new ContentPart()
                    {
                        ID = Guid.NewGuid(),
                        ContentTypeID = contentType.ID,
                    };

                    contentPart.SetProperty("Content", contentPage.PageContent);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot convert pages of type '{0}'.", contentPageView.ContentType.Name));
                }

                if (contentPart != null)
                {
                    contentPage.ContentTypeID = CmsSettings.ContentTypes.Single(x => x.Name == "ContentPart").ID;

                    cmsDbContext.ContentParts.Add(contentPart);

                    var contentPartInstance = new ContentPartInstance()
                    {
                        ID = Guid.NewGuid(),
                        RootContainerID = contentPageView.ID,
                        ContainerID = contentPageView.ID,
                        ContentPartID = contentPart.ID,
                        DisplayIndex = 0,
                    };

                    cmsDbContext.ContentPartInstances.Add(contentPartInstance);

                    cmsDbContext.SaveChanges();
                }
            }

        }

        public void Dispose()
        {
        }
    }
}
