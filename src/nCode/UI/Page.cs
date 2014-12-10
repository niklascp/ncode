using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using nCode.Metadata;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace nCode.UI
{
    /// <summary>
    /// Represents a nCode Page. All ASPX-files should inhiret this class.
    /// </summary>
    public class Page : System.Web.UI.Page
    {
        public event EventHandler SetMasterPage;

        private const string masterPageDir = "~/MasterPages/";
        private const string defaultMasterPage = "Default.master";
        private const string printerFriendlyMasterPage = "Print.master";

        public bool IsEditing { get; set; }

        public TitleMode TitleMode { get; set; }

        public string UICultureName { get { return CultureInfo.CurrentUICulture.Name; } }

        public string CultureUrlPrefix {
            get
            {
                if (string.Equals(CultureInfo.CurrentUICulture.Name, Settings.SupportedCultureNames.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                    return string.Empty;

                return "/" + CultureInfo.CurrentUICulture.Name.ToLower();
            } 
        }


        /// <summary>
        /// Returns a metadata context that are global to the entire site, sutable for storing site-wise properties.
        /// </summary>
        public IMetadataContext SiteMetadataContext
        {
            get
            {
                return nCode.Metadata.SiteMetadataContext.Instance;
            }
        }

        public IMetadataContext MetadataContext { get; set; }

        public Navigation Navigation { get; private set; }

        protected void OnSetMasterPage(EventArgs e)
        {
            if (SetMasterPage != null)
                SetMasterPage(this, e);

            /* Printer frindly */
            if (string.Equals(Request.QueryString["Print"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase) && File.Exists(Server.MapPath(masterPageDir + printerFriendlyMasterPage)))
                MasterPageFile = masterPageDir + printerFriendlyMasterPage;
            else if (Request.QueryString["Template"] != null && File.Exists(Server.MapPath(masterPageDir + Request.QueryString["Template"] + ".master")))
                MasterPageFile = masterPageDir + Request.QueryString["Template"] + ".master";

            /* Set Default MasterPage */
            if (MasterPageFile == null)
                MasterPageFile = masterPageDir + defaultMasterPage;

            /* We resolved to the default masterpage, but have a different default masterpage given by the current host mapping. */
            if (CanonicalDomainModule.CurrentHostMapping != null && 
                !string.IsNullOrEmpty(CanonicalDomainModule.CurrentHostMapping.MasterPageFile) &&
                MasterPageFile.Equals(VirtualPathUtility.ToAbsolute(masterPageDir + defaultMasterPage), StringComparison.InvariantCultureIgnoreCase))
                MasterPageFile = CanonicalDomainModule.CurrentHostMapping.MasterPageFile;
        }

        /// <summary>
        /// Initializes a new page.
        /// </summary>
        public Page()
        {
            Navigation = new Navigation();
        }

        [Obsolete("Pages that require special urls should rewrite content after render.")]
        public virtual string PageHandledUrl(string url)
        {
            return url;
        }

        protected override void OnPreInit(EventArgs e)
        {
            OnSetMasterPage(new EventArgs());
            base.OnPreInit(e);
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            /* Add a dummy navigation item, if no one has been set yet. */
            if (Navigation.Current == null)
            {
                Navigation.Current = new SiteNavigationItem()
                {
                    ID = Guid.NewGuid(),
                    Title = Header != null ? Header.Title : null,
                    Url = Request.Url.PathAndQuery
                };
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Page.Header != null)
            {
                var container = Header.FindControl("metadataPlaceholder") ?? Header.FindControl("pageMetaDataPlaceholder");

                if (container != null)
                {
                }
                else
                {
                    /* Add some legacy Metadags if using "old"-style. */
                    Page.AddHttpEquivMetaTag("content-type", "text/html; charset=" + Page.Response.HeaderEncoding.WebName);
                    Page.AddHttpEquivMetaTag("content-language", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    Page.AddMetaTag("generator", "nCode, http://www.ncode.dk/");
                }
            }

            base.OnLoad(e);            

            /* Inject Google Analytics JavaScript. */
            if (Page.Header != null && !string.IsNullOrEmpty(Settings.GoogleAnalyticsAccount))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"  var _gaq = _gaq || [];");
                sb.AppendLine(string.Format(@"  _gaq.push(['_setAccount', '{0}']);", Settings.GoogleAnalyticsAccount));
                sb.AppendLine(@"  _gaq.push(['_trackPageview']);");
                sb.AppendLine();
                sb.AppendLine(@"  (function() {");
                sb.AppendLine(@"    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;");
                sb.AppendLine(@"    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';");
                sb.AppendLine(@"    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);");
                sb.AppendLine(@"  })();");

                var script = new LiteralControl(sb.ToString());

                var javascriptControl = new HtmlGenericControl("script");
                javascriptControl.Attributes.Add("type", "text/javascript");
                javascriptControl.Controls.Add(script);

                Page.Header.Controls.Add(javascriptControl);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Header == null)
                return;

            var titleMode = TitleMode;
            if (titleMode == TitleMode.Default)
                titleMode = Settings.TitleMode;

            if (string.IsNullOrEmpty(Title))
                titleMode = TitleMode.Default;

            switch (titleMode)
            {
                case TitleMode.Default:
                    Header.Title = Settings.Title;
                    break;
                case TitleMode.None:
                    break;
                case TitleMode.AppendAfter:
                    Header.Title = Title + " - " + Settings.Title;
                    break;
                case TitleMode.AppendBefore:
                    Header.Title = Settings.Title + " - " + Title;
                    break;
            }
        }
    }
}
