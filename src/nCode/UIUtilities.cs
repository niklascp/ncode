using System;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace nCode
{
    public static class UIUtilities
    {

        /// <summary>
        /// Add a JavaScript trapping code to the control, simulating a click on the button.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="b"></param>
        public static void TrapEnter(WebControl t, WebControl b)
        {
            if (b == null)
                t.Attributes.Add("onkeydown", "if (event.keyCode==13){return false;}");
            else
                t.Attributes.Add("onkeydown", "if (event.keyCode==13){document.getElementById('" + b.ClientID + "').focus();return true;}");
            //t.Page.ClientScript.RegisterClientScriptInclude("Common", "/javascript/common.js");
        }

        /// <summary>
        /// Resize an image. Default format is Jpeg.
        /// </summary>
        /// <param name="InputImage">Path to inputfile</param>
        /// <param name="OutputImage">Path to outputfile</param>
        /// <param name="MaxWidth">Max width in pixels</param>
        /// <param name="MaxHeight">Max height in pixels</param>
        public static void ResizeImage(string InputImage, string OutputImage, int MaxWidth, int MaxHeight)
        {
            int Width, Height;
            Bitmap OriginalBitmap = new Bitmap(InputImage);
            decimal OriginalRatio = (decimal)OriginalBitmap.Width / (decimal)OriginalBitmap.Height;
            decimal MaxRatio = (decimal)MaxWidth / (decimal)MaxHeight;
            if (OriginalRatio > MaxRatio)
            {
                Height = (int)Math.Ceiling((decimal)MaxWidth / (decimal)OriginalBitmap.Width * (decimal)OriginalBitmap.Height);
                Width = MaxWidth;
            }
            else
            {
                Height = MaxHeight;
                Width = (int)Math.Ceiling((decimal)MaxHeight / (decimal)OriginalBitmap.Height * (decimal)OriginalBitmap.Width);
            }
            Bitmap CachedBitmap = new Bitmap(Width, Height);
            Graphics CachedBitmapGraphics = Graphics.FromImage(CachedBitmap);
            CachedBitmapGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            CachedBitmapGraphics.DrawImage(OriginalBitmap, new Rectangle(0, 0, Width, Height));
            CachedBitmap.Save(OutputImage, System.Drawing.Imaging.ImageFormat.Jpeg);

            OriginalBitmap.Dispose();
            CachedBitmap.Dispose();
        }

        public static void IncludeStyleSheet(Page page, string stylesheet)
        {
            // CSS Includes
            HtmlLink link1 = new HtmlLink();
            link1.Href = stylesheet;
            link1.Attributes["type"] = "text/css";
            link1.Attributes["rel"] = "stylesheet";
            page.Header.Controls.Add(link1);
        }

        [Obsolete("Use exstension method Page.AddMetaTag")]
        public static void AddMetaTag(Page page, string name, string content)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            HtmlMeta metaTag = new HtmlMeta();
            metaTag.Name = name;
            metaTag.Content = content;
            page.Header.Controls.Add(metaTag);
        }
    }
}
