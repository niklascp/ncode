using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Drawing.Drawing2D;
using System.Linq;

namespace nCode.Imaging
{
    public static class GrayscaleImageExtension
    {
        /// <summary>
        /// Grayscales a image.
        /// </summary>
        public static Image GrayscaleImage(this Image inputImage)
        {
            /* Create a blank bitmap the same size as original */
            Bitmap newBitmap = new Bitmap(inputImage.Width, inputImage.Height);

            /* Get a graphics object from the new image */
            using (var g = Graphics.FromImage(newBitmap))
            {
                /* Create the grayscale ColorMatrix */
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][] {
                                    new float[] { .3f,  .3f,  .3f,   0f,   0f},
                                    new float[] {.59f, .59f, .59f,   0f,   0f},
                                    new float[] {.11f, .11f, .11f,   0f,   0f},
                                    new float[] {  0f,   0f,   0f,   1f,   0f},
                                    new float[] {  0f,   0f,   0f,   0f,   1f}
                                 });

                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(colorMatrix);


                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(inputImage, new Rectangle(0, 0, inputImage.Width, inputImage.Height),
                   0, 0, inputImage.Width, inputImage.Height, GraphicsUnit.Pixel, attributes);

                return newBitmap;
            }
        }

    }
}
