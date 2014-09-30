using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Logging
{
    public abstract class LoggingEngine
    {
        /// <summary>
        /// Writes a Infomation Log Message.
        /// </summary>
        public abstract void Info(string message, Exception ex = null);

        /// <summary>
        /// Writes a Warning Log Message.
        /// </summary>
        public abstract void Warn(string message, Exception ex = null);

        /// <summary>
        /// Writes a Error Log Message.
        /// </summary>
        public abstract void Error(string message, Exception ex = null);
    }
}
