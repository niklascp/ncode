using System;
using System.Collections.Specialized;
using System.Web.UI;
using nCode.CMS.UI;

namespace nCode.CMS
{
    public class GenericContentType : ContentType
    {
        private Guid id;
        private string name;
        private string title;
        private string description;

        private string viewControlPath;
        private string editControlPath;

        public override Guid ID
        {
            get { return id; }
        }

        public override string Name
        {
            get { return name; }
        }

        public override string Title
        {
            get { return title; }
        }

        public override string Description
        {
            get { return description; }
        }

        public override void Initialize(NameValueCollection config, string root)
        {
            if (config["id"] == null)
                throw new ApplicationException("Must specify 'id' attribute for Content Type '" + typeof(GenericContentType).FullName + "'.");

            if (config["name"] == null)
                throw new ApplicationException("Must specify 'name' attribute for Content Type '" + typeof(GenericContentType).FullName + "'.");

            if (config["title"] == null)
                throw new ApplicationException("Must specify 'title' attribute for Content Type '" + typeof(GenericContentType).FullName + "'.");

            //if (config["viewControlPath"] == null)
            //    throw new ApplicationException("Must specify 'viewControlPath' attribute for Content Type '" + typeof(GenericContentType).FullName + "'.");

            id = new Guid(config["id"]);
            name = config["name"];
            title = config["title"];
            description = config["description"] ?? string.Empty;

            viewControlPath = config["viewControlPath"];
            editControlPath = config["editControlPath"];

            base.Initialize(config, root);
        }

        public override ContentControl GetControl(Page page)
        {
            return (ContentControl)page.LoadControl(viewControlPath);
        }

        public override ContentEditControl GetEditControl(Page page)
        {
            if (editControlPath == null)
                return null;

            return (ContentEditControl)page.LoadControl(editControlPath);
        }
    }
}
