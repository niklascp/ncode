using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.Data
{
    public class SchemaConfigurationException : Exception
    {
        public SchemaConfigurationException(string message)
            : base(message)
        { }
    }
}
