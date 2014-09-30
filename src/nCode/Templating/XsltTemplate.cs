using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Net.Mail;
using System.Web;

namespace nCode.Templating
{
    public class XsltTemplate
    {
        public static XsltTemplate Load(string name, string[] searchPath, string culture)
        {
            foreach (string s in searchPath)
            {
                string path = HttpContext.Current.Server.MapPath(s);
                string cultureSpecific = Path.Combine(path, name + "." + culture + ".xslt");
                string cultureInvariant = Path.Combine(path, name + ".xslt");

                if (File.Exists(cultureSpecific))
                    return new XsltTemplate(cultureSpecific);
                else if (File.Exists(cultureInvariant))
                    return new XsltTemplate(cultureInvariant); 
            }

            throw new ApplicationException(string.Format("Could not find template '{0}' for culture '{1}'. Searched the following paths:\n{2}", name, culture, string.Join("\n", searchPath)));
        }

        XslCompiledTransform transform;

        public XsltTemplate(string xslFile)
        {
            transform = new XslCompiledTransform();
            transform.Load(
                xslFile,
                new XsltSettings()
                {
                    EnableScript = true
                },
                new XmlUrlResolver()
                {                    
                }
            );
        }

        public XsltTemplate(XslCompiledTransform transform)
        {
            this.transform = transform;
        }

        public XDocument TransformXDocument(XDocument data)
        {
            XDocument result = new XDocument();
            using (XmlWriter writer = result.CreateWriter())
            {
                // Execute the transform and output the results to a writer.
                transform.Transform(data.CreateReader(), writer);
            }
            return result;
        }

        public string Transform(XDocument data)
        {
            StringBuilder output = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(output, transform.OutputSettings))
            {
                // Execute the transform and output the results to a writer.
                transform.Transform(data.CreateReader(), writer);
            }

            return output.ToString();
        }

        public MailMessage TransformToMailMessage(XDocument data)
        {
            XDocument transformedData = TransformXDocument(data);
            MailMessage message = new MailMessage();
            message.Body = Transform(data);
            message.IsBodyHtml = transform.OutputSettings.OutputMethod == XmlOutputMethod.Html;

            /* Try to get a title from the html-template's title-tag. */
            if (message.IsBodyHtml)
            {
                var subject = (from html in transformedData.Elements("html")
                               from head in html.Elements("head")
                               from title in head.Elements("title")
                               select title.Value.Trim()).FirstOrDefault();

                message.Subject = subject.Replace('\r', ' ').Replace('\n', ' ').Replace("  ", " ");
            }

            return message;
        }
    }
}
