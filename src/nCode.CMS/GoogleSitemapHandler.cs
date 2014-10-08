using System;
using System.Web;
using System.Xml;
using System.Text;

namespace nCode.CMS
{
    /// <summary>
    /// Outputs XML-data for Google Sitemap.
    /// </summary>
    public class GoogleSitemapHandler : IHttpHandler
    {
        protected virtual void WriteAdditionalPages(XmlTextWriter writer)
        { }

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            response.ContentType = "text/xml";

            using (XmlTextWriter writer = new XmlTextWriter(response.OutputStream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset");
                writer.WriteAttributeString("xmlns", "http://www.google.com/schemas/sitemap/0.84");

                foreach (ContentPage page in Utilities.ContentPages)
                {                    
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", page.FullUrl);
                    writer.WriteElementString("lastmod", page.Modified.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                    writer.WriteEndElement();
                }

                WriteAdditionalPages(writer);
            }
        }
    }
}
