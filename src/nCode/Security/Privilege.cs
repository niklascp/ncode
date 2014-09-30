using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.Security
{
    /// <summary>
    /// Privilege object.
    /// </summary>
    public class Privilege
    {
        private Guid id;
        private string name;
        private string displayName;

        /// <summary>
        /// Initializes a new Privilege object.
        /// </summary>
        /// <param name="id">The unique identifier for this Privilege.</param>
        /// <param name="name">A static name for easy access.</param>
        /// <param name="displayName">A localized name to display.</param>
        public Privilege(Guid id, string name, string displayName)
        {
            this.id = id;
            this.name = name;
            this.displayName = displayName;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public Guid ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the localized DisplayName
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
        }
    }
}
