using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Data
{
    /// <summary>
    /// Instructs the Schema Update that the given columns should be indexed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class RenameAttribute : Attribute
    {        
        /// <summary>
        /// Creates a new Index Attribute that represent an index on the specified property names.
        /// </summary>
        public RenameAttribute(string oldName)
        {
            OldName = oldName;
        }

        /// <summary>
        /// Gets the names of the properties that this index is defined on.
        /// </summary>
        public string OldName { get; private set; }
    }
}
