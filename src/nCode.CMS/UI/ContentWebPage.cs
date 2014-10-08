using System;
using System.Web;
using System.Web.Security;

namespace nCode.CMS.UI
{
    public class ContentPage : nCode.UI.Page
    {
        private string path;
        private ContentPage content;

        protected override void OnPreInit(EventArgs e)
        {
            path = Request.QueryString["Path"];

            base.OnPreInit(e);

            // Load the Content Object from the path.
            ContentPageCollection contents = new ContentPageCollection();
            contents.LoadContent();
            Content = contents[path];

            if (Content == null)
                throw new HttpException(404, "The requested CMS Page does not exits.");

            if (Content.IsProtected && !Content.PrivilegeContext.HasPrivilege("View"))
                Response.Redirect(FormsAuthentication.LoginUrl + "?returnurl=" + HttpUtility.UrlEncode(Content.Url));

            // Set MasterPage
            if (Request.QueryString["Template"] != null)
                Page.MasterPageFile = "~/CMS/MasterPages/" + Request.QueryString["Template"] + ".master";
            else if (Content.MasterPageFile != null)
                Page.MasterPageFile = Content.MasterPageFile;
            else
                Page.MasterPageFile = "~/CMS/MasterPages/Default.master";

            Title = Content.Title;
        }

        /// <summary>
        /// Gets or sets the current Content object.
        /// </summary>
        public ContentPage Content
        {
            get { return content; }
            set { content = value; }
        }
    }
}