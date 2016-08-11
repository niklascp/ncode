using System.Collections.Generic;
using nCode.Configuration;

namespace nCode
{
    public sealed class FeatureGroup
    {       
        /// <summary>
        /// Creates a new FeatureGroup.
        /// </summary>
        internal FeatureGroup(Module module, string name)
        {
            Module = module;
            Name = name;
            Features = new List<Feature>();
        }

        /// <summary>
        /// Gets the Module that this FeatureGroup belongs to.
        /// </summary>
        public Module Module { get; private set; }

        /// <summary>
        /// Gets the name of this FeatureGroup. 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a list of Features in this FeatureGroup.
        /// </summary>
        public IList<Feature> Features {get; private set; }
    }
}
