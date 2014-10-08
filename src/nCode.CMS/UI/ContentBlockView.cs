using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using System.IO;

namespace nCode.CMS.UI
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ContentBlockView runat=server></{0}>")]
    public class ContentBlockView : Control
    {
        [Category("Content")]
        [DefaultValue(false)]
        public string Code
        {
            get
            {;
                return (string)ViewState["ContentBlockCode"];
            }
            set
            {
                ViewState["ContentBlockCode"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var content = ContentBlockUtilities.GetContentBlockContent(Code);

            if (content != null)
            {
                using (StringReader reader = new StringReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {                        
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
}
