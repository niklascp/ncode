using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace nCode.IO
{
    public static class ImageUtilities
    {
        /// <summary>
        ///  Returns the image codec with the given mime type     
        /// </summary>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            // Find the correct image codec
            foreach (ImageCodecInfo codec in codecs)
            {
                if (string.Equals(codec.MimeType, mimeType, StringComparison.InvariantCultureIgnoreCase))
                    return codec;
            }
            return null;
        }

        public static Image ResizeImage(Image inputImage, int maxWidth, int maxHeight)
        {
            decimal inputRatio = (decimal)inputImage.Width / (decimal)inputImage.Height;
            decimal maxRatio = (decimal)maxWidth / (decimal)maxHeight;
            int outputWidth, outputHeight;

            if (inputRatio > maxRatio)
            {
                outputWidth = maxWidth;
                outputHeight = (int)Math.Ceiling((decimal)maxWidth / (decimal)inputImage.Width * (decimal)inputImage.Height);
            }
            else
            {
                outputWidth = (int)Math.Ceiling((decimal)maxHeight / (decimal)inputImage.Height * (decimal)inputImage.Width);
                outputHeight = maxHeight;
            }
            Bitmap outputImage = new Bitmap(outputWidth, outputHeight);

            using (Graphics outputGraphics = Graphics.FromImage(outputImage))
            {
                outputGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                outputGraphics.DrawImage(inputImage, new Rectangle(0, 0, outputWidth, outputHeight));
            }

            return outputImage;
        }
    }
}
