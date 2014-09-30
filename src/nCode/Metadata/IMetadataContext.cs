using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    /// <summary>
    /// Interface for Metadata Contexts
    /// </summary>
    public interface IMetadataContext
    {
        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        T GetProperty<T>(string key, T defaultValue);

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SetProperty<T>(string key, T value);
    }
}
