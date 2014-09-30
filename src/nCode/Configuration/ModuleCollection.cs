using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using nCode.ProviderModel;

namespace nCode.Configuration
{
    public sealed class ModuleCollection : ProviderCollection<Module> 
    {
        public bool HasModule(string name)
        {
            return this.Any(x => x.Name == name);
        }
    
    }
}
