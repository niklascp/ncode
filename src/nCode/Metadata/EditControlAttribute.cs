using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EditControlAttribute : Attribute
    {
        public EditControlAttribute(string editControlPath)
        {
            EditControlPath = editControlPath;
        }

        public string EditControlPath { get; private set; }
    }
}
