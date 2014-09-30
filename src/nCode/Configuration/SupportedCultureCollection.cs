using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    public class SupportedCultureCollection : ConfigurationElementCollection
    {
        public SupportedCulture this[int index]
        {
            get
            {
                return base.BaseGet(index) as SupportedCulture;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SupportedCulture();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SupportedCulture)element).Name;
        }
    }
}
