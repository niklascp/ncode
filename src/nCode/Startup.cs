using System.Globalization;
using System.Web.Http;
using Common.Logging;
using nCode.JobScheduling;
using nCode.JobScheduling.Hangfire;
using nCode.Logging.Log4Net;
using nCode.Search;
using Owin;

namespace nCode
{
    /// <summary>
    /// Main startup class for OWIN.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /* TODO: Make these Service Registrations configurable, 
             * and use OWIN extension method design pattern (e.g. UseLog4Net() etc.) 
             */
            Log.Initialize(new Log4NetLoggingEngine());

            var log = LogManager.GetLogger<Startup>();
            log.Info("Application is starting on OWIN ...");

            /* Test if system has been configured to enable Setup Middleware. */
            if (string.IsNullOrEmpty(Settings.ConnectionString) || !Settings.GetProperty("nCode.System.Setup", false))
            {
                log.Info("System has not been configured - injecting Setup middleware");
                app.Use<SetupMiddleware>();
            }
            /* Otherwise system has been configured and actual Middleware will be enabled. */
            else
            {
                log.Info("Starting Search Middleware ...");
                SearchHandler.Initialize(new LuceneSearchEngine());
                log.Info("Starting Job Scheduling Middleware ...");
                JobHandler.Initialize(new HangfireJobEngine());
            }

            /* Start each module. */
            foreach (var module in Settings.Modules)
            {
                log.Info(string.Format("Staring module: {0} ({1}) ...", module.Name, module.Version.ToString(CultureInfo.InvariantCulture)));
                module.Startup(app);
            }

            log.Info("Starting WebApi Middleware ...");
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.MapHttpAttributeRoutes();
            httpConfiguration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            httpConfiguration.EnsureInitialized();

            app.UseWebApi(httpConfiguration);
        }
    }
}
