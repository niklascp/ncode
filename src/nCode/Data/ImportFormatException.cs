using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Data
{
    public class ImportFormatException : Exception
    {
        /// <summary>
        /// Initializes a new ImportFormatException
        /// </summary>
        public ImportFormatException(string message, int? lineNumber = null, string data = null, Exception innerException = null)
            : base(message, innerException)
        {
            InputLine = lineNumber;
            InputData = data;
        }

        /// <summary>
        /// Gets or set the line number data for the Import Format Exception, if the inport is from a line numbered source.
        /// </summary>
        public int? InputLine { get; private set; }

        /// <summary>
        /// Gets or set any associated data for the Import Format Exception.
        /// </summary>
        public string InputData { get; private set; }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + (InputLine != null ? " at line " + InputLine : string.Empty) + (InputLine != null ? ":" + Environment.NewLine + InputLine : ".");
        }
    }
}
