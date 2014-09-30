using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Data
{
    /// <summary>
    /// Instructs the Schema Update that existing rows shuold be set to the given value. This is handy when introducing a new non-null column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SetExistingRowsAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string value;

        /// <summary>
        /// Instructs the Schema Update that existing rows shuold be set to the given value. This is handy when introducing a new non-null column.
        /// </summary>
        public SetExistingRowsAttribute(string value)
        {
            this.value = value;
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value
        {
            get { return value; }
        }
    }
}
