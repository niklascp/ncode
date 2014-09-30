using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    public class ImportManagerConfigurationCollection : ConfigurationElementCollection, IEnumerable<ImportManagerConfigurationElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportManagerConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ImportManagerConfigurationElement)element).Name;
        }

        public new IEnumerator<ImportManagerConfigurationElement> GetEnumerator()
        {
            for (int i = 0; i < base.Count; i++)
            {
                yield return base.BaseGet(i) as ImportManagerConfigurationElement;
            }
        }
    }
}
