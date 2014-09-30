using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ObjectTypeGuidAttribute : Attribute
    {        
        public ObjectTypeGuidAttribute(string objectTypeGuid)
        {
            ObjectTypeGuid = new Guid(objectTypeGuid);
        }

        public Guid ObjectTypeGuid { get; private set; }
    }
}
