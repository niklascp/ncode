using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace nCode.Configuration
{
    public class ShortcutCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Shortcut();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Shortcut)element).Name;
        }
    }
}
