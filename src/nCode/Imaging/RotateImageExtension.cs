using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Drawing.Drawing2D;
using System.Linq;

namespace nCode.Imaging
{
    public static class RotateImageExtension
    {
        /// <summary>
        /// Rotates a image using the given rotation angle (in degrees).
        /// </summary>
        public static Image RotateImage(this Image inputImage, int rotation, Color backgroundColor)
        {
            if (rotation < 0 || rotation > 359)
                throw new ArgumentException("Rotation must be between 0 and 359 degrees (both inclusive)", "rotation");

            double theta = (rotation * Math.PI / 180d);

            /* Caltulate bounding box width and height */
            int targetWidth = (int)Math.Ceiling(inputImage.Width * Math.Abs(Math.Cos(theta)) + inputImage.Height * Math.Abs(Math.Sin(theta)));
            int targetHeight = (int)Math.Ceiling(inputImage.Width * Math.Abs(Math.Sin(theta)) + inputImage.Height * Math.Abs(Math.Cos(theta)));

            Image targetImage = new Bitmap(targetWidth, targetHeight, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(targetImage);
            g.Clear(backgroundColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            while (theta > Math.PI / 2)
                theta -= Math.PI / 2;

            if (rotation <= 90)
                g.TranslateTransform((float)(inputImage.Height * Math.Sin(theta)), 0);
            else if (rotation <= 180)
                g.TranslateTransform(targetWidth, (float)(inputImage.Height * Math.Sin(theta)));
            else if (rotation <= 270)
                g.TranslateTransform((float)(inputImage.Width * Math.Cos(theta)), targetHeight);
            else
                g.TranslateTransform(0, (float)(inputImage.Width * Math.Cos(theta)));

            g.RotateTransform(rotation);

            g.DrawImage(inputImage, 0, 0);
            g.Dispose();
            return targetImage;
        }

    }
}
