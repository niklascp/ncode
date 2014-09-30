using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Data
{
    /// <summary>
    /// Instructs the Schema Update that the given columns should be indexed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class IndexAttribute : Attribute
    {        
        /// <summary>
        /// Creates a new Index Attribute that represent an index on the specified property names.
        /// </summary>
        public IndexAttribute(params string[] columns)
        {
            Columns = columns;
        }


        /// <summary>
        /// Gets or sets the name of the index, or null if de default name should be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the names of the properties that this index is defined on.
        /// </summary>
        public string[] Columns { get; private set; }
    }
}
