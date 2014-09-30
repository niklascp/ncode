using Common.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Logging.Log4Net
{
    /// <summary>
    /// Implements a Logging Engine that useslog4net.
    /// </summary>
    public class Log4NetLoggingEngine : LoggingEngine
    {
        private log4net.ILog log;

        /// <summary>
        /// Initializes Logging.
        /// </summary>
        public Log4NetLoggingEngine()
        {
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(typeof(Log4NetLoggingEngine));

            /* Set logging for subsystems that use Common.Logging */
            var properties = new NameValueCollection();
            properties["configType"] = "EXTERNAL"; // We have already called XmlConfigurator.Configure()
            Common.Logging.LogManager.Adapter = new nCode.Logging.Log4Net.Log4NetLoggerFactoryAdapter(properties);   
        }

        /// <summary>
        /// Writes a Infomation Log Message.
        /// </summary>
        public override void Info(string message, Exception ex = null)
        {
            if (ex != null)
                log.Info(message, ex);
            else
                log.Info(message);
        }

        /// <summary>
        /// Writes a Warning Log Message.
        /// </summary>
        public override void Warn(string message, Exception ex = null)
        {
            if (ex != null)
                log.Warn(message, ex);
            else
                log.Warn(message);
        }

        /// <summary>
        /// Writes a Error Log Message.
        /// </summary>
        public override void Error(string message, Exception ex = null)
        {
            if (ex != null)
                log.Error(message, ex);
            else
                log.Error(message);
        }
    }
}
