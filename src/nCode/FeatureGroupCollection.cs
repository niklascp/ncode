using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace nCode
{
    public sealed class FeatureGroupCollection : List<FeatureGroup>
    {
        /// <summary>
        /// Returns the Feature Group with the given name, or null if the Feature Group dosn't exists.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        public FeatureGroup this[string name]
        {
            get { return this.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)); }
        }
    }
}
