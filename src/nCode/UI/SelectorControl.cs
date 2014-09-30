using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace nCode.UI
{
    public class SelectorControl : Control, IPostBackDataHandler
    {
        private static readonly object EventValueChanged = new object();

        /// <summary>
        /// Gets or sets the CSS class rendered by the control.
        /// </summary>
        [Category("Appearance"), DefaultValue(""), CssClassProperty()]
        public string CssClass
        {
            get { return (string)ViewState["CssClass"]; }
            set { ViewState["CssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the Url for the Icon Image.
        /// </summary>
        [Category("Appearance"), DefaultValue(""), UrlProperty()]
        public string IconUrl
        {
            get { return (string)ViewState["IconUrl"]; }
            set { ViewState["IconUrl"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the control, excluding the Icon Image.
        /// </summary>
        [Category("Layout"), DefaultValue(typeof(Unit), "")]
        public Unit Width
        {
            get { return ViewState["Width"] != null ? (Unit)ViewState["Width"] : Unit.Empty; }
            set { ViewState["Width"] = value; }
        }

        /// <summary>
        /// Gets or sets the padding of the control.
        /// </summary>
        [Category("Layout"), DefaultValue(typeof(Unit), "1px")]
        public Unit Padding
        {
            get { return ViewState["Padding"] != null ? (Unit)ViewState["Width"] : new Unit("1px"); }
            set { ViewState["Padding"] = value; }
        }

        /// <summary>
        /// Gets or sets the value of the field. 
        /// </summary>
        [Bindable(true), Category("Behavior"), DefaultValue("")]
        public string Value
        {
            get { return (string)ViewState["Value"] ?? string.Empty; }
            set { ViewState["Value"] = value; }
        }

        protected virtual string BrowserUrl
        {
            get { return string.Empty; }
        }

        protected virtual string GetDisplayText()
        {
            return Value;
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[EventValueChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            StringBuilder sbOpenBrowser = new StringBuilder();
            sbOpenBrowser.AppendLine("function openSelector(url) {");
            sbOpenBrowser.AppendLine("  var w = 400;");
            sbOpenBrowser.AppendLine("  var h = 500;");
            sbOpenBrowser.AppendLine("  var x = (screen.width - w) / 2;");
            sbOpenBrowser.AppendLine("  var y = (screen.height - h) / 2;");
            sbOpenBrowser.AppendLine("  var selector = window.open(url, 'CategoryBrowser', 'toolbar=no,status=no,resizable=yes,dependent=yes,scrollbars=yes,width='+w+',height='+h+',top='+y+',left='+x);");
            sbOpenBrowser.AppendLine("  if (selector)");
            sbOpenBrowser.AppendLine("    selector.opener = window;");
            sbOpenBrowser.AppendLine("}");
            Page.ClientScript.RegisterClientScriptBlock(typeof(SelectorControl), "OpenSelector", sbOpenBrowser.ToString(), true);

            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Hidden Field
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            if (!string.IsNullOrEmpty(Value))
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Value);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            string onClick = "openSelector('" + ResolveClientUrl(BrowserUrl) + "?id=" + ClientID + "');";

            // Outer Div
            if (!string.IsNullOrEmpty(CssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            if (!Width.IsEmpty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            // Inner Div
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_DisplayField");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, Padding.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            string s = GetDisplayText();
            if (!string.IsNullOrEmpty(s))
                writer.Write(s);
            else
                writer.Write("&nbsp");

            writer.RenderEndTag(); // Inner Div

            writer.RenderEndTag(); // Outer Div

            // A litle space between the Display Field and the Icon Image
            writer.WriteLine();

            // Icon Image
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + onClick);
            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "3px");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveClientUrl(IconUrl));
            writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag(); // Img
            writer.RenderEndTag(); // Link
        }

        #region IPostBackDataHandler Members

        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string postData = postCollection[postDataKey];
            if (!Value.Equals(postData, StringComparison.Ordinal))
            {
                Value = postData;
                return true;
            }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
            OnValueChanged(EventArgs.Empty);
        }

        #endregion
    }
}
