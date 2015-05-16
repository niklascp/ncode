using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Common.Logging;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Owin;

namespace nCode
{
    /// <summary>
    /// Middleware for System Setup.
    /// </summary>
    public class SetupMiddleware : OwinMiddleware
    {
        private readonly ILog log;
        /// <summary>
        /// Creates a SetupMiddleware
        /// </summary>
        public SetupMiddleware(OwinMiddleware next)
            : base(next)
        {
            log = LogManager.GetLogger<SetupMiddleware>();
        }

        /// <summary>
        /// Invokes SetupMiddleware
        /// </summary>
        public async override Task Invoke(IOwinContext context)
        {
            if ((string.IsNullOrEmpty(Settings.ConnectionString) || 
                !Settings.GetProperty("nCode.System.Setup", false)) &&
                context.Request.Uri.PathAndQuery.StartsWith("/Admin/System/Setup/Setup"))
            {
                var stopwatch = new Stopwatch();
                log.Info("Checking for frontend packages ...");

                /* TODO: */
                var packageName = "nCode.Core-1.1.6";
                var tempPath = HostingEnvironment.MapPath("~/Files/Temp");
                var unpackPath = HostingEnvironment.MapPath("~/Files/Temp/nCode.Core-1.1.6");

                var di = new DirectoryInfo(unpackPath);

                /* Clean unpack directory if already exists. */
                if (di.Exists)
                {
                    log.Info(string.Format("Directory '{0}' already exists - cleaning ...", unpackPath));

                    stopwatch.Restart();
                    di.Delete(true);
                    stopwatch.Stop();

                    log.Info(string.Format("Directory '{0}' was cleaned in {1:n0} ms.", unpackPath, stopwatch.ElapsedMilliseconds));
                }

                di.Create();

                stopwatch.Restart();

                ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(tempPath + "/nCode.Core-1.1.6.zip");


                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (!zipEntry.IsFile)
                        continue;

                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zipFile.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(di.FullName, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }

                stopwatch.Stop();
                log.Info(string.Format("Unpacked '{0}' in {1:n0} ms", packageName, stopwatch.ElapsedMilliseconds));

                context.Response.Write(string.Format("Unpacked '{0}' in {1:n0} ms", packageName, stopwatch.ElapsedMilliseconds));

            }
            else if ((string.IsNullOrEmpty(Settings.ConnectionString) || 
                !Settings.GetProperty("nCode.System.Setup", false)) &&
                !context.Request.Uri.PathAndQuery.StartsWith("/Admin/System/Setup/Setup"))
            {
                context.Response.StatusCode = 302;
                context.Response.Headers.Set("Location", "/Admin/System/Setup/Setup");
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}
