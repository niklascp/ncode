using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    /// <summary>
    /// Class for storing Site-wise Metadata.
    /// </summary>
    public class SiteMetadataContext : IMetadataContext
    {
        static Guid objectTypeId;
        static SiteMetadataContext instance;

        static SiteMetadataContext()
        {
            objectTypeId = new Guid("94366aaf-54c5-4d9e-a9af-e1a42046d04b");
            instance = new SiteMetadataContext();
        }

        /// <summary>
        /// Gets the Object Type Id for Site Metadata.
        /// </summary>
        public static Guid ObjectTypeId
        {
            get { return objectTypeId; }
        }

        /// <summary>
        /// Gets a singleton instance of this class.
        /// </summary>
        public static SiteMetadataContext Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets the given property from the database.
        /// </summary>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(objectTypeId, objectTypeId, key, defaultValue);
        }

        /// <summary>
        /// Sets the given property in the database.
        /// </summary>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(objectTypeId, objectTypeId, key, value);
        }
    }
}
