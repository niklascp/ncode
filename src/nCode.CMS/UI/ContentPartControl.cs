using System;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.IO;
using nCode.CMS.Model;
using System.Web.UI.WebControls;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Represents an abstract class for Content Types Editing Controls.
    /// </summary>
    public abstract class ContentPartControl : UserControl
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {           
            try
            {
                base.OnLoad(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void HandleException(Exception ex)
        {
            Controls.Clear();

            if (IsEditing)
            {
                var panel = new Panel();
                panel.CssClass = "alert alert-danger";
                panel.Controls.Add(new LiteralControl("<p>" + HttpUtility.HtmlEncode(ex.Message) + "</p>"));
                panel.Controls.Add(new LiteralControl("<pre>Stack Trace:\n" + ex.StackTrace + "</pre>"));
                Controls.Add(panel);
            }
            else
            {
                var panel = new Panel();
                panel.CssClass = "alert alert-info";
                panel.Controls.Add(new LiteralControl("Vi har desværre problemer med at vise dele af denne side."));
                Controls.Add(panel);
            }
        }

        /// <summary>
        /// Gets or sets a reference to the Content Page.
        /// </summary>
        public virtual ContentPartInstance ContentPartInstance { get; set; }

        /// <summary>
        /// Gets or sets a reference to the Content Part.
        /// </summary>
        public virtual ContentPart ContentPart { get; set; }

        /// <summary>
        /// Returns a string that can be used to prefix urls.
        /// </summary>
        public virtual string CultureUrlPrefix
        {
            get
            {
                if (Page is nCode.UI.Page)
                    return ((nCode.UI.Page)Page).CultureUrlPrefix;

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Content Part is currently in Edit Mode.
        /// </summary>
        public virtual bool IsEditing { get; set; }

        /// <summary>
        /// Loads the Content from the Database.
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Saves the Content to the Database.
        /// </summary>
        public virtual void SaveContent() { }
    }
}
