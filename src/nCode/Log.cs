using System;
using System.Runtime.CompilerServices;
using Common.Logging;

namespace nCode
{
    /// <summary>
    /// Classify a Log entry.
    /// </summary>
    [Obsolete]
    public enum EntryType
    {
        Information,
        Warning,
        Error,
        SuccessAudit,
        FailureAudit
    }

    /// <summary>
    /// Obsolete logging methods. Please use the Common.Logging Framework.
    /// </summary>
    [Obsolete]
    public static class Log
    {
        private const string obespleteMessage = "Please use the Common.Logging Framework.";

        private static ILog log;

        /// <summary>
        /// Initializes Logging. This is a no-op since migration to Common Logging.
        /// </summary>
        [Obsolete(obespleteMessage)]
        public static void Initialize()
        {
            /* No-Op */
        }

        /// <summary>
        /// Writes a Infomation Log Message.
        /// </summary>
        [Obsolete(obespleteMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string message, Exception ex = null)
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Info(message, ex);
        }


        /// <summary>
        /// Writes a Warning Log Message.
        /// </summary>
        [Obsolete(obespleteMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message, Exception ex = null)
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Warn(message, ex);
        }


        /// <summary>
        /// Writes a Error Log Message.
        /// </summary>
        [Obsolete(obespleteMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, Exception ex = null)
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Error(message, ex);
        }

        /// <summary>
        /// Writes a Trace Log Message.
        /// </summary>
        [Obsolete(obespleteMessage)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(string message, Exception ex = null)
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Trace(message, ex);
        }

        /// <summary>
        /// Writes a entry to the Log.
        /// </summary>
        /// <param name="module">The module that is executing.</param>
        /// <param name="action">The action tried to preform.</param>
        /// <param name="message">A messege descriping the entry.</param>
        /// <param name="type">A EntryType classifying the entry.</param>
        [Obsolete(obespleteMessage)]
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
        [Obsolete(obespleteMessage)]
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
        [Obsolete(obespleteMessage)]
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
        [Obsolete(obespleteMessage)]
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
