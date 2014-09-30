using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Drawing.Drawing2D;
using System.Linq;

namespace nCode.Imaging
{
    static class RoundCornersExtention
    {
        /// <summary>
        /// Returns and image with rounded corners.
        /// </summary>
        public static Image RoundCorners(this Image startImage, int cornerRadius, Color backgroundColor)
        {
            // From: http://stackoverflow.com/questions/1758762/how-to-create-image-with-rounded-corners-in-c
            cornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(startImage.Width, startImage.Height);
            Graphics g = Graphics.FromImage(RoundedImage);
            g.Clear(backgroundColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush brush = new TextureBrush(startImage);
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            gp.AddArc(RoundedImage.Width - cornerRadius - 1, 0, cornerRadius, cornerRadius, 270, 90);
            gp.AddArc(RoundedImage.Width - cornerRadius - 1, 0 + RoundedImage.Height - cornerRadius - 1, cornerRadius, cornerRadius, 0, 90);
            gp.AddArc(0, RoundedImage.Height - cornerRadius - 1, cornerRadius, cornerRadius, 90, 90);
            g.FillPath(brush, gp);
            return RoundedImage;
        }

    }
}
