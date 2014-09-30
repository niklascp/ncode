using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    public class HostMappingCollection : ConfigurationElementCollection, IEnumerable<HostMappingElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new HostMappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HostMappingElement)element).Name;
        }

        public new IEnumerator<HostMappingElement> GetEnumerator()
        {
            for (int i = 0; i < base.Count; i++)
            {
                yield return base.BaseGet(i) as HostMappingElement;
            }
        }
    }
}
