using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;

namespace nCode.UI
{
    /// <summary>
    /// Contains extention methods for pages.
    /// </summary>
    public static class PageExtensions
    {
        public static IList<string> GetStylesheets(this System.Web.UI.Page page) {
            var o = (List<string>)page.Items["StyleSheet"];
            if (o == null) {
                o = new List<string>();
                page.Items["StyleSheet"] = o;
            }
            return o;
        }

        /// <summary>
        /// Registers a Stylesheet-file.
        /// </summary>
        public static void IncludeStyleSheet(this System.Web.UI.Page page, string stylesheet)
        {
            var path = page.ResolveUrl(stylesheet).ToLower();

            if (page.GetStylesheets().Contains(path))
                return;

            page.GetStylesheets().Add(path);

            HtmlLink link1 = new HtmlLink();
            link1.Href = path;
            link1.Attributes["type"] = "text/css";
            link1.Attributes["rel"] = "stylesheet";
            page.Header.Controls.Add(link1);
        }

        /// <summary>
        /// Registers a Javascript-file using the given key.
        /// </summary>
        public static void RegisterScriptInclude(this System.Web.UI.Page page, string scriptKey, string scriptFile)
        {
            page.ClientScript.RegisterClientScriptInclude(scriptKey, page.ResolveUrl(scriptFile).ToLower());
        }


        /// <summary>
        /// Adds a meta-tag to the header of this page.
        /// </summary>
        public static void AddMetaTag(this System.Web.UI.Page page, string name, string content)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            var container = page.FindControl("pageMetaDataPlaceholder");

            if (container == null)
                container = page.Header;

            if (container != null)
            {
                HtmlMeta metaTag = new HtmlMeta();
                metaTag.Name = name;
                metaTag.Content = content;
                container.Controls.Add(metaTag);
            }
        }

        /// <summary>
        /// Adds a Http-equivalent meta-tag to the header of this page.
        /// </summary>
        public static void AddHttpEquivMetaTag(this System.Web.UI.Page page, string httpEquiv, string content)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            var container = page.FindControl("pageMetaDataPlaceholder");

            if (container == null)
                container = page.Header;

            if (container != null)
            {
                HtmlMeta metaTag = new HtmlMeta();
                metaTag.HttpEquiv = httpEquiv;
                metaTag.Content = content;
                container.Controls.Add(metaTag);
            }
        }
    }
}
