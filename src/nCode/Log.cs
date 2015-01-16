using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using nCode.Logging;

namespace nCode
{

    /// <summary>
    /// Classify a Log entry.
    /// </summary>
    public enum EntryType
    {
        Information,
        Warning,
        Error,
        SuccessAudit,
        FailureAudit
    }

    public static class Log
    {
        private const string notInitializedExceptionMessage = "Logging is not initialized.";

        private static LoggingEngine log;

        /// <summary>
        /// Initializes Logging.
        /// </summary>
        public static void Initialize()
        {
            Initialize(new nCode.Logging.Log4Net.Log4NetLoggingEngine());
        }

        /// <summary>
        /// Initializes Logging.
        /// </summary>
        public static void Initialize(LoggingEngine loggingEngine)
        {
            if (loggingEngine == null)
                throw new ArgumentNullException("loggingEngine");

            log = loggingEngine;

            Info(string.Format("Initialized Logging Engine: {0}.", loggingEngine.GetType().FullName));
        }

        /// <summary>
        /// Writes a Infomation Log Message.
        /// </summary>
        public static void Info(string message, Exception ex = null)
        {
            if (log == null)
                throw new InvalidOperationException(notInitializedExceptionMessage);

            log.Info(message, ex);
        }


        /// <summary>
        /// Writes a Warning Log Message.
        /// </summary>
        public static void Warn(string message, Exception ex = null)
        {
            if (log == null)
                throw new InvalidOperationException(notInitializedExceptionMessage);

            log.Warn(message, ex);
        }


        /// <summary>
        /// Writes a Error Log Message.
        /// </summary>
        public static void Error(string message, Exception ex = null)
        {
            if (log == null)
                throw new InvalidOperationException(notInitializedExceptionMessage);

            log.Error(message, ex);
        }

        /// <summary>
        /// Writes a Error Log Message.
        /// </summary>
        public static void Verbose(string message, Exception ex = null)
        {
            if (log == null)
                throw new InvalidOperationException(notInitializedExceptionMessage);

            log.Verbose(message, ex);
        }



        /* 
         *  OBSOLETE METHODS: 
         */

        /// <summary>
        /// Writes a entry to the Log.
        /// </summary>
        /// <param name="module">The module that is executing.</param>
        /// <param name="action">The action tried to preform.</param>
        /// <param name="message">A messege descriping the entry.</param>
        /// <param name="type">A EntryType classifying the entry.</param>
        [Obsolete]
        public static void WriteEntry(string module, string action, string message, EntryType type)
        {
            WriteEntry(module, action, message, type, null);
        }

        /// <summary>
        /// Writes a entry to the Log.
        /// </summary>
        /// <param name="module">The module that is executing.</param>
        /// <param name="action">The action tried to preform.</param>
        /// <param name="type">>A EntryType classifying the entry.</param>
        /// <param name="ex">The exception occurred.</param>
        [Obsolete]
        public static void WriteEntry(string module, string action, EntryType type, Exception ex)
        {
            WriteEntry(module, action, null, type, ex);
        }

        /// <summary>
        /// Writes a entry to the Log.
        /// </summary>
        /// <param name="module">The module that is executing.</param>
        /// <param name="action">The action tried to preform.</param>
        /// <param name="message">A messege descriping the entry.</param>
        /// <param name="type">>A EntryType classifying the entry.</param>
        /// <param name="ex">The exception occurred.</param>
        [Obsolete]
        public static void WriteEntry(string module, string action, string message, EntryType type, Exception ex)
        {
            WriteEntry(type, module, action, message, ex);
        }

        /// <summary>
        /// Writes a entry to the Log.
        /// </summary>
        /// <param name="type">>A EntryType classifying the entry.</param>
        /// <param name="module">The module that is executing.</param>
        /// <param name="action">The action tried to preform.</param>
        /// <param name="message">A messege descriping the entry.</param>
        /// <param name="exception">The exception occurred.</param>
        [Obsolete]
        public static void WriteEntry(EntryType type, string module, string action, string message = null, Exception exception = null)
        {
            if (type == EntryType.Error)
                Error(module + ": " + action + " - " + message, exception);
            else if (type == EntryType.Warning)
                Warn(module + ": " + action + " - " + message, exception);
            else if (type == EntryType.Information)
                Info(module + ": " + action + " - " + message, exception);
        }
    }
}
