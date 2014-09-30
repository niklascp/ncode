using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data
{
    /// <summary>
    /// Instructs the Schema Update to use the specified Cascade Rule on a Foreign Key Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CascadeRule : Attribute
    {
        public bool WillCascadeOnDelete { get; set; }

        /// <summary>
        /// Creates a new Cascade Rule Attribute.
        /// </summary>
        public CascadeRule(bool willCascadeOnDelete = true)
        {
            WillCascadeOnDelete = willCascadeOnDelete;
        }
    }
}
