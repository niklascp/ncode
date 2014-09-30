using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Drawing.Drawing2D;
using System.Linq;

namespace nCode.Imaging
{
    public static class CompressImageExtension
    {
        /// <summary>
        /// Compresses a image using JPEG.
        /// </summary>
        public static Image CompressImage(this Image inputImage, int quality = 75)
        {
            MemoryStream ms = new MemoryStream();

            /* Save as Jpeg with specific quality */
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
            ImageCodecInfo ic = ImageUtilities.GetEncoderInfo(ImageFormat.Jpeg);
            inputImage.Save(ms, ic, encparam);

            ms.Position = 0;

            return new Bitmap(ms);

            /* Notice: Don't dispose the memory stream, since it will throw a GDI+ exception.
             * See: http://stackoverflow.com/questions/336387/image-save-throws-a-gdi-exception-because-the-memory-stream-is-closed */
        }

    }
}
