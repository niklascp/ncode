using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Collections.Generic;

namespace nCode.Imaging
{
    public static class ImageUtilities
    {
        const string formats = ".jpg|.jpeg|.png";
        const int defaultQuality = 75;
        const string imageCache = "/Files/.ImageCache/";

        private static string GetTargetFileName(string sourceFile, ImageFormat format)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFile);
            string ext = Path.GetExtension(sourceFile);

            if (format == null)
                fileName += (!string.IsNullOrEmpty(ext) && formats.IndexOf(ext) >= 0) ? ext : ".jpg";
            else if (format == ImageFormat.Jpeg)
                fileName += ".jpg";
            else if (format == ImageFormat.Png)
                fileName += ".png";
            else
                throw new ApplicationException("Unknow Image Format: " + format.ToString());

            return fileName;
        }

        private static string GetTargetDirectory(
            string sourceFile, 
            int width, 
            int height,
            ImageResizeMode mode,
            bool grayscale,
            int roundedCornerRadius,
            int actualRotation)
        {
            var modeAppendix = 
                mode == ImageResizeMode.Fill    ? "f" : 
                mode == ImageResizeMode.Stretch ? "s" :
                mode == ImageResizeMode.Contain ? "c" : 
                string.Empty;

            var grayscaleAppendix = 
                grayscale ? "g" : 
                string.Empty;

            var roundedCornerRadiusAppendix =
                roundedCornerRadius != 0 ? "r" + roundedCornerRadius : 
                string.Empty;

            var rotationAppendix =
                actualRotation != 0 ? "'" + actualRotation : 
                string.Empty;

            /* Calculate the path of this image given the options */
            return string.Format("{0}{1}x{2}{3}{4}{5}{6}{7}/", 
                imageCache, 
                width, 
                height, 
                modeAppendix, 
                grayscaleAppendix, 
                roundedCornerRadiusAppendix, 
                rotationAppendix, 
                VirtualPathUtility.ToAbsolute(Path.GetDirectoryName(sourceFile)));
        }

        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().ToList().FirstOrDefault(x => x.FormatID == format.Guid);
        }

        [Obsolete]
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().ToList().FirstOrDefault(x => x.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Resizes a image using the given sizes and resize mode.
        /// </summary>
        public static Image ResizeImage(Image inputImage, int width, int height, ImageResizeMode mode = ImageResizeMode.Normal, Color? backgroundColor = null)
        {
            decimal inputRatio = (decimal)inputImage.Width / (decimal)inputImage.Height;
            decimal outputRatio = (decimal)width / (decimal)height;
            int outputWidth, outputHeight;
            
            Image outputImage = null;
            Size size;
            Point offset;

            switch (mode)
            {
                default:
                case ImageResizeMode.Normal:
                    
                    if (inputRatio > outputRatio)
                    {
                        outputWidth = Math.Min(width, inputImage.Width);
                        outputHeight = (int)Math.Ceiling(outputWidth / inputRatio);
                    }
                    else
                    {
                        outputHeight = Math.Min(height, inputImage.Height);
                        outputWidth = (int)Math.Ceiling(outputHeight * inputRatio);
                    }
            
                    outputImage = new Bitmap(outputWidth, outputHeight);

                    using (Graphics outputGraphics = Graphics.FromImage(outputImage))
                    {
                        outputGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        outputGraphics.DrawImage(inputImage, new Rectangle(0, 0, outputWidth, outputHeight));
                    }

                    break;

                case ImageResizeMode.Stretch:

                    outputImage = new Bitmap(width, height);

                    using (Graphics outputGraphics = Graphics.FromImage(outputImage))
                    {
                        outputGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        outputGraphics.DrawImage(inputImage, new Rectangle(0, 0, width, height));
                    }

                    break;

                case ImageResizeMode.Fill:

                    if (inputRatio > outputRatio)
                    {
                        size = new Size((int)Math.Ceiling(inputImage.Height * outputRatio), inputImage.Height);
                        offset = new Point((inputImage.Width - size.Width) / 2, 0);
                    }
                    else
                    {
                        size = new Size(inputImage.Width, (int)Math.Ceiling(inputImage.Width / outputRatio));
                        offset = new Point(0, (inputImage.Height - size.Height) / 2);
                    }

                    outputImage = new Bitmap(width, height);

                    using (Graphics outputGraphics = Graphics.FromImage(outputImage))
                    {
                        outputGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        outputGraphics.DrawImage(inputImage, new Rectangle(0, 0, width, height), new Rectangle(offset, size), GraphicsUnit.Pixel);
                    }

                    break;

                case ImageResizeMode.Contain:

                    if (inputRatio > outputRatio)
                    {
                        outputWidth = Math.Min(width, inputImage.Width);
                        outputHeight = (int)Math.Ceiling(outputWidth / inputRatio);                        
                    }
                    else
                    {
                        outputHeight = Math.Min(height, inputImage.Height);
                        outputWidth = (int)Math.Ceiling(outputHeight * inputRatio);
                    }
            
                    size = new Size(outputWidth, outputHeight);
                    offset = new Point((width - size.Width) / 2, (height - size.Height) / 2);
                    
                    outputImage = new Bitmap(width, height);

                    using (Graphics outputGraphics = Graphics.FromImage(outputImage))
                    {                        
                        outputGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        if (backgroundColor != null)
                            outputGraphics.Clear(backgroundColor.Value);
                        outputGraphics.DrawImage(inputImage, new Rectangle(offset, size));
                    }

                    break;
            }

            return outputImage;
        }

        public static string EnsureImageSize(string sourceFile, int width, int height)
        {
            return EnsureImageSize(sourceFile, width, height, ImageResizeMode.Normal);
        }

        /// <summary>
        /// Ensures that the given image exits and is up-to-date in the given size. Saves as PNG.
        /// </summary>        
        [Obsolete("Use EnsureImageSize with optional parameters.")]
        public static string EnsureImageSize(string sourceFile, int width, int height, int rotation)
        {
            return EnsureImageSize(sourceFile, width, height, ImageResizeMode.Normal, rotation: rotation, format: ImageFormat.Png);
        }

        /// <summary>
        /// Ensures that the given image exits and is up-to-date in the given size.
        /// </summary>        
        public static string EnsureImageSize(
            string sourceFile, 
            int width, int height, 
            ImageResizeMode mode = ImageResizeMode.Normal, 
            bool grayscale = false,
            ImageFormat format = null,
            int formatQuality = defaultQuality,            
            int roundCornerRadius = 0, 
            Color? backgroundColor = null,
            int rotation = 0)
        {
            if (sourceFile == null)
                throw new ArgumentNullException("sourceFile", "Source File cannot be null.");

            if (roundCornerRadius < 0)
                throw new ArgumentException("roundCornerRadius must be non-negative.");

            if (HttpContext.Current == null)
                throw new InvalidOperationException("Must have Current HttpContext");

            try
            {
                /* We only want positive rotation values between 0 and 359. */
                int actualRotation = rotation % 360;

                /* Fix for negative numbers, since C# preserves the sign during modulus, e.g. -90 % 360 = -90 and not 270. */
                if (actualRotation < 0)
                    actualRotation += 360;

                string targetDirectory = GetTargetDirectory(sourceFile, width, height, mode, grayscale, roundCornerRadius, actualRotation);
                string targetFile = targetDirectory + GetTargetFileName(sourceFile, format);

                FileInfo sourceInfo = new FileInfo(HttpContext.Current.Server.MapPath(sourceFile));
                FileInfo targetInfo = new FileInfo(HttpContext.Current.Server.MapPath(targetFile));

                /* Neither files exists - nothing to work width. */
                if (!sourceInfo.Exists && !targetInfo.Exists)
                    return null;

                /* Target files does not exists, or is out-of-date. */
                if (!targetInfo.Exists || targetInfo.LastAccessTimeUtc < sourceInfo.LastWriteTimeUtc)
                {
                    Image image;
                    List<Image> garbage = new List<Image>(); /* Keeps a pointer to every thing we need to dispose. */

                    /* Ensure target directory */
                    if (!targetInfo.Directory.Exists)
                        targetInfo.Directory.Create();

                    /* Load original image */
                    image = Image.FromFile(sourceInfo.FullName);
                    garbage.Add(image);

                    if (format == null)
                        format = image.RawFormat;

                    if (backgroundColor == null)
                        backgroundColor = format == ImageFormat.Png ? Color.Transparent : Color.White;

                    /* Resize */
                    image = ImageUtilities.ResizeImage(image, width, height, mode, backgroundColor.Value);
                    garbage.Add(image);

                    if (grayscale)
                    {
                        image = image.GrayscaleImage();
                        garbage.Add(image);
                    }

                    /* Round coners */
                    if (roundCornerRadius != 0)
                    {
                        image = image.RoundCorners(roundCornerRadius, backgroundColor.Value);
                        garbage.Add(image);
                    }

                    /* Rotate Image */
                    if (rotation != 0)
                    {
                        image = image.RotateImage(actualRotation, backgroundColor.Value);
                        garbage.Add(image);
                    }

                    /* Save as Jpeg with specific quality */
                    if (format == ImageFormat.Jpeg && formatQuality != defaultQuality) {
                        EncoderParameters encparam = new EncoderParameters(1);
                        encparam.Param[0] = new EncoderParameter(Encoder.Quality, (long)formatQuality);
                        ImageCodecInfo ic = GetEncoderInfo(format);
                        image.Save(targetInfo.FullName, ic, encparam);
                    }
                    /* Save as Png */
                    else {
                        image.Save(targetInfo.FullName, format);
                    }

                    /* Clean up. */
                    foreach (var imageToDispose in garbage)
                        imageToDispose.Dispose();
                }

                return targetFile;
            }
            catch (Exception ex)
            {
                /* Something went wrong. */
                Log.WriteEntry("System", "Ensure Image Size", EntryType.Warning, ex);
                return null;
            }
        }

        /// <summary>
        /// Returns the orientation of the given file.
        /// </summary>
        public static ImageOrientation GetImageOrientation(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file", "File cannot be null.");

            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(file));

            if (!fileInfo.Exists)
                return ImageOrientation.Unknown;

            using (var image = Image.FromFile(fileInfo.FullName))
            {
                if (image.Width > image.Height)
                    return ImageOrientation.Landscape;
                else
                    return ImageOrientation.Portrait;
            }
        }

        /// <summary>
        /// Get the dimensions of the given image file.
        /// </summary>
        public static Size GetImageSize(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file", "File cannot be null.");

            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(file));

            if (!fileInfo.Exists)
                return Size.Empty;

            using (var image = Image.FromFile(fileInfo.FullName))
            {
                return image.Size;
            }
        }

    }

    /// <summary>
    /// Indeicates how an image should be resized to a given width/height.
    /// </summary>
    public enum ImageResizeMode
    {
        /// <summary>
        /// Resizes the images, such it is as most maxWidth x maxHeight.
        /// </summary>
        Normal,
        /// <summary>
        /// Stretches the image, ignoring any aspect ratio. The output will have the dimension maxWidth x maxHeight.
        /// </summary>
        Stretch,
        /// <summary>
        /// Fills the output image from center, allowing cropping of edges. The output will have dimension the maxWidth x maxHeight.
        /// </summary>
        Fill,
        Contain
    }

    public enum ImageOrientation
    {
        Landscape,
        Portrait,
        Unknown
    }
}
