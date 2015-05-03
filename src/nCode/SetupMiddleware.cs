using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
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
                log.Info("Checking for frontend packages ...");

                /* TODO: */
                context.Response.Write("OK");
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
