using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using nCode.Metadata.Model;
using System.Xml.Linq;

namespace nCode.Metadata
{
    public static class GenericMetadataHelper
    {
        /// <summary>
        /// Deserializes a xml-encoded property and returns it as type T. If serializedData is null the default value is returned.
        /// </summary>
        public static T Deserialize<T>(string serializedData, T defaultValue)
        {
            if (serializedData == null)
                return defaultValue;

            /* Copy the string data to a Memory Stream. */
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(serializedData);
                sw.Flush();

                /* Reset the Memory Stream. */
                ms.Position = 0;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                try
                {                    
                    return (T)xmlSerializer.Deserialize(ms);
                }
                catch (InvalidOperationException)
                {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Gets the given property from the database.
        /// </summary>
        public static T GetProperty<T>(Type objectType, Guid objectId, string key, T defaultValue)
        {
            var objectTypeAttribute = objectType.GetCustomAttributes(typeof(ObjectTypeGuidAttribute), false).SingleOrDefault() as ObjectTypeGuidAttribute;

            if (objectTypeAttribute == null)
                throw new InvalidOperationException(string.Format("Could not find an ObjectTypeGuidAttribute for the type '{0}'", objectType.FullName));

            return GetProperty<T>(objectTypeAttribute.ObjectTypeGuid, objectId, key, defaultValue);
        }

        /// <summary>
        /// Gets the given property from the database.
        /// </summary>
        public static T GetProperty<T>(Guid objectTypeId, Guid objectId, string key, T defaultValue)
        {
            using (var dbContext = new SystemDbContext())
            {
                var property = (from p in dbContext.MetadataProperties
                                where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId && p.Key == key
                                select p.Value).SingleOrDefault();

                return Deserialize<T>(property, defaultValue);
            }
        }

        /// <summary>
        /// Sets the given property in the database.
        /// </summary>
        public static void SetProperty<T>(Type objectType, Guid objectId, string key, T value)
        {
            var objectTypeAttribute = objectType.GetCustomAttributes(typeof(ObjectTypeGuidAttribute), false).SingleOrDefault() as ObjectTypeGuidAttribute;

            if (objectTypeAttribute == null)
                throw new InvalidOperationException(string.Format("Could not find an ObjectTypeGuidAttribute for the type '{0}'", objectType.FullName));

            SetProperty<T>(objectTypeAttribute.ObjectTypeGuid, objectId, key, value);
        }

        /// <summary>
        /// Sets the given property in the database.
        /// </summary>
        public static void SetProperty<T>(Guid objectTypeId, Guid objectId, string key, T value)
        {
            using (var dbContext = new SystemDbContext())
            {
                /* Serialize the data and copy to string. */
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(ms, value);

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    using (StreamReader sr = new StreamReader(ms))
                    {
                        var property = (from p in dbContext.MetadataProperties
                                        where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId && p.Key == key
                                        select p).SingleOrDefault();

                        /* Property does not exists. */
                        if (property == null)
                        {
                            property = new MetadataPropertyEntity();
                            property.ID = Guid.NewGuid();
                            property.ObjectTypeID = objectTypeId;
                            property.ObjectID = objectId;
                            property.Key = key;
                            dbContext.MetadataProperties.Add(property);
                        }

                        property.Value = sr.ReadToEnd();

                        dbContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of all property keys for the given type.
        /// </summary>
        public static IEnumerable<string> GetPropertyKeys(Guid objectTypeId, Guid objectId)
        {
            using (var dbContext = new SystemDbContext())
            {
                var keys = (from p in dbContext.MetadataProperties
                            where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId
                            select p.Key).ToList();

                return keys;
            }
        }

        /// <summary>
        /// Gets a list of all properties for the given type.
        /// </summary>
        public static IEnumerable<XElement> GetProperties(Guid objectTypeId, Guid objectId)
        {
            using (var dbContext = new SystemDbContext())
            {
                var properties = (from p in dbContext.MetadataProperties
                                  where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId
                                  select new
                                  {
                                      p.Key,
                                      p.Value
                                  }).ToList();

                var list = new List<XElement>(properties.Count);

                foreach (var p in properties)
                {
                    var doc = XDocument.Parse(p.Value);
                    doc.Root.Name = p.Key;
                    list.Add(doc.Root);
                }

                return list;
            }
        }

    }
}
