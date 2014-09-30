using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data
{
    /// <summary>
    /// Instructs the Schema Update that the given table should not be maintained by this context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ForeignEntityAtrribute : Attribute
    {
        /// <summary>
        /// Creates a new Foreign Entity Attribute.
        /// </summary>
        public ForeignEntityAtrribute()
        { }
    }
}
